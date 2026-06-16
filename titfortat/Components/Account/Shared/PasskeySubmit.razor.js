const browserSupportsPasskeys =
    typeof navigator.credentials !== 'undefined' &&
    typeof window.PublicKeyCredential !== 'undefined' &&
    typeof window.PublicKeyCredential.parseCreationOptionsFromJSON === 'function' &&
    typeof window.PublicKeyCredential.parseRequestOptionsFromJSON === 'function';

async function fetchWithErrorHandling(url, options = {}) {
    const response = await fetch(url, {
        credentials: 'include',
        ...options
    });
    if (!response.ok) {
        const text = await response.text();
        console.error(text);
        throw new Error(`The server responded with status ${response.status}.`);
    }
    return response;
}

async function createCredential(email, headers, signal) {
    const url = email ? `/Account/RegisterPasskeyOptions?username=${email}` : '/Account/PasskeyCreationOptions';
    const optionsResponse = await fetchWithErrorHandling(url, {
        method: 'POST',
        headers,
        signal,
    });
    const optionsJson = await optionsResponse.json();
    const options = PublicKeyCredential.parseCreationOptionsFromJSON(optionsJson);
    return await navigator.credentials.create({ publicKey: options, signal });
}

async function requestCredential(email, mediation, headers, signal) {
    const optionsResponse = await fetchWithErrorHandling(`/Account/PasskeyRequestOptions?username=${email}`, {
        method: 'POST',
        headers,
        signal,
    });
    const optionsJson = await optionsResponse.json();
    const options = PublicKeyCredential.parseRequestOptionsFromJSON(optionsJson);
    return await navigator.credentials.get({ publicKey: options, mediation, signal });
}

customElements.define('passkey-submit', class extends HTMLElement {
    static formAssociated = true;

    connectedCallback() {
        this.internals = this.attachInternals();
        this.attrs = {
            operation: this.getAttribute('operation'),
            name: this.getAttribute('name'),
            emailName: this.getAttribute('email-name'),
            requestTokenName: this.getAttribute('request-token-name'),
            requestTokenValue: this.getAttribute('request-token-value'),
        };

        this.internals.form.addEventListener('submit', (event) => {
            if (event.submitter?.name === '__passkeySubmit') {
                event.preventDefault();
                this.obtainAndSubmitCredential();
            }
        });

        this.tryAutofillPasskey();
    }

    disconnectedCallback() {
        this.abortController?.abort();
    }

    async obtainCredential(useConditionalMediation, signal) {
        if (!browserSupportsPasskeys) {
            throw new Error('Some passkey features are missing. Please update your browser.');
        }

        const headers = {
            [this.attrs.requestTokenName]: this.attrs.requestTokenValue,
        };

        if (this.attrs.operation === 'Create') {
            const email = this.attrs.emailName ? new FormData(this.internals.form).get(this.attrs.emailName) : null;
            return await createCredential(email, headers, signal);
        } else if (this.attrs.operation === 'Request') {
            const email = new FormData(this.internals.form).get(this.attrs.emailName);
            const mediation = useConditionalMediation ? 'conditional' : undefined;
            return await requestCredential(email, mediation, headers, signal);
        } else {
            throw new Error(`Unknown passkey operation '${this.attrs.operation}'.`);
        }
    }

    async obtainAndSubmitCredential(useConditionalMediation = false) {
        this.abortController?.abort();
        this.abortController = new AbortController();
        const signal = this.abortController.signal;
        
        let credentialJson = '';
        let errorMessage = '';

        try {
            const credential = await this.obtainCredential(useConditionalMediation, signal);
            credentialJson = JSON.stringify(credential);
        } catch (error) {
            if (error.name === 'AbortError') {
                return;
            }
            console.error(error);
            if (useConditionalMediation) {
                return;
            }
            errorMessage = error.name === 'NotAllowedError'
                ? 'No passkey was provided by the authenticator.'
                : error.message;
        }

        if (credentialJson) {
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = `${this.attrs.name}.CredentialJson`;
            input.value = credentialJson;
            this.internals.form.appendChild(input);
        }
        
        if (errorMessage) {
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = `${this.attrs.name}.Error`;
            input.value = errorMessage;
            this.internals.form.appendChild(input);
        }

        // Add a hidden input to simulate the submit button being pressed
        // so Blazor knows which form/action was triggered if necessary
        const submitInput = document.createElement('input');
        submitInput.type = 'hidden';
        submitInput.name = '__passkeySubmit';
        submitInput.value = '';
        this.internals.form.appendChild(submitInput);

        this.internals.form.requestSubmit();
    }

    async tryAutofillPasskey() {
        if (browserSupportsPasskeys && this.attrs.operation === 'Request' && await PublicKeyCredential.isConditionalMediationAvailable?.()) {
            await this.obtainAndSubmitCredential(/* useConditionalMediation */ true);
        }
    }
});
