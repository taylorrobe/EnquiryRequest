//get references from DOM
var searchAreaWkt = document.getElementById("SearchAreaWkt");
var searchAreaJson = document.getElementById("SearchAreaJson");
var typeSelect = document.getElementById('type');
var map;
var select = null;  // ref to currently selected interaction
var selectClick;
var savedFeatures = [];

proj4.defs('EPSG:27700', '+proj=tmerc +lat_0=49 +lon_0=-2 +k=0.9996012717 ' +
    '+x_0=400000 +y_0=-100000 +ellps=airy ' +
    '+towgs84=446.448,-125.157,542.06,0.15,0.247,0.842,-20.489 ' +
    '+units=m +no_defs');
var proj27700 = ol.proj.get('EPSG:27700');
proj27700.setExtent([0, 0, 700000, 1300000]);


function UserException(message) {
    this.message = message;
    this.name = 'UserException';
}



function getLayerById(id) {
    var layer;
    var layers = map.getLayers();
    layers.forEach(function (lyr) {
        if (id === lyr.get('id')) {
            layer = lyr;
        }
    });
    return layer;
}

function getLayerSource(layerName) {
    var layer = getLayerById(layerName);
    var source = layer.getSource();
    return source;
}

function getFeaturesFromLayer(layerName) {
    var source = getLayerSource(layerName)
    var features = source.getFeatures();
    return features;
}

function selectShape() {
    // select interaction working on "click"
    selectClick = new ol.interaction.Select({
        condition: ol.events.condition.click,
        layers: function (layer) {
            return layer.get('id') == 'drawingVector';
        }
    });

    var modify = new ol.interaction.Modify({ features: selectClick.getFeatures() });
    map.addInteraction(modify);
    select = selectClick;
}

function changeInteraction() {


    if (select !== null) {
        map.removeInteraction(select);
    }

    var value = typeSelect.value;

    if (value == 'Select') {
        selectShape();
    } else {
        select = null;
    }

    if (select !== null) {
        map.addInteraction(select);
    }
}

function initialiseMap() {
    var draw, snap;
    map = null;

    function drawStart(e) {
        clearSearchAreaTextBoxes();
    }

    function drawEnd(e) {

    }

    function addInteractions() {
        if (typeSelect.value !== "Select") {

            draw = new ol.interaction.Draw({
                source: source,
                type: typeSelect.value
            });

            draw.on('drawstart', function drawStart(e) { clearSearchAreaTextBoxes(); });

            draw.on('drawend', function drawEnd(e) {
                //map.removeInteraction(draw);
                //map.removeInteraction(snap);
            });

            map.addInteraction(draw);
            //todo select newly added shape

            snap = new ol.interaction.Snap({ source: source });
            map.addInteraction(snap);
        }

    }

    var raster = new ol.layer.Tile({
        source: new ol.source.OSM()
    });

    var source = new ol.source.Vector({ wrapX: false });
    var sourceBoundary = new ol.source.Vector({ wrapX: false });

    var vectorBoundaries = new ol.layer.Vector({
        id: "boundaries",
        source: sourceBoundary
    });

    //variable gets data from the ViewBag.boundaries
    var boundariesFromHtmlData = document.getElementById("Boundaries").getAttribute('data-geoJson');
    var format = new ol.format.GeoJSON();
    var boundaries = format.readFeatures(boundariesFromHtmlData);
    sourceBoundary.addFeatures(boundaries);
    //boundaries.forEach(boundary => {

    //    sourceBoundary.addFeature(boundary);
    //})

    

    var vector = new ol.layer.Vector({
        id: "drawingVector",
        source: source
    });
    var defaultView = new ol.View({
        projection: 'EPSG:27700',
        center: [362000, 369000],
        zoom: 5
    })
    map = new ol.Map({
        layers: [raster, vectorBoundaries, vector],
        target: 'map',
        view: defaultView
    });

    /**
     * Handle change event.
     */
    typeSelect.onchange = function () {
        map.removeInteraction(draw);
        map.removeInteraction(snap);
        addInteractions();
        changeInteraction();
    };

    addInteractions();

    /**
     * onchange callback on the select element.
     */
    changeInteraction();
}

function extendToDrawing() {
    try {

        //Create an empty extent that we will gradually extend
        var extent = ol.extent.createEmpty();

        var source = getLayerSource("drawingVector");
        ol.extent.extend(extent, source.getExtent());

        //Finally fit the map's view to our combined extent
        map.getView().fit(extent, map.getSize());

    } catch (e) {
        alert(e.message, e.name);
    }
}

