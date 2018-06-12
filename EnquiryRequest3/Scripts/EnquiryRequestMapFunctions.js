//get references from DOM
var searchAreaWkt = document.getElementById("SearchAreaWkt");
var searchAreaJson = document.getElementById("SearchAreaJson");
var areaText = document.getElementById("Area");
var costText = document.getElementById("Cost");

var typeSelect = document.getElementById('type');
//file accessible references
//var geocoder;
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
        color: 'rgba(255, 255, 255, 0)'
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
        color: 'red',
        width: 5
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

//set grid ref style
var gridRefStyle = new ol.style.Style({
    fill: new ol.style.Fill({
        color: 'rgba(255, 0, 0, 0)'
    }),
    stroke: new ol.style.Stroke({
        color: 'red',
        width: 5
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
            return layer.get('id') === 'drawingVector';
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

    if (value === 'Select') {
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
    ExtendToLayerFeatures(source);
    populateTextBoxes();
}

//text options for boundary layer labels
var boundaryVectorTextOptions = {
    polygons: {
        text: 'normal',
        align: 'center',
        baseline: 'middle',
        rotation: '0',
        font: 'Arial',
        weight: 'bold',
        placement: 'point',
        maxangle: '0.7853981633974483',
        size: '10px',
        offsetX: '0',
        offsetY: '0',
        color: 'green',
        outline: '#ffffff',
        outlineWidth: '3',
        overflow: 'false',
        maxreso: '1200'

    }
};

//text options for label layer labels
var labelVectorTextOptions = {
    polygons: {
        text: 'wrap',
        align: 'center',
        baseline: 'middle',
        rotation: '0',
        font: 'Arial',
        weight: 'bold',
        placement: 'point',
        maxangle: '0.7853981633974483',
        size: '10px',
        offsetX: '0',
        offsetY: '20',
        color: 'blue',
        outline: '#ffffff',
        outlineWidth: '3',
        overflow: 'true',
        maxreso: '800'

    }
};

//text options for gridRef layer labels
var gridRefVectorTextOptions = {
    polygons: {
        text: 'normal',
        align: 'center',
        baseline: 'top',
        rotation: '0',
        font: 'Arial',
        weight: 'bold',
        placement: 'point',
        maxangle: '360',
        size: '10px',
        offsetX: '0',
        offsetY: '0',
        color: 'red',
        outline: '#ffffff',
        outlineWidth: '3',
        overflow: 'true',
        maxreso: '38400'

    }
};

//get text function for boundary layers
var getText = function (feature, resolution, dom) {
    var type = dom.text.value;
    var maxResolution = dom.maxreso.value;
    var text = feature.get('Name');

    if (resolution > maxResolution) {
        text = '';
    } else if (type === 'hide') {
        text = '';
    } else if (type === 'shorten') {
        text = text.trunc(12);
    } else if (type === 'wrap' && dom.placement.value !== 'line') {
        text = stringDivider(text, 16, '\n');
    }

    return text;
};

// boundary style using text
function boundaryStyleFunction(feature, resolution) {
    return new ol.style.Style({
        stroke: new ol.style.Stroke({
            color: 'green',
            width: 1
        }),
        fill: new ol.style.Fill({
            color: 'rgba(0, 255, 0, 0.01)'
        }),
        text: createTextStyle(feature, resolution, boundaryVectorTextOptions.polygons)
    });
}

// label style using text
function labelStyleFunction(feature, resolution) {
    return new ol.style.Style({
        stroke: new ol.style.Stroke({
            color: 'blue',
            width: 0
        }),
        fill: new ol.style.Fill({
            color: 'rgba(0, 0, 255, 0)'
        }),
        text: createTextStyle(feature, resolution, labelVectorTextOptions.polygons)
    });
}

// gridref style using text
function gridRefStyleFunction(feature, resolution) {
    return new ol.style.Style({
        stroke: new ol.style.Stroke({
            color: 'red',
            width: 0
        }),
        fill: new ol.style.Fill({
            color: 'rgba(0, 0, 255, 0)'
        }),
        text: createTextStyle(feature, resolution, gridRefVectorTextOptions.polygons)
    });
}

var createTextStyle = function (feature, resolution, dom) {
    var align = dom.align;
    var baseline = dom.baseline;
    var size = dom.size;
    var offsetX = parseInt(dom.offsetX, 10);
    var offsetY = parseInt(dom.offsetY, 10);
    var weight = dom.weight;
    var placement = dom.placement ? dom.placement : undefined;
    var maxAngle = dom.maxangle ? parseFloat(dom.maxangle) : undefined;
    var overflow = dom.overflow ? (dom.overflow === 'true') : undefined;
    var rotation = parseFloat(dom.rotation);
    if (dom.font === '\'Open Sans\'' && !openSansAdded) {
        var openSans = document.createElement('link');
        openSans.href = 'https://fonts.googleapis.com/css?family=Open+Sans';
        openSans.rel = 'stylesheet';
        document.getElementsByTagName('head')[0].appendChild(openSans);
        openSansAdded = true;
    }
    var font = weight + ' ' + size + ' ' + dom.font;
    var fillColor = dom.color;
    var outlineColor = dom.outline;
    var outlineWidth = parseInt(dom.outlineWidth, 10);

    return new ol.style.Text({
        textAlign: align === '' ? undefined : align,
        textBaseline: baseline,
        font: font,
        text: getText(feature, resolution, dom),
        fill: new ol.style.Fill({ color: fillColor }),
        stroke: new ol.style.Stroke({ color: outlineColor, width: outlineWidth }),
        offsetX: offsetX,
        offsetY: offsetY,
        placement: placement,
        maxAngle: maxAngle,
        overflow: overflow,
        rotation: rotation
    });
};

//init map
function initialiseMap() {
    var draw, snap;
    map = null;
    //geocoder = new google.maps.Geocoder();

    ////bing maps stuff
    //var styles = [
    //    'Road',
    //    'RoadOnDemand',
    //    'Aerial',
    //    'AerialWithLabels',
    //    'collinsBart',
    //    'ordnanceSurvey'
    //];
    //var layers = [];
    //var i, ii;
    //for (i = 0, ii = styles.length; i < ii; ++i) {
    //    layers.push(new ol.layer.Tile({
    //        visible: false,
    //        preload: Infinity,
    //        source: new ol.source.BingMaps({
    //            key: 'Aj4s3uxt7sMUlytcU9MPkK7OO5JXQvMV-Y0iF6F0tPhYaFevU7hVq_-v6pjs0Aog',
    //            imagerySet: styles[i],
    //            maxZoom: 19
    //        })
    //    }));
    //}
    function addInteractions() {
        if (typeSelect.value !== "Select") {

            draw = new ol.interaction.Draw({
                source: drawingSource,
                type: typeSelect.value
            });
            //event listener to clear the area text boxes when new drawing started
            draw.on('drawstart', function drawStart(e) { });

            //event listener (empty) for map drawing end
            draw.on('drawend', function drawEnd(e) {
                clearSearchAreaTextBoxes();
            });

            map.addInteraction(draw);
            //TODO auto select newly added shape

            snap = new ol.interaction.Snap({ source: drawingSource });
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
        id: "boundaryVector",
        source: boundarySource,
        style: boundaryStyleFunction
    });

    //variable gets data from the ViewBag.boundaries to add boundaries to layer
    var boundariesFromHtmlData = document.getElementById("Boundaries").getAttribute('data-geoJson');
    var formatBoundary = new ol.format.GeoJSON();
    var boundaries = formatBoundary.readFeatures(boundariesFromHtmlData);
    boundarySource.addFeatures(boundaries);

    //layer for coverage area
    var coverageSource = new ol.source.Vector({ wrapX: false });
    var coverageVector = new ol.layer.Vector({
        id: "coverageVector",
        source: coverageSource,
        style: coverageStyle
    });

    //variable gets data from the ViewBag.boundaries to add boundaries to coverage layer
    var coverageFromHtmlData = document.getElementById("Boundaries").getAttribute('data-coverageGeoJson');
    var formatCoverage = new ol.format.GeoJSON();
    var coverageBoundaries = formatCoverage.readFeatures(coverageFromHtmlData);
    coverageSource.addFeatures(coverageBoundaries);

    //layer to display user map interactions / drawing
    var drawingSource = new ol.source.Vector({ wrapX: false });
    var vector = new ol.layer.Vector({
        id: "drawingVector",
        source: drawingSource


    });
    //layer to display labels for drawn features
    var labelSource = new ol.source.Vector({ wrapX: false });
    var labelVector = new ol.layer.Vector({
        id: "labelVector",
        source: labelSource,
        style: labelStyleFunction,
        declutter: true
    });

    //layer to display gridRef
    var gridRefSource = new ol.source.Vector({ wrapX: false });
    var gridRefVector = new ol.layer.Vector({
        id: "gridRefVector",
        source: gridRefSource,
        style: gridRefStyleFunction,
        declutter: true
    });


    //set the projection and centre / zoom map
    var defaultView = new ol.View({
        projection: 'EPSG:27700',
        center: [362000, 369000],
        zoom: 5,
        minZoom: 4,
        maxZoom: 16
    })

    //create map with new layers
    map = new ol.Map({
        layers: [raster, coverageVector, boundaryVector, vector, labelVector, gridRefVector],
        target: 'map',
        view: defaultView,
        loadTilesWhileInteracting: true
    });

    if (typeSelect !== null) {
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

    ////event listener for bing maps layer select
    //var select = document.getElementById('layer-select');
    //function onChange() {
    //    var style = select.value;
    //    for (var i = 0, ii = layers.length; i < ii; ++i) {
    //        layers[i].setVisible(styles[i] === style);
    //    }
    //}
    //select.onchange = onChange();
    //onChange();

    //if wkt is not "" (edit mode), set json and map up search area
    var wkt = searchAreaWkt.value;
    if (wkt !== "") {
        setMapAndTextFromWkt(wkt, drawingSource);
    }
    else {
        ExtendToLayerFeatures(coverageSource);
    }
    var element = document.getElementById('popup');

    //set up popup info
    var popup = new ol.Overlay({
        element: element,
        positioning: 'bottom-center',
        stopEvent: false,
        offset: [0, -50]
    });
    map.addOverlay(popup);

    // display popup on click
    //map.on('click', function (evt) {
    //    var value = typeSelect.value;
    //    var drawingFeatures = getFeaturesFromLayer("drawingVector");
    //    var layerGroup = map.getLayerGroup();
    //    var layers = map.getLayers();
    //    if (value == 'Select') {
    //        var feature = map.forEachFeatureAtPixel(evt.pixel,
    //            function (feature, layer) {
    //                if (layer) {
    //                    if (layer.get('id') == "drawingVector") {
    //                        return feature;
    //                    }
    //                }
    //            });
    //        if (feature) {
    //            var coordinates = feature.getGeometry().getCoordinates();
    //            popup.setPosition(coordinates);
    //            $(element).popover({
    //                'placement': 'top',
    //                'html': true,
    //                'content': feature.get('name')
    //            });
    //            $(element).popover('show');

    //        } else {
    //            $(element).popover('destroy');
    //        }
    //    }
    //    else {
    //        $(element).popover('destroy');
    //    }
    //});


    // change mouse cursor when over marker
    //map.on('pointermove', function(e) {
    //    if (e.dragging) {
    //        $(element).popover('destroy');
    //        return;
    //    }
    //    var pixel = map.getEventPixel(e.originalEvent);
    //    var hit = map.hasFeatureAtPixel(pixel);
    //    map.getTarget().style.cursor = hit ? 'pointer' : '';
    //});
}

//get array of new boundaries intersecting the feature for labelling and local authority wording
function getIntersectingDisplayAreas(feature) {
    var parser = new jsts.io.OL3Parser();
    var boundaryFeatures = getFeaturesFromLayer("boundaryVector");
    var jstsGeomOfFeature = parser.read(feature.getGeometry());
    var intersectingFeatures = [];
    boundaryFeatures.forEach(boundaryFeature => {
        var boundaryName = boundaryFeature.get("Name");
        var jstsGeomOfBoundary = parser.read(boundaryFeature.getGeometry());
        var intersection = jstsGeomOfFeature.intersection(jstsGeomOfBoundary);
        var newFeature = new ol.Feature();
        newFeature.setGeometry(parser.write(intersection));
        newFeature.set("Name", boundaryName);
        intersectingFeatures.push(newFeature);
    })
    return intersectingFeatures;
}

function addLabelsToMap() {
    var drawingFeatures = getFeaturesFromLayer("drawingVector");
    var labelSource = getLayerSource("labelVector");
    labelSource.clear();
    drawingFeatures.forEach(feature => {
        var intersecting = getIntersectingDisplayAreas(feature);
        intersecting.forEach(labelFeature => {
            labelSource.addFeature(labelFeature);
        })
    })
}

//get intersection with coverage area
function removeOutsideCoverageArea() {
    //merge features
    unionFeaturesInLayer("drawingVector");
    unionFeaturesInLayer("coverageVector");
    //create jsts parser
    var parser = new jsts.io.OL3Parser();
    var drawingSource = getLayerSource("drawingVector");
    var features = getFeaturesFromLayer("drawingVector");
    var coverageFeatures = getFeaturesFromLayer("coverageVector");
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
    drawingSource.clear();
    drawingSource.addFeature(newFeature);
    populateTextBoxes();
}

//enlarge view window to see all drawings
function ExtendToLayerFeatures(layerSource) {
    try {

        //Create an empty extent that will gradually extend to all drawings
        var extent = ol.extent.createEmpty();


        ol.extent.extend(extent, layerSource.getExtent());

        //Finally fit the map's view to our combined extent
        map.getView().fit(extent, map.getSize());
        if (map.getView().getZoom() > 15) {
            map.getView().setZoom(15);
        }


    } catch (e) {
        alert(e.message, e.name);
    }
}

function getCostFromArea(area) {
    var cost;
    var roundedCost;
    var searchType = document.getElementById("SearchTypeId").value;

    if (area === 0) {
        cost = 0;
    }
    else {
        if (searchType === "1")
        {
            cost = ((area - 314) / 12.57) + 150;
        }
        else
        {
            cost = (((area - 314) / 12.57) + 150)/2;
        }
    }

    roundedCost = Math.round(cost * 1) / 1;
    return roundedCost;
}

function populateTextBoxes() {
    //open layers helpers to convert features to wkt and geoJson
    var wktFormat = new ol.format.WKT();
    var geoJsonFormat = new ol.format.GeoJSON();

    var features = getFeaturesFromLayer("drawingVector");
    if (features.length > 0) {
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
    else {
        clearSearchAreaTextBoxes();
    }
}

function setArea() {
    try {
        var features = getFeaturesFromLayer("drawingVector");
        var drawingSource = getLayerSource("drawingVector");
        if (features.length > 0) {

            addLabelsToMap();
            unionFeaturesInLayer("drawingVector");

            removeOutsideCoverageArea();
            ExtendToLayerFeatures(drawingSource);
            populateTextBoxes();

        }


    } catch (e) {
        alert(e.message, e.name);
    }
}

//clear current selected feature
function ClearSelectedShape() {
    var drawingSource = getLayerSource("drawingVector");
    if (select !== null) {

            var features = select.getFeatures();
            features.forEach(feature => {
                drawingSource.removeFeature(feature);
                map.removeInteraction(select);
                select = null;
            });

    }
    else {
        alert("no shape selected, please change 'Geometry type' dropdown to 'Select', then select a shape to delete")
    }
    changeInteraction();
    //clear text as map has changed
    if (select !== null) {
        populateTextBoxes();
    } else {
        clearSearchAreaTextBoxes();
    }

}

//clear text representation of drawn map features
function clearSearchAreaTextBoxes() {
    searchAreaWkt.value = "";
    searchAreaJson.value = "";
    areaText.value = "";
    costText.value = "";
}

function clearDrawings() {
    //clear all drawings
    var drawingSource = getLayerSource("drawingVector");
    drawingSource.clear();
}

function clearLabels()
{
    //clear all labels
    var labelSource = getLayerSource("labelVector");
    labelSource.clear();
}
function clearGridSquares(){
    //clear all gridSquares
    var gridRefSource = getLayerSource("gridRefVector");
    gridRefSource.clear();
}

function clearDrawingsAndLabels() {
    clearDrawings();
    clearLabels();
    clearGridSquares()
}

//clear all drawings and reset map
function ResetMapAndWkt() {
    clearDrawingsAndLabels();

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
    var layerSource = getLayerSource(layerName);
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
    layerSource.clear();
    layerSource.addFeature(unionOfFeatures);
}

//apply distance buffer to individual feature
function applyBufferToFeature(feature, distance) {
    //use jsts parser
    var parser = new jsts.io.OL3Parser();

    // convert the OpenLayers geometry to a JSTS geometry
    var jstsGeom = parser.read(feature.getGeometry());

    // create a buffer around the feature
    var buffered = jstsGeom.buffer(distance);
    var geom = parser.write(buffered)
    return geom;
}

//apply buffer to all features
function applyBufferToShapes() {
    var distance = document.getElementById("Buffer").value;
    if (distance !== "" || distance !== "0") {
        var features = getFeaturesFromLayer("drawingVector");
        var drawingSource = getLayerSource("drawingVector");
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
            feature.setGeometry(buffered);
        })

        //unselect selected feature if required
        if (select !== null) {
            map.removeInteraction(select);
        }

        //zoom to extent and populate Text Boxes with new area
        ExtendToLayerFeatures(drawingSource);
        populateTextBoxes();
    }


}

//undo buffer function
function undoApplyBufferToShapes() {
    if (savedFeatures.length > 0) {
        //get newly added shape
        var features = savedFeatures.pop();
        clearDrawingsAndLabels();
        //add last saved features to map
        var drawingSource = getLayerSource("drawingVector");
        features.forEach(feature => {
            drawingSource.addFeature(feature);
        })
        //unselect
        if (select !== null) {
            map.removeInteraction(select);
        }
        //zoom and clear text
        ExtendToLayerFeatures(drawingSource);
        populateTextBoxes();
        clearLabels();
    }

}

//function AddressLookup() {
//    var address = document.getElementById("Address").value;
//    geocoder.geocode({ 'address': address }, function (results, status) {
//        if (status == google.maps.GeocoderStatus.OK) {
//            map.getView().setCenter(results[0].geometry.location);
//            //var marker = new google.maps.Marker({
//            //    map: map,
//            //    position: results[0].geometry.location
//            //});
//        } else {
//            alert("Geocode was not successful for the following reason: " + status);
//        }
//    });
//}

function CenterAndZoomMap(eAndN, zoom) {
    //console.log("east: " + east + " north: " + north);

    map.getView().setCenter(eAndN);
    map.getView().setZoom(zoom);
}

//returns geometry representing the gridref supplied
function GetSquareFromGridRef(gridRef) {
    var squareSize = 0;
    var geom = null;
    //this gets the size of the gridsquare from length of gridref
    gridRef = gridRef.replace(/\s/g, '');
    if (gridRef && gridRef.length > 1 && gridRef.length % 2 === 0) {
        gridRefFigure = gridRef.length - 2;
        switch (gridRefFigure) {
            case 0:
                squareSize = 100000;
                break;
            case 2:
                squareSize = 10000;
                break;
            case 4:
                squareSize = 1000;
                break;
            case 6:
                squareSize = 100;
                break;
            case 8:
                squareSize = 10;
                break;
            case 10:
                squareSize = 1;
                break;
            case 12:
                squareSize = 0.1;
                break;
        }

        //using latlon2bng.js and latlon.js to convert grid ref to easting and northing
        var EN = OsGridRef.parse(gridRef.toString(gridRefFigure));
        var E = EN.easting;
        var N = EN.northing;

        //create ol.coordinates for grid square (coordinate is simple array of xy e.g. [x, y])
        var xy1 = ol.coordinate.add([E, N], [0, 0]);

        var xy2 = ol.coordinate.add([E, N], [squareSize, 0]);

        var xy3 = ol.coordinate.add([E, N], [squareSize, squareSize]);

        var xy4 = ol.coordinate.add([E, N], [0, squareSize]);

        //coordinates are array of coordinate i.e. [[x1,y1], [x2,y2], ...[x1,y1]] 
        //polygon also needs to be closed so last coordinate sould be the same as first
        var coordinates = [xy1, xy2, xy3, xy4, xy1];

        //ol.geom.Polygon constructor takes an array of coordinates the first is the outer polygon
        //the others are the inner holes, in this case just one is supplies but is 
        //important to provide the array, the "XY" is the format of the coordinates
        //i.e. new ol.geom.Polygon([coordinatesOfOuter, coordinatesOfInner1, ... coordinatesOfInner2], "XY")
        geom = new ol.geom.Polygon([coordinates], "XY");
    }
    return geom;
}

function ValidateAndReformatGridRef(gridRef) {
    var gridRefFormatted = null;
    gridRef = gridRef.replace(/\s/g, '');
    var regExpGridRef = /^[A-Za-z][A-Za-z](\d\d)*$/;

    if (regExpGridRef.test(gridRef)) {
        //format grid ref for name display
        gridRefFormatted = gridRef.replace(/[a-z]/g, Function.prototype.call.bind(String.prototype.toUpperCase));
        return gridRefFormatted;
    }
    else {
        throw "Invalid grid reference";
    }
}

//function to add grid reference to map
function GridReferenceLookup() {
    try {
        var gridRefElement = document.getElementById("GridReferenceLookup");
        var validGridRef = ValidateAndReformatGridRef(gridRefElement.value);

        //get geom of gridsquare
        var geom = GetSquareFromGridRef(validGridRef);
        if (geom) {
            var chkGridRefLayer = document.getElementById("chkGridRefLayer");
            chkGridRefLayer.checked = true;
            UpdateLayers();
            //create feature including naming with gridref
            var gridRefFeature = new ol.Feature({
                geometry: geom,
                Name: validGridRef
            });

            var gridRefSource = getLayerSource("gridRefVector")
            gridRefSource.clear();
            gridRefSource.addFeatures([gridRefFeature]);

            //zoom to gridsquare
            ExtendToLayerFeatures(gridRefSource);
        }
        else {
            alert("invalid grid reference");
        }
    }
    catch (exception) {
        alert(exception);
    }

}
function UpdateLayers() {
    var chkBoundaryLayer = document.getElementById("chkBoundaryLayer");
    var boundaryLayer = getLayerById("boundaryVector");
    var chkGridRefLayer = document.getElementById("chkGridRefLayer");
    var gridRefLayer = getLayerById("gridRefVector");
    var chkLabelLayer = document.getElementById("chkLabelLayer");
    var labelLayer = getLayerById("labelVector");

    if (chkBoundaryLayer.checked === true) {
        boundaryLayer.setVisible(true);
    }
    else {
        boundaryLayer.setVisible(false);
    }

    if (chkGridRefLayer.checked === true) {
        gridRefLayer.setVisible(true);
    }
    else {
        gridRefLayer.setVisible(false);
    }

    if (chkLabelLayer.checked === true) {
        labelLayer.setVisible(true);
    }
    else {
        labelLayer.setVisible(false);
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
    clearLabels();
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

//function AddressLookupButtonClick() {
//    AddressLookup();
//}

function GridReferenceLookupButtonClick() {
    GridReferenceLookup();
}

function UpdateLayersClick() {
    UpdateLayers();
}

initialiseMap();