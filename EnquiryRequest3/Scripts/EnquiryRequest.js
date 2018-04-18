
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
    if (index != -1) {
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

function applyBuffer(shape, buffer) {
    if (shape == null) {
        throw new UserException("no shape drawn");
    }
    if (TryParseInt(buffer, 0) <= 0) {
        throw new UserException("no buffer specified");
    }
    if (shape.type == "polygon") {
        var padding = parseInt(buffer);
        var vertices = shape.getPath();
        var polybounds = new google.maps.LatLngBounds();
        for (var i = 0; i < vertices.getLength(); i++) {
            polybounds.extend(vertices.getAt(i));
        }
        var center = polybounds.getCenter();
        var centerMarker = new google.maps.Marker({
            position: center,
            visible : false,
            map: map,
            icon: {
                size: new google.maps.Size(7, 7),
                anchor: new google.maps.Point(4, 4)
            }
        });

        var polylines = [];
        var newPath = [];
        for (var i = 0; i < vertices.getLength(); i++) {
            polylines.push(new google.maps.Polyline({
                path: [center, vertices.getAt(i)],
                visible: false,
                map: map,
                strokeWidth: 2,
                strokeColor: 'red'
            }));
            newPath[i] = google.maps.geometry.spherical.computeOffset(center,
                padding + google.maps.geometry.spherical.computeDistanceBetween(center, vertices.getAt(i)),
                google.maps.geometry.spherical.computeHeading(center, vertices.getAt(i)));
        }
        // render outer shape
        var outer = new google.maps.Polygon({
            strokeColor: 'white',
            strokeOpacity: 0.8,
            strokeWeight: 1,
            fillColor: 'black',
            fillOpacity: 0.35,
            map: map,
            editable: false,
            path: newPath
        });
        var overlay = {
            overlay: outer,
            type: google.maps.drawing.OverlayType.POLYGON
        };
        return overlay;
    } else if (shape.type === "marker") {
        latlng = shape.getPosition();
        circle = new google.maps.Circle({
            center: latlng,
            map: map,
            strokeColor: '#000',
            strokeWeight: 2,
            strokeOpacity: 0.5,
            fillColor: '#f0f0f0',
            fillOpacity: 0.5,
            radius: parseInt(buffer)
        });
        return circle;
    }
};

function disableDrawing() {
    drawingManager.setDrawingMode(null);
    drawingManager.setOptions({
        drawingControl: false
    });
    clearSelection();
}

function getWktFromShape(shape) {
    var wkt = "";
    if (shape.type == google.maps.drawing.OverlayType.POLYGON) {

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
    else if (shape.type == google.maps.drawing.OverlayType.MARKER) {
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
            if (shape.type == "marker") {
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
    var geoJson = getGeoJsonFromWkt(wkt);
    alert(JSON.stringify(geoJson), "geoJSON");
    return wkt;
}

//this function requires terraformer-wkt-parser.js
function getGeoJsonFromWkt(wkt) {
    return Terraformer.WKT.parse(wkt);
}

/** @this {google.maps.Polygon} */
function showInfo(event) {
    var windowPos;
    if (selectedShape.type === "polygon") {
        windowPos = event.latLng;
        var contentString = '<b>Shape: ' + shapeId + '</b>' + '<br>' +
            'Clicked location: <br>' + event.latLng.lat() + ',' + event.latLng.lng() +
            '<br>';

        var vertices = selectedShape.getPath();
        // Iterate over the vertices.
        for (var i = 0; i < vertices.getLength(); i++) {
            var xy = vertices.getAt(i);
            contentString += '<br>' + 'Coordinate ' + i + ':<br>' + xy.lat() + ',' +
                xy.lng();
        }
    } else if (selectedShape.type === "marker") {
        windowPos = selectedShape.getPosition();
        var contentString = '<b>Marker: ' + shapeId + '</b>' + '<br>' +
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

function setAreaButtonClick() {
    //do something
    try {
        var wkt = getWktFromShapes();
        document.getElementById("SearchAreaWkt").value = wkt;
        disableDrawing();
        removeShapeClickListenerFromShapes();
    } catch (e) {
        alert(e.message, e.name);
    }
}
function ClearSelectedShapeButtonClick() {
    deleteSelectedShape();
}
function ResetMapAndWktClick() {
    //todo
    allShapes.splice(0, allShapes.length)
    initMap();
    document.getElementById("SearchAreaWkt").value = "";
}
function ApplyBufferToSelectedButtonClick() {
    try {
        var buffer = document.getElementById("Buffer").value;
        var newShape = applyBuffer(selectedShape, buffer);
        addNewShape(newShape);
    } catch (e) {
        alert(e.message, e.name);
    }
}
// This example requires the Drawing library. Include the libraries=drawing
// parameter when you first load the API. For example:
// <script src="https://maps.googleapis.com/maps/api/js?key=YOUR_API_KEY&libraries=drawing">



function initMap() {
    //var isPostBack = document.getElementById("validationRepost").value;
    //if (isPostBack === "false") {
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
        
    //}
}

function initMapReadOnly(wkt) {
    var googleObj = getGeoJson(wkt, opts);
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: 53.24150676822664, lng: -2.5523899555555545 },
        zoom: 9
    });

}


