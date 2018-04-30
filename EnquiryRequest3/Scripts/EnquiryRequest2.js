//get references from DOM
var searchAreaWkt = document.getElementById("SearchAreaWkt");
var searchAreaJson = document.getElementById("SearchAreaJson");
var areaText = document.getElementById("Area");
var costText = document.getElementById("Cost");

var typeSelect = document.getElementById('type');
//file accessible references
var map;
// refs to currently selected interaction
var select = null;
var selectClick;
//array to hold saved features for undo action
var savedFeatures = [];

//create projection for map
proj4.defs('EPSG:27700', '+proj=tmerc +lat_0=49 +lon_0=-2 +k=0.9996012717 ' +
    '+x_0=400000 +y_0=-100000 +ellps=airy ' +
    '+towgs84=446.448,-125.157,542.06,0.15,0.247,0.842,-20.489 ' +
    '+units=m +no_defs');
var proj27700 = ol.proj.get('EPSG:27700');
proj27700.setExtent([0, 0, 700000, 1300000]);

//set boundary style
var boundaryStyle = new ol.style.Style({
    fill: new ol.style.Fill({
        color: 'rgba(255, 255, 255, 0.6)'
    }),
    stroke: new ol.style.Stroke({
        color: '#319FD3',
        width: 1
    }),
    text: new ol.style.Text({
        font: '12px Calibri,sans-serif',
        fill: new ol.style.Fill({
            color: '#000'
        }),
        stroke: new ol.style.Stroke({
            color: '#fff',
            width: 3
        })
    })
});

//set coverage style
var coverageStyle = new ol.style.Style({
    fill: new ol.style.Fill({
        color: 'rgba(255, 0, 0, 0.05)'
    }),
    stroke: new ol.style.Stroke({
        color: '#319FD3',
        width: 1
    }),
    text: new ol.style.Text({
        font: '12px Calibri,sans-serif',
        fill: new ol.style.Fill({
            color: '#000'
        }),
        stroke: new ol.style.Stroke({
            color: '#fff',
            width: 3
        })
    })
});

//helper to display exceptions
function UserException(message) {
    this.message = message;
    this.name = 'UserException';
}

//return layer by id helper
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

//return openLayer source
function getLayerSource(layerName) {
    var layer = getLayerById(layerName);
    var source = layer.getSource();
    return source;
}

//return collection of features from layer
function getFeaturesFromLayer(layerName) {
    var source = getLayerSource(layerName)
    var features = source.getFeatures();
    return features;
}

//set feature as selected with modify function to allow repositioning of nodes
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

//sets the correct interaction dependant on drop down select
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

//helper to set map and text boxes from wkt, for use in edit enquiry
function setMapAndTextFromWkt(wkt, source) {
    //get GeoJson from wkt
    var wktFormat = new ol.format.WKT();
    var geom = wktFormat.readFeature(wkt);
    var geoJsonFormat = new ol.format.GeoJSON();
    var geoJson = geoJsonFormat.writeFeature(geom);
    //set geoJson text on page
    searchAreaJson.value = geoJson;
    //get features from GeoJson
    var features = geoJsonFormat.readFeatures(geoJson);
    source.addFeatures(features);
    populateTextBoxes();
}

//init map
function initialiseMap() {
    var draw, snap;
    map = null;

    function addInteractions() {
        if (typeSelect.value !== "Select") {

            draw = new ol.interaction.Draw({
                source: source,
                type: typeSelect.value
            });
            //event listener to clear the area text boxes when new drawing started
            draw.on('drawstart', function drawStart(e) { clearSearchAreaTextBoxes(); });

            //event listener (empty) for map drawing end
            draw.on('drawend', function drawEnd(e) {

            });

            map.addInteraction(draw);
            //TODO auto select newly added shape

            snap = new ol.interaction.Snap({ source: source });
            map.addInteraction(snap);
        }

    }

    //set up map layers
    var raster = new ol.layer.Tile({
        source: new ol.source.OSM()
    });

    //layer for boundaries
    var boundarySource = new ol.source.Vector({ wrapX: false });
    var boundaryVector = new ol.layer.Vector({
        id: "boundaries",
        source: boundarySource,
        style: boundaryStyle
    });

    //variable gets data from the ViewBag.boundaries to add boundaries to layer
    var boundariesFromHtmlData = document.getElementById("Boundaries").getAttribute('data-geoJson');
    var format = new ol.format.GeoJSON();
    var boundaries = format.readFeatures(boundariesFromHtmlData);
    boundarySource.addFeatures(boundaries);

    //layer for coverage area
    var coverageSource = new ol.source.Vector({ wrapX: false });
    var coverageVector = new ol.layer.Vector({
        id: "coverage",
        source: coverageSource,
        style: coverageStyle
    });

    //variable gets data from the ViewBag.boundaries to add boundaries to coverage layer
    var coverageFromHtmlData = document.getElementById("Boundaries").getAttribute('data-coverageGeoJson');
    var format = new ol.format.GeoJSON();
    var coverageBoundaries = format.readFeatures(coverageFromHtmlData);
    coverageSource.addFeatures(coverageBoundaries);

    //layer to display user map interactions / drawing
    var source = new ol.source.Vector({ wrapX: false });
    var vector = new ol.layer.Vector({
        id: "drawingVector",
        source: source
    });

    //set the projection and centre / zoom map
    var defaultView = new ol.View({
        projection: 'EPSG:27700',
        center: [362000, 369000],
        zoom: 5
    })

    //create map with new layers
    map = new ol.Map({
        layers: [raster, coverageVector, boundaryVector, vector],
        target: 'map',
        view: defaultView
    });

    if (typeSelect !=null) {
        //Handle change event for type select
        typeSelect.onchange = function () {
            map.removeInteraction(draw);
            map.removeInteraction(snap);
            addInteractions();
            changeInteraction();
        };
        addInteractions();
        changeInteraction();
    }

    //if wkt is not "" (edit mode), set json and map up search area
    var wkt = searchAreaWkt.value;
    if (wkt != "") {
        setMapAndTextFromWkt(wkt, source);
    }

}

