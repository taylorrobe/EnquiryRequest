
var map;
var selectedShape;
var allShapes = [];
var shapeEventListeners = [];
var shapeId = 0;
var drawingManager;

function UserException(message) {
    this.message = message;
    this.name = 'UserException';
}
function TryParseInt(str, defaultValue) {
    var retValue = defaultValue;
    if (str !== null) {
        if (str.length > 0) {
            if (!isNaN(str)) {
                retValue = parseInt(str);
            }
        }
    }
    return retValue;
}
function getDrawingManager() {
    drawingManager = new google.maps.drawing.DrawingManager({
        drawingMode: google.maps.drawing.OverlayType.MARKER,
        drawingControl: true,
        drawingControlOptions: {
            position: google.maps.ControlPosition.TOP_CENTER,
            drawingModes: ['marker', 'polygon']
        },
        markerOptions: {
            icon: '../../images/CustomMapMarkers/red_MarkerA.png',
            draggable: true
        }
    });
    return drawingManager;
}
function clearSelection() {
    if (selectedShape) {
        console.log("clearSelection");
        if (selectedShape.type === "polygon") {
            selectedShape.setEditable(false);
            selectedShape.setDraggable(false);
        }
        if (selectedShape.type === "marker") {
            selectedShape.setIcon('../../images/CustomMapMarkers/red_MarkerA.png');
            selectedShape.setDraggable(false);
        }
        selectedShape = null;
    }
}

function setSelection(shape) {
    console.log("setSelection" + shape);
    clearSelection();
    selectedShape = shape;
    if (shape.type === "polygon") {
        shape.setEditable(true);
        shape.setDraggable(true);
    }
    if (shape.type === "marker") {
        shape.setIcon('../../images/CustomMapMarkers/yellow_MarkerA.png');
        shape.setDraggable(true);
    }

    var infoOn = document.getElementById("checkBoxShowInfo").checked;
    if (infoOn) {
        showInfo(shape);
    }
}

function deleteShape(shape) {
    shape.setMap(null);
    var index = allShapes.indexOf(shape);
    if (index !== -1) {
        allShapes.splice(index, 1);
        shapeEventListeners.splice(index, 1);
    }
    selectedShape = null;

}

function deleteSelectedShape() {
    if (selectedShape) {
        deleteShape(selectedShape);
    }
}

function addNewShape(event) {
    var newShape = event.overlay;
    newShape.type = event.type;
    shapeId++;
    var allShapesSize = allShapes.push(newShape);
    var eventListener = google.maps.event.addListener(newShape, 'click', function shapeClickListener(event) {
        setSelection(newShape, event);
    });
    shapeEventListeners.splice(allShapesSize - 1, 0, eventListener);
    setSelection(newShape, event);
}

function addNewMarker(event) {
    var newMarker = event.overlay;
    newMarker.type = event.type;
    shapeId++;
    var allShapesSize = allShapes.push(newMarker);
    var eventListener = google.maps.event.addListener(newMarker, 'click', function shapeClickListener(event) {
        setSelection(newMarker, event);
    });
    shapeEventListeners.splice(allShapesSize - 1, 0, eventListener);
    setSelection(newMarker, event);
}

function disableDrawing() {
    drawingManager.setDrawingMode(null);
    drawingManager.setOptions({
        drawingControl: false
    });
    clearSelection();
}

function getWktFromShape(shape) {
    var wkt = "";
    if (shape.type === google.maps.drawing.OverlayType.POLYGON) {

        wkt = "POLYGON(";
        wkt += "(";
        paths = shape.getPath();

        paths.forEach((latlng) => {
            wkt += latlng.lng() + " "
            wkt += latlng.lat() + " , "
        });
        wkt += paths.getAt(0).lng() + " " + paths.getAt(0).lat() + "),";
        wkt = wkt.substring(0, wkt.length - 1) + ")";
    }
    else if (shape.type === google.maps.drawing.OverlayType.MARKER) {
        wkt = "POINT(";
        var latlng = shape.getPosition();

        wkt += latlng.lng()
        wkt += " "
        wkt += latlng.lat()
        wkt += ")";
    }

    return wkt
}

