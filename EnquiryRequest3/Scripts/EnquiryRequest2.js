
var typeSelect = document.getElementById('type');
var map;
var select = null;  // ref to currently selected interaction
var selectClick;
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

function changeInteraction() {
    // select interaction working on "click"
    selectClick = new ol.interaction.Select({
        condition: ol.events.condition.click,
        layers: function (layer) {
            return layer.get('id') == 'drawingVector';
        }
    });

    var modify = new ol.interaction.Modify({ features: selectClick.getFeatures() });
    map.addInteraction(modify);

    if (select !== null) {
        map.removeInteraction(select);
    }

    var value = typeSelect.value;

    if (value == 'Select') {
        select = selectClick;
    } else {
        select = null;
    }

    if (select !== null) {
        map.addInteraction(select);
        //select.on('select', function (e) {
        //    document.getElementById('status').innerHTML = '&nbsp;' +
        //        e.target.getFeatures().getLength() +
        //        ' selected features (last operation selected ' + e.selected.length +
        //        ' and deselected ' + e.deselected.length + ' features)';
        //});
    }
}

function initialiseMap() {
    var draw, snap;
    
    function drawEnd() {
        map.removeInteraction(draw);
        map.removeInteraction(snap);
    }

    function addInteractions() {
        if (typeSelect.value !== "Select") {

            draw = new ol.interaction.Draw({
                source: source,
                type: typeSelect.value
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

    //var vectorBoundaries = new ol.layer.Vector({
    //    id: 'boundaries',
    //    source: new ol.source.GeoJSON({
    //        projection: 'EPSG:3857',
    //        url: '../assets/data/nutsv9_lea.geojson'
    //    }),
    //    style: defaultEuropa
    //});

    var vector = new ol.layer.Vector({
        id: "drawingVector",
        source: source
    });

    map = new ol.Map({
        layers: [raster, vector],
        target: 'map',
        view: new ol.View({
            projection: 'EPSG:27700',
            center: [362000, 369000],
            zoom: 5
        })
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

        var layer = getLayerById("drawingVector");
        ol.extent.extend(extent, layer.getSource().getExtent());
        var source = layer.getSource();

        //Finally fit the map's view to our combined extent
        map.getView().fit(extent, map.getSize());

    } catch (e) {
        alert(e.message, e.name);
    }
}



function setArea() {
    try {
        extendToDrawing();

        //get text box references from DOM
        var searchAreaWkt = document.getElementById("SearchAreaWkt");
        var searchAreaJson = document.getElementById("SearchAreaJson");

        var wkt;

        //convert features to wkt
        var wktFormat = new ol.format.WKT();
        var layer = getLayerById("drawingVector");
        if (layer instanceof ol.layer.Vector) {
            var source = layer.getSource();
            var features = source.getFeatures();
            wkt = wktFormat.writeFeatures(features);
            //if (source) {
            //    source.forEachFeature(feature => {
            //        feature;
            //        wkt = wktFormat.writeFeature(feature);
            //    });
            //}

        }

        searchAreaWkt.value = wkt;

    } catch (e) {
        alert(e.message, e.name);
    }
}

function ClearSelectedShape() {
    var drawingLayer = getLayerById("drawingVector");
    var source = drawingLayer.getSource();
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


}

function setAreaButtonClick() {
    setArea();
}

function ClearSelectedShapeButtonClick() {
    ClearSelectedShape();
}

initialiseMap();