//get intersection with coverage area
function removeOutsideCoverageArea() {
    //merge features
    unionFeaturesInLayer("drawingVector");
    unionFeaturesInLayer("coverage");
    //create jsts parser
    var parser = new jsts.io.OL3Parser();
    var source = getLayerSource("drawingVector");
    var features = getFeaturesFromLayer("drawingVector");
    var coverageFeatures = getFeaturesFromLayer("coverage");
    var newFeature = new ol.Feature();

    //only one in features as have been merged
    feature = features[0];
    coverageFeature = coverageFeatures[0];
    var jstsGeomOfCoverage = parser.read(coverageFeature.getGeometry());
    var jstsGeomOfFeature = parser.read(feature.getGeometry());
    removedOutsideFeature = jstsGeomOfFeature.intersection(jstsGeomOfCoverage);

    //parse back to ol.feature
    newFeature.setGeometry(parser.write(removedOutsideFeature));
    //clear original features from map
    source.clear();
    source.addFeature(newFeature);
    clearSearchAreaTextBoxes();
}

//enlarge view window to see all drawings
function extendToDrawing() {
    try {

        //Create an empty extent that will gradually extend to all drawings
        var extent = ol.extent.createEmpty();

        var source = getLayerSource("drawingVector");
        ol.extent.extend(extent, source.getExtent());

        //Finally fit the map's view to our combined extent
        map.getView().fit(extent, map.getSize());

    } catch (e) {
        alert(e.message, e.name);
    }
}

function getCostFromArea(area) {
    var cost = ((area - 314) / 12.57) + 150;
    var roundedCost = Math.round(cost * 1) / 1;
    return roundedCost;
}

function populateTextBoxes() {
    //open layers helpers to convert features to wkt and geoJson
    var wktFormat = new ol.format.WKT();
    var geoJsonFormat = new ol.format.GeoJSON();

    var features = getFeaturesFromLayer("drawingVector");
    if (features) {
        //convert to text / JSON and set text areas
        var wkt = wktFormat.writeFeatures(features);
        var geoJson = geoJsonFormat.writeFeatures(features);
        searchAreaWkt.value = wkt;
        searchAreaJson.value = geoJson;
        //get area and display cost
        var parser = new jsts.io.OL3Parser();
        var feature = features[0];
        var geom = parser.read(feature.getGeometry());
        var area = geom.getArea() / 10000;
        var roundedArea = Math.round(area * 1000) / 1000;
        areaText.value = roundedArea;
        costText.value = getCostFromArea(roundedArea);
    }
}

function setArea() {
    try {
        unionFeaturesInLayer("drawingVector");
        removeOutsideCoverageArea();
        extendToDrawing();
        populateTextBoxes();

    } catch (e) {
        alert(e.message, e.name);
    }
}

//clear current selected feature
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
    //clear text as map has changed
    clearSearchAreaTextBoxes();
}

//clear text representation of drawn map features
function clearSearchAreaTextBoxes() {
    searchAreaWkt.value = "";
    searchAreaJson.value = "";
    areaText.value = "";
    costText.value = "";
}

//clear all drawings and reset map
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

//help to merge all user drawings to one
function unionFeaturesInLayer(layerName) {
    //create jsts parser
    var parser = new jsts.io.OL3Parser();
    var source = getLayerSource(layerName);
    var features = getFeaturesFromLayer(layerName);
    var unionOfFeatures = new ol.Feature();
    //variable to hold merged features as one feature
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
    //clear original features from map
    source.clear();
    source.addFeature(unionOfFeatures);
    clearSearchAreaTextBoxes();
}

//apply distance buffer to individual feature
function applyBufferToFeature(feature, distance) {
    //use jsts parser
    var parser = new jsts.io.OL3Parser();

    // convert the OpenLayers geometry to a JSTS geometry
    var jstsGeom = parser.read(feature.getGeometry());

    // create a buffer around the feature
    var buffered = jstsGeom.buffer(distance);

    return buffered;
}

//apply buffer to all features
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

    //unselect selected feature if required
    if (select !== null) {
        map.removeInteraction(select);
    }

    //zoom to extent and clear text
    extendToDrawing();
    clearSearchAreaTextBoxes();
}

//undo buffer function
function undoApplyBufferToShapes() {
    if (savedFeatures.length > 0) {
        //get newly added shape
        var features = savedFeatures.pop();
        var source = getLayerSource("drawingVector");
        //clear current buffer
        source.clear();
        //add last saved features to map
        features.forEach(feature => {
            source.addFeature(feature);
        })
        //unselect
        if (select !== null) {
            map.removeInteraction(select);
        }
        //zoom and clear text
        extendToDrawing();
        clearSearchAreaTextBoxes();
    }

}

//click events
function unionFeaturesButtonClick() {
    unionFeaturesInLayer("drawingVector");
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