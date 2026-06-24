# TIT4TAXI

**TIT4TAXI** is a collaborative coordination protocol and ledger system designed to resolve resource-allocation conflicts and market imbalances between competing transport groups sharing common transit infrastructures.

---

## 💡 Core Principles

### 1. The Common Pool Resource (CPR) Problem
In metropolitan transportation, high-demand transit hubs—such as airport pickup terminals, train stations, and designated taxi stands—act as **Common Pool Resources (CPRs)**. These hubs are highly subtractable (one driver picking up a passenger leaves fewer passengers for others) yet difficult to exclude drivers from. Without a structured coordination mechanism, these hubs suffer from the *Tragedy of the Commons*: chronic congestion, long idle times, predatory competition, and a degraded experience for both commuters and drivers.

### 2. Market Asymmetry, Territorial Licensing & Reciprocity
To establish a functional cooperative framework, the system must recognize and counteract the inherent **market asymmetries** and **territorial licenses** governing the transport cooperatives:
- **Territorial Boundaries & Travel:** While vehicles are permitted to travel freely between different territories (e.g., driving a passenger from Territory A to Territory B), a Cooperative is entitled to load/pick up passengers only in its home territory.
- **Reciprocal Loading Rights:** To prevent inefficient empty return trips, the system mediates the rights for drivers of one Cooperative to load passengers in the opposite territory.
- **Token-Weighted Markets:** Since different territories (markets) have unequal sizes, demands, and values, a naive 1:1 exchange is unsustainable. TIT4TAXI regulates reciprocity using a ledger with a token weighted by the established economic values of the respective markets. This ensures a balanced, fair, and proportional exchange of cross-border passenger-loading rights.

### 3. Elinor Ostrom's Governing the Commons
The theoretical foundation of TIT4TAXI is inspired by **Elinor Ostrom's** seminal work on *Governing the Commons*. Ostrom demonstrated that common-pool resources can be sustainably managed by the users themselves without relying solely on state regulation or privatization. TIT4TAXI operationalizes Ostrom's design principles by:
- **Defining boundaries** for resource access.
- **Matching rules** to local transport demand and hub conditions.
- **Allowing collective modification** of operational rules.
- **Enabling low-cost conflict resolution** through the ledger system.

### 4. The Role of the Controller
Trust in a self-governing commons is maintained through active verification. TIT4TAXI incorporates a **Controller** role responsible for ensuring rules are respected. The Controller:
- Monitors compliance with queueing, pickup, and token-spending rules.
- Verifies real-time event logs and check-ins at transit hubs.
- Audits ledger balances to prevent double-spending or unauthorized transactions.
- Applies graduated adjustments or flags violations when deviation from the rules is detected.

---

[A youtube link that explains the problem (in Italian, only audio)](https://youtu.be/lOG7-_cjANA?si=Lcsoi7MAJYHG3Ut7) (Note: A.I. generated)
[A paper/preprint](https://zenodo.org/records/20824501)

---

## 📄 License

This project is licensed under the **Business Source License 1.1** (BSL-1.1). 

* **Before June 21, 2030:** Licensed for non-production, evaluation, testing, and development use only. Commercial production use requires a separate license.
* **On/After June 21, 2030:** Automatically transitions to the **GNU General Public License v3.0 or later (GPL-3.0+)**.

For more details, see the [LICENSE](file:///Users/antoniolucca/github/titfortat/LICENSE) file.


