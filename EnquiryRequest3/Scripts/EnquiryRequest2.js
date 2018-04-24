var draw, snap; // global so we can remove them later
var typeSelect = document.getElementById('type');
var shape, vector, modify, source, map;
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
        snap = new ol.interaction.Snap({ source: source });
        map.addInteraction(snap);
    }
    
}

function changeInteraction () {
    if (select !== null) {
        map.removeInteraction(select);
    }

    //select = selectClick;
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

function initialiseMap(){
    raster = new ol.layer.Tile({
        source: new ol.source.OSM()
    });

    source = new ol.source.Vector({ wrapX: false });

    //var vectorBoundaries = new ol.layer.Vector({
    //    id: 'boundaries',
    //    source: new ol.source.GeoJSON({
    //        projection: 'EPSG:3857',
    //        url: '../assets/data/nutsv9_lea.geojson'
    //    }),
    //    style: defaultEuropa
    //});

    vector = new ol.layer.Vector({
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

    

    draw = new ol.interaction.Modify({ source: source });
    map.addInteraction(draw);

    typeSelect = document.getElementById('type');

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
    
    // select interaction working on "click"
    selectClick = new ol.interaction.Select({
        condition: ol.events.condition.click,
        layers: function (layer) {
            return layer.get('id') == 'drawingVector';
        }
    });



    /**
     * onchange callback on the select element.
     */
    changeInteraction();
}

function extendToDrawing() {
    try {

        //Create an empty extent that we will gradually extend
        var extent = ol.extent.createEmpty();

        map.getLayers().forEach(function (layer) {
            //If this is actually a group, we need to create an inner loop to go through its individual layers
            if (layer instanceof ol.layer.Group) {
                layer.getLayers().forEach(function (groupLayer) {
                    //If this is a vector layer, add it to our extent
                    if (layer instanceof ol.layer.Vector)
                        ol.extent.extend(extent, groupLayer.getSource().getExtent());
                    var source = layer.getSource();

                });
            }
            else if (layer instanceof ol.layer.Vector)
                ol.extent.extend(extent, layer.getSource().getExtent());
            var source = layer.getSource();

        });

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
        var layers = map.getLayers();
        layers.forEach(layer => {
            if (layer instanceof ol.layer.Vector) {
                var source = layer.getSource();
                if (source) {
                    source.forEachFeature(feature => {
                        feature;
                        wkt = wktFormat.writeFeature(feature);
                    });
                }

            }
        });
        searchAreaWkt.value = wkt;

    } catch (e) {
        alert(e.message, e.name);
    }
}

function ClearSelectedShape() {
    map.removeInteraction(select);
}

function setAreaButtonClick() {
    setArea();
}

function ClearSelectedShapeButtonClick() {
    ClearSelectedShape();
}

initialiseMap();