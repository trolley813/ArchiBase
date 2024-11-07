class LeafletMap {
    constructor(containerId, lat, lon, zoom, dotNetHelper) {
        this.map = L.map(containerId).setView([lat, lon], zoom);
        // de-Ukrainize the attribution prefix
        var attribution = this.map.attributionControl;
        attribution.setPrefix(
            '<a href="https://leafletjs.com">Leaflet</a> <img src="/flags/ru.svg" style="height: 12px"/>');
        L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution:
                '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
        }).addTo(this.map);
        this.markers = {};
        this.cameraMarker = null;
        this.principalMarker = null;
        this.buildingMarkersLayer = null;
        window.dotNetHelper = dotNetHelper;
    }

    addBuildingMarkers(points) {
        var markers = L.markerClusterGroup({ disableClusteringAtZoom: 16 });

        for (var i = 0; i < points.length; i++) {
            var a = points[i];
            var text = a[2];
            var title = a[3];
            var textColor = a[4];
            var color = a[5];
            let circleSVGString =
                `<svg version="1.2" baseProfile="tiny" xmlns="http://www.w3.org/2000/svg" width="250" height="250"><rect x="5" y="5" width="240" height="240" rx="40" fill="${color}"/><text x="50%" y="50%" text-anchor="middle" fill="${textColor}" font-size="100px" font-family="Arial" dy=".3em">${text}</text></svg>`
            let iconURL = 'data:image/svg+xml,' + encodeURIComponent(circleSVGString);
            var icon = L.icon({ iconUrl: iconURL, iconSize: [30, 30] });
            var marker =
                L.marker(new L.LatLng(a[0], a[1]), { title: title, icon: icon }).on('click', function (e) { onMarkerClick(e) });
            markers.addLayer(marker);
            this.markers[text] = marker;
        }
        this.buildingMarkersLayer = markers;
        this.map.addLayer(markers);
        this.map.on('click', onMapClick);
    }

    addCameraMarker(lat, lon, angle) {
        var icon = L.divIcon({
            className: '',
            html: `<img src="/icons/camera.svg" style="transform-origin: center; transform: rotate(${angle}deg); width: 30px; height: 30px;" />`
        });
        this.cameraMarker =
            new L.marker(new L.LatLng(lat, lon), { icon: icon });
        this.cameraMarker.addTo(this.map);
    }

    addPrincipalMarker(lat, lon) {
        this.principalMarker =
            new L.marker(new L.LatLng(lat, lon));
        this.principalMarker.addTo(this.map);
    }

    updatePrincipalMarker(lat, lon) {
        if (this.principalMarker != null) this.principalMarker.setLatLng(new L.LatLng(lat, lon));
        else addPrincipalMarker(lat, lon);
    }

    updateCameraMarker(lat, lon, angle) {
        if (this.cameraMarker != null) {
            this.cameraMarker.setLatLng(new L.LatLng(lat, lon));
            var icon = L.divIcon({
                className: '',
                html: `<img src="/icons/camera.svg" style="transform-origin: center; transform: rotate(${angle}deg); width: 30px; height: 30px;" />`
            });
            this.cameraMarker.setIcon(icon);
        } else addCameraMarker(lat, lon, angle);
    }

    updateBuildingMarkers(points) {
        this.markers = {};
        this.map.removeLayer(this.buildingMarkersLayer);
        this.buildingMarkersLayer = null;
        addBuildingMarkers(points);
    }
}

function createLeafletMap(containerId, lat, lon, zoom, dotNetHelper) {
    window.abLeafletMap = new LeafletMap(containerId, lat, lon, zoom, dotNetHelper)
}

function addBuildingMarkers(points) {
    window.abLeafletMap.addBuildingMarkers(points)
}

function addCameraMarker(lat, lon, angle) {
    window.abLeafletMap.addCameraMarker(lat, lon, angle)
}

function addPrincipalMarker(lat, lon) {
    window.abLeafletMap.addPrincipalMarker(lat, lon)
}

function updateBuildingMarkers(points) {
    window.abLeafletMap.updateBuildingMarkers(points)
}

function updateCameraMarker(lat, lon, angle) {
    window.abLeafletMap.updateCameraMarker(lat, lon, angle)
}

function updatePrincipalMarker(lat, lon) {
    window.abLeafletMap.updatePrincipalMarker(lat, lon)
}


function onMapClick(e) {
    window.dotNetHelper.invokeMethodAsync("OnInternalMapClick", e.latlng);
}

function onMarkerClick(e) {
    window.dotNetHelper.invokeMethodAsync("OnInternalMarkerClick", e.latlng);
}