function setArea() {
    try {
        extendToDrawing();

        //convert features to wkt and geoJson
        var wktFormat = new ol.format.WKT();
        var geoJsonFormat = new ol.format.GeoJSON();

        var features = getFeaturesFromLayer("drawingVector");
        if (features) {
            var wkt = wktFormat.writeFeatures(features);
            var geoJson = geoJsonFormat.writeFeatures(features);
            searchAreaWkt.value = wkt;
            searchAreaJson.value = geoJson;
        }

    } catch (e) {
        alert(e.message, e.name);
    }
}

function ClearSelectedShape() {
    var source = getLayerSource("drawingVector");
    if (select !== null) {
        var confirmPolygon = function () { return confirm("Do you want to delete this shape?") };

        if (confirmPolygon()) {
            var features = select.getFeatures();
            features.forEach(feature => {
                source.removeFeature(feature);
                map.removeInteraction(select);
            });

        }
    }
    else {
        alert("no shape selected, please change 'Geometry type' dropdown to 'Select', then select a shape to delete")
    }
    changeInteraction();
    clearSearchAreaTextBoxes();
}

function clearSearchAreaTextBoxes() {
    //get text box references from DOM
    searchAreaWkt.value = "";
    searchAreaJson.value = "";
}

function ResetMapAndWkt() {

        //clear all drawings
        var source = getLayerSource("drawingVector");
        source.clear();
        //recentre map
        var defaultView = new ol.View({
            projection: 'EPSG:27700',
            center: [362000, 369000],
            zoom: 5
        })
        map.setView(defaultView);
        clearSearchAreaTextBoxes();
    
}

function unionFeatures() {
    var parser = new jsts.io.OL3Parser();
    var source = getLayerSource("drawingVector");
    var features = getFeaturesFromLayer("drawingVector");
    var unionOfFeatures = new ol.Feature();
    var jstsGeomOfUnionOfFeatures;
    var i = 0;
    features.forEach(feature => {
        if (i < 1) {
            //for first iteration set up unionGeom
            jstsGeomOfUnionOfFeatures = parser.read(feature.getGeometry());
        } else {
            //for next iterations merge to uniongeom
            var jstsGeomOfFeature = parser.read(feature.getGeometry());
            //union current geometry with single geometry
            jstsGeomOfUnionOfFeatures = jstsGeomOfFeature.union(jstsGeomOfUnionOfFeatures);
        }
        i++;
    })
    //parse back to ol.feature
    unionOfFeatures.setGeometry(parser.write(jstsGeomOfUnionOfFeatures));
    source.clear();
    source.addFeature(unionOfFeatures);
    clearSearchAreaTextBoxes();
}

function applyBufferToFeature(feature, distance) {
    var parser = new jsts.io.OL3Parser();

    // convert the OpenLayers geometry to a JSTS geometry
    var jstsGeom = parser.read(feature.getGeometry());

    // create a buffer around the feature
    var buffered = jstsGeom.buffer(distance);

    return buffered;
}

function applyBufferToShapes() {
    var distance = document.getElementById("Buffer").value;
    var parser = new jsts.io.OL3Parser();
    var features = getFeaturesFromLayer("drawingVector");

    //save copy of original features for undoApplyBufferToShapes function
    var clonedFeatures = [];
    features.forEach(feature => {
        var clonedFeature = feature.clone();
        clonedFeatures.push(clonedFeature);
    })
    savedFeatures.push(clonedFeatures);

    //apply buffer to all features and add back to the map
    features.forEach(feature => {
        var buffered = applyBufferToFeature(feature, distance);
            // convert back from JSTS and replace the geometry on the feature
            feature.setGeometry(parser.write(buffered));
    })
    if (select !== null) {
        map.removeInteraction(select);
    }
    extendToDrawing();
    clearSearchAreaTextBoxes();
}

function undoApplyBufferToShapes() {
    if (savedFeatures.length > 0) {
        //ResetMapAndWkt();
        var features = savedFeatures.pop();
        var source = getLayerSource("drawingVector");
        source.clear();
        features.forEach(feature => {
            source.addFeature(feature);
        })
        if (select !== null) {
            map.removeInteraction(select);
        }
        extendToDrawing();
        clearSearchAreaTextBoxes();
    }

}

function unionFeaturesButtonClick() {
    unionFeatures();
}

function setAreaButtonClick() {
    setArea();
}

function ClearSelectedShapeButtonClick() {
    ClearSelectedShape();
}

function ResetMapAndWktClick() {
    var confirmReset = function () { return confirm("Do you want to reset the map? this will clear all drawings.") };

    if (confirmReset()) {
        ResetMapAndWkt();
    }
}

function ApplyBufferToShapesButtonClick() {
    applyBufferToShapes();
}

function undoApplyBufferToShapesButtonClick() {
    undoApplyBufferToShapes();
}


initialiseMap();