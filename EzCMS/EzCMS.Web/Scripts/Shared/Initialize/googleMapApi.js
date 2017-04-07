var defaultAddress = defaultAddress || '';
var defaultLatitude = defaultLatitude || 0;
var defaultLongitude = defaultLongitude || 0;
var defaultZoom = defaultZoom || 0;
var defaultPosition = new google.maps.LatLng(defaultLatitude, defaultLongitude);

//TODO: update this file

function noEnter(event) {
    if (event.keyCode == 13) {
        event.preventDefault();
        return false;
    }
    return true;
}

function initialize() {
    var geo = new google.maps.Geocoder;
    var address = $("#address-input").val();
    if (address == '') {
        setupMap(defaultPosition);
    } else {
        geo.geocode({ 'address': address }, function (results, status) {
            var position = defaultPosition;
            if (status == google.maps.GeocoderStatus.OK) {
                position = results[0].geometry.location;
            } else {
                //alert("Geocode was not successful for the following reason: " + status);
            }

            setupMap(position);
        });
    }
}

function setupMap(position) {
    var mapOptions = {
        center: position,
        zoom: defaultZoom
    };
    var map = new google.maps.Map(document.getElementById('mapCanvas'),
      mapOptions);

    var input = document.getElementById('address-input');
    var types = document.getElementById('address-buttons');

    map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);
    map.controls[google.maps.ControlPosition.TOP_LEFT].push(types);

    var autocomplete = new google.maps.places.Autocomplete(input);
    autocomplete.bindTo('bounds', map);

    var infowindow = new google.maps.InfoWindow();
    var marker = new google.maps.Marker({
        map: map,
        anchorPoint: new google.maps.Point(0, -29)
    });

    google.maps.event.addListener(autocomplete, 'place_changed', function () {
        infowindow.close();
        marker.setVisible(false);
        var place = autocomplete.getPlace();
        if (!place.geometry) {
            return;
        }

        // If the place has a geometry, then present it on a map.
        if (place.geometry.viewport) {
            map.fitBounds(place.geometry.viewport);
        } else {
            map.setCenter(place.geometry.location);
            map.setZoom(defaultZoom);
        }
        marker.setIcon(/** @type {google.maps.Icon} */({
            url: place.icon,
            size: new google.maps.Size(71, 71),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(17, 34),
            scaledSize: new google.maps.Size(35, 35)
        }));
        marker.setPosition(place.geometry.location);
        marker.setVisible(true);

        var address = '';
        if (place.address_components) {
            address = [
              (place.address_components[0] && place.address_components[0].short_name || ''),
              (place.address_components[1] && place.address_components[1].short_name || ''),
              (place.address_components[2] && place.address_components[2].short_name || '')
            ].join(' ');
        }

        infowindow.setContent('<div><strong>' + place.name + '</strong><br>' + address);
        infowindow.open(map, marker);
    });
}

google.maps.event.addDomListener(window, 'load', initialize);