function getWktFromShapes() {
    var wkt = "";
    var allShapesArePolygons = true;
    if (allShapes.length > 1) {
        //check that allshapes only contains polygons
        allShapes.forEach((shape) => {
            if (shape.type === "marker") {
                allShapesArePolygons = false;
            }
        })
    }
    if (allShapes.length > 1 && allShapesArePolygons) { //if more than 1 shape make multipolygon wkt
        wkt = "MULTIPOLYGON(("
        allShapes.forEach((shape) => {
            wkt += "("
            paths = shape.getPath();

            paths.forEach((latlng) => {
                wkt += latlng.lng() + " "
                wkt += latlng.lat() + " , "
            });
            wkt += paths.getAt(0).lng() + " " + paths.getAt(0).lat() + "),";

        });
        wkt = wkt.substring(0, wkt.length - 1) + "))";
    } else if (allShapes.length === 1) { //make single marker or polygon wkt
        var shape = allShapes[0];
        wkt = getWktFromShape(shape);
    } else if (allShapes.length === 0) {
        throw new UserException('There are no shapes, please draw a shape or place a marker');
    }
    else {
        throw new UserException('Invalid shapes, please use only one marker or draw polygons without markers');
    }

    return wkt;
}

//this function requires openlayers (ol.js)
function getGeoJsonFromWkt(wkt) {
    //var val = 'GEOMETRYCOLLECTION(MULTIPOLYGON(((-0.12072212703174 51.51899157882951,-0.128597092699465 51.51191439062526,-0.129004788469729 51.51260880491084,-0.129584145616946 51.51374388239237,-0.130120587419924 51.51494569831,-0.130614113878664 51.51653471734371,-0.125507187914309 51.51718900318654,-0.121001076769289 51.519178508517115,-0.12072212703174 51.51899157882951))))';
    var geojson_options = {};
    var wkt_format = new ol.format.WKT();
    var feature = wkt_format.readFeature(wkt);
    var wkt_options = {};
    var geojson_format = new ol.format.GeoJSON(wkt_options);
    return geojson_format.writeFeatureObject(feature);
    //this function requires terraformer-wkt-parser.js
    //return Terraformer.WKT.parse(wkt);
}


function showInfo(event) {
    var windowPos;
    var contentString;
    if (selectedShape.type === "polygon") {
        windowPos = event.latLng;
        contentString = '<b>Shape: ' + shapeId + '</b>' + '<br>' +
            'Clicked location: <br>' + event.latLng.lat() + ',' + event.latLng.lng() +
            '<br>';

        var vertices = selectedShape.getPath();
        // Iterate over the vertices.
        for (var i = 0; i < vertices.getLength() ; i++) {
            var xy = vertices.getAt(i);
            contentString += '<br>' + 'Coordinate ' + i + ':<br>' + xy.lat() + ',' +
                xy.lng();
        }
    } else if (selectedShape.type === "marker") {
        windowPos = selectedShape.getPosition();
        contentString = '<b>Marker: ' + shapeId + '</b>' + '<br>' +
        'Marker location: <br>' + windowPos.lat() + ',' + windowPos.lng() +
        '<br>';
    }

    infoWindow = new google.maps.InfoWindow;
    // Replace the info window's content and position.
    infoWindow.setContent(contentString);
    infoWindow.setPosition(windowPos);

    infoWindow.open(map);
}

function removeShapeClickListenerFromShapes() {
    shapeEventListeners.forEach((eventListener) => {
        google.maps.event.removeListener(eventListener);
    });
    shapeEventListeners = [];
}

function setArea() {
    try {
        //convert shapes to wkt
        var wkt = getWktFromShapes();
        //set wkt textbox and grey out
        var searchAreaWkt = document.getElementById("SearchAreaWkt");
        searchAreaWkt.value = wkt;
        searchAreaWkt.setAttribute("readonly", true);
        //get json text box
        var searchAreaJson = document.getElementById("SearchAreaJson");
        var geoJson = getGeoJsonFromWkt(wkt);
        //var gMapGeoJson = map.data.toGeoJson();
        //var gMapGeoJsonStr = JSON.stringify(gMapGeoJson);
        //alert(gMapGeoJsonStr);
        searchAreaJson.value = JSON.stringify(geoJson);
        //remove drawing from map and event listeners from shapes
        disableDrawing();
        removeShapeClickListenerFromShapes();
    } catch (e) {
        alert(e.message, e.name);
    }
}

function setAreaButtonClick() {
    setArea();
}
function ClearSelectedShapeButtonClick() {
    deleteSelectedShape();
}
function resetMapAndSpatialText() {
    allShapes.splice(0, allShapes.length)
    initMap();
    var searchAreaWkt = document.getElementById("SearchAreaWkt");
    searchAreaWkt.value = "";
    searchAreaWkt.removeAttribute("readonly");
    var searchAreaJson = document.getElementById("SearchAreaJson");
    searchAreaJson.value = "";
}
function ResetMapAndWktClick() {
    resetMapAndSpatialText();
}

function applyBufferToFeatures(buffer) {
    var parser = new jsts.io.OL3Parser();
    var wktReader = new jsts.io.WKTReader()
    var searchAreaWkt = document.getElementById("SearchAreaWkt");
    var jstsGeom = wktReader.read(searchAreaWkt.value);
    var buffered = jstsGeom.buffer(buffer);
    var coords = buffered.getCoordinates();
    var envelope = buffered.getEnvelope();
    var googleCoords = [];
    coords.forEach(coord => {
        console.log('>> ', 'coord is: ', coord)
        var myLatlng = new google.maps.LatLng(coord.y, coord.x);
        googleCoords.push(myLatlng);
    });

    // Construct the polygon.
    var bufferedPolygon = new google.maps.Polygon({
        paths: googleCoords,
        strokeColor: '#FF0000',
        strokeOpacity: 0.8,
        strokeWeight: 2,
        fillColor: '#FF0000',
        fillOpacity: 0.35
    });
    bufferedPolygon.setMap(map);

    //not working for some reason:
    //map.data.forEach(feature => {

    //    //console.log('>> ', 'properties are: ');
    //    //feature.forEachProperty(function (value, property) {
    //    //    console.log(property, ':', value);
    //    //});
    //    var jstsGeom = parser.read(feature.getGeometry());
    //    var area = computeArea(jstsGeom)
    //    var buffered = jstsGeom.buffer(buffer);

    //});
    return buffered;
}

function ApplyBufferToShapesButtonClick() {
    try {
        var buffer = document.getElementById("Buffer").value;
        setArea();
        var buffered = applyBufferToFeatures(buffer);

    } catch (e) {
        alert(e.message, e.name);
    }
}



function initMap() {
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: 53.24150676822664, lng: -2.5523899555555545 },
        zoom: 9
    });

    drawingManager = getDrawingManager();
    drawingManager.setMap(map);

    var eventListener = google.maps.event.addListener(drawingManager, 'overlaycomplete', function shapeComplete(e) {
        // Switch back to non-drawing mode after drawing a shape.
        drawingManager.setDrawingMode(null);
        if (e.type === "polygon") {
            addNewShape(e);
        } else if (e.type === "marker") {
            addNewMarker(e);
        }
    });
}

function initMapReadOnly() {
    var wktDiv = document.getElementById("wkt");
    var innerText = wktDiv.innerText;
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: 53.24150676822664, lng: -2.5523899555555545 },
        zoom: 9
    });
    try {
        var geoJson = getGeoJsonFromWkt(innerText);
        var features = map.data.addGeoJson(geoJson);

        //map.getBoundsZoomLevel(shape.getBounds());
        //map.setZoom();
        var bounds = new google.maps.LatLngBounds();
        features.forEach(feature => {
            var geom = feature.getGeometry();
            geom.forEachLatLng(latlong => {
                bounds.extend(latlong);
            })

        })

        map.fitBounds(bounds);
    }
    catch (e) {
        alert(e.message, e.name);
    }
}


