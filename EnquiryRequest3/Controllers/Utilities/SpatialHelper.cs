using NetTopologySuite.Features;
using NetTopologySuite.IO;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using EnquiryRequest3.Models;
using GeoAPI.Geometries;

namespace EnquiryRequest3.Controllers.Utilities
{
    public class SpatialHelper
    {
        public List<string> GetGeoJsonListFromArray(DbGeometry[] geometryArray)
        {
            List<string> returnArray = new List<string>();
            foreach (DbGeometry dBGeometry in geometryArray)
            {
                returnArray.Add(GetGeoJsonFromGeometry(dBGeometry));
            }
            return returnArray;
        }
        public string GetGeoJsonFromGeometry(DbGeometry dBGeometry)
        {
            var geoJsonWriter = new NetTopologySuite.IO.GeoJsonWriter();
            return geoJsonWriter.Write(dBGeometry);
        }
        public string GetGeoJsonCollectionFromBoundaryCollection(ICollection<Boundary> collectionOfBoundaries)
        {
            //create GeoJSON writer
            GeoJsonWriter geoJsonWriter = new GeoJsonWriter();
            //create WKT Binary reader
            WKBReader wKBReader = new WKBReader();
            //create empty feature collection 
            FeatureCollection features = new FeatureCollection();
            //iterate through collection
            foreach (Boundary boundary in collectionOfBoundaries)
            {
                //set up feature attributes table
                AttributesTable attributesTable = new AttributesTable();
                //add attributes to table from boundary
                attributesTable.AddAttribute("id", boundary.BoundaryId);
                attributesTable.AddAttribute("Name", boundary.Name);
                //convert DbGeometry to GeoApi.IGeometry
                IGeometry iGeom = wKBReader.Read(boundary.Area.AsBinary());
                //create feature from geom and attributes
                Feature feature = new Feature(iGeom, attributesTable);
                //add feature to feature collection
                features.Add(feature);
            }
            return geoJsonWriter.Write(features);
        }
    }
}