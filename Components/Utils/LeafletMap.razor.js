class LeafletMap {
    constructor(containerId, lat, lon, zoom, dotNetHelper) {
        var osm = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution:
                '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
        });
        var ksm = L.tileLayer('https://{s}tilecart.kosmosnimki.ru/kosmo/{z}/{x}/{y}.png', {
            attribution:
                '&copy; <a href="https://kosmosnimki.ru/">ООО ИТЦ "СКАНЭКС"</a> | &copy; участники сообщества <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
            updateWhenIdle: false,
            maxZoom: 18,
            maxNativeZoom: 17,
        })
        // var ydx = L.yandex({ attribution: '&copy; <a href="https://yandex.ru/maps">Yandex</a>' });
        this.map = L.map(containerId).setView([lat, lon], zoom);
        this.map._layersMaxZoom = 19;
        // de-Ukrainize the attribution prefix
        var attribution = this.map.attributionControl;
        attribution.setPrefix(
            '<a href="https://leafletjs.com">Leaflet</a> <img src="/flags/ru.svg" style="height: 12px"/>');
        attribution.setPosition('bottomleft');
        var baseMaps = {
            "OpenStreetMap": osm,
            "Kosmosnimki": ksm,
            // "Yandex": ydx
        }
        // no overlays yet
        L.control.layers(baseMaps, {}, { collapsed: false }).addTo(this.map);
        var selectedLayer = baseMaps[window.localStorage.getItem('archibaseSelectedLayer')];
        if (selectedLayer != null) {
            console.log(window.localStorage.getItem('archibaseSelectedLayer'));
            selectedLayer.addTo(this.map);
        }
        this.map.on('click', onMapClick);
        this.map.on('baselayerchange', function (e) {
            window.localStorage.setItem('archibaseSelectedLayer', e.name);
        });
        this.cameraMarker = null;
        this.principalMarker = null;
        this.buildingMarkersLayer = null;
        this.sublocationMarkersLayer = null;
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
        }
        this.buildingMarkersLayer = markers;
        this.map.addLayer(markers);
    }

    addSublocationMarkers(points) {
        var markers = L.markerClusterGroup({ disableClusteringAtZoom: 14 });
        for (var i = 0; i < points.length; i++) {
            var a = points[i];
            var title = a[2];
            let circleSVGString =
                `<svg version="1.2" baseProfile="tiny" xmlns="http://www.w3.org/2000/svg" width="250" height="250"><circle cx="125" cy="125" r="120" fill="#dc143c"/></svg>`
            let iconURL = 'data:image/svg+xml,' + encodeURIComponent(circleSVGString);
            var icon = L.icon({ iconUrl: iconURL, iconSize: [30, 30] });
            var marker =
                L.marker(new L.LatLng(a[0], a[1]), { title: title, icon: icon }).on('click', function (e) { onSublocationMarkerClick(e) });
            markers.addLayer(marker);
        }
        this.sublocationMarkersLayer = markers;
        this.map.addLayer(markers);
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
        this.principalMarker.options.icon.style = "filter: hue-rotate(120deg)";
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
        if (this.buildingMarkersLayer != null) {
            this.buildingMarkersLayer.clearLayers();
        }
        this.buildingMarkersLayer = null;
        addBuildingMarkers(points);
    }

    updateSublocationMarkers(points) {
        if (this.sublocationMarkersLayer != null) {
            this.sublocationMarkersLayer.clearLayers();
        }
        this.sublocationMarkersLayer = null;
        addSublocationMarkers(points);
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

function addSublocationMarkers(points) {
    window.abLeafletMap.addSublocationMarkers(points)
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

function updateSublocationMarkers(points) {
    window.abLeafletMap.updateSublocationMarkers(points)
}

function onMapClick(e) {
    window.dotNetHelper.invokeMethodAsync("OnInternalMapClick", e.latlng);
}

function onMarkerClick(e) {
    window.dotNetHelper.invokeMethodAsync("OnInternalMarkerClick", e.latlng);
}

function onSublocationMarkerClick(e) {
    window.dotNetHelper.invokeMethodAsync("OnInternalSublocationMarkerClick", e.latlng);
}