let mapInstance = null;
let markerInstance = null;

window.coopMap = {
    initialize: function (elementId, dotNetHelper, initialLat, initialLng) {
        const container = document.getElementById(elementId);
        if (!container) {
            setTimeout(function () {
                window.coopMap.initialize(elementId, dotNetHelper, initialLat, initialLng);
            }, 100);
            return;
        }

        // If there's an existing map instance, remove it to prevent duplicates on hot reload or navigation
        if (mapInstance) {
            mapInstance.remove();
            mapInstance = null;
            markerInstance = null;
        }

        const defaultLat = initialLat || 41.9028; // Default to Rome latitude
        const defaultLng = initialLng || 12.4964; // Default to Rome longitude
        const zoomLevel = (initialLat && initialLng) ? 13 : 5;

        mapInstance = L.map(elementId).setView([defaultLat, defaultLng], zoomLevel);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '© OpenStreetMap contributors'
        }).addTo(mapInstance);

        // Create draggable marker
        markerInstance = L.marker([defaultLat, defaultLng], {
            draggable: true
        }).addTo(mapInstance);

        // Event listener for dragging the marker
        markerInstance.on('dragend', function (event) {
            const position = markerInstance.getLatLng();
            dotNetHelper.invokeMethodAsync('UpdateCoordinatesFromMap', position.lat, position.lng);
        });

        // Event listener for clicking on the map
        mapInstance.on('click', function (event) {
            const lat = event.latlng.lat;
            const lng = event.latlng.lng;
            markerInstance.setLatLng([lat, lng]);
            dotNetHelper.invokeMethodAsync('UpdateCoordinatesFromMap', lat, lng);
        });

        // Force a size recalculation after the DOM settles
        setTimeout(function () {
            if (mapInstance) {
                mapInstance.invalidateSize();
            }
        }, 200);
    },

    updateMarker: function (lat, lng) {
        if (markerInstance && mapInstance) {
            markerInstance.setLatLng([lat, lng]);
            mapInstance.setView([lat, lng], 13);
        }
    },

    getUserLocation: function (dotNetHelper) {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                function (position) {
                    const lat = position.coords.latitude;
                    const lng = position.coords.longitude;
                    dotNetHelper.invokeMethodAsync('OnLocationSuccess', lat, lng);
                },
                function (error) {
                    let errMsg = "Unable to retrieve your location.";
                    switch (error.code) {
                        case error.PERMISSION_DENIED:
                            errMsg = "User denied the request for Geolocation.";
                            break;
                        case error.POSITION_UNAVAILABLE:
                            errMsg = "Location information is unavailable.";
                            break;
                        case error.TIMEOUT:
                            errMsg = "The request to get user location timed out.";
                            break;
                    }
                    dotNetHelper.invokeMethodAsync('OnLocationError', errMsg);
                }
            );
        } else {
            dotNetHelper.invokeMethodAsync('OnLocationError', "Geolocation is not supported by this browser.");
        }
    },

    getUserCoordinates: function () {
        return new Promise(function (resolve, reject) {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(
                    function (position) {
                        resolve({ latitude: position.coords.latitude, longitude: position.coords.longitude });
                    },
                    function (error) {
                        reject(error.message);
                    }
                );
            } else {
                reject("Geolocation is not supported by this browser.");
            }
        });
    }
};
