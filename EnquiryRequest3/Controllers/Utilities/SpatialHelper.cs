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
        public enum BoundaryType{
            ALL,
            COVERAGE,
            DISPLAY
        }

        //returns geoJSON from array
        public List<string> GetGeoJsonListFromArray(DbGeometry[] geometryArray)
        {
            List<string> returnArray = new List<string>();
            foreach (DbGeometry dBGeometry in geometryArray)
            {
                returnArray.Add(GetGeoJsonFromGeometry(dBGeometry));
            }
            return returnArray;
        }

        //returns geoJSON from geometry
        public string GetGeoJsonFromGeometry(DbGeometry dBGeometry)
        {
            var geoJsonWriter = new GeoJsonWriter();
            return geoJsonWriter.Write(dBGeometry);
        }

        //returns geoJSON of boundaries from DB filters by type
        public string GetGeoJsonCollectionFromBoundaryCollection(ICollection<Boundary> collectionOfBoundaries, BoundaryType boundaryType)
        {
            //create GeoJSON writer
            GeoJsonWriter geoJsonWriter = new GeoJsonWriter();
            //create WKT Binary reader
            WKBReader wKBReader = new WKBReader();
            //create empty feature collection 
            FeatureCollection features = new FeatureCollection();

            //setup return boundary flag
            bool returnBoundary = false;

            //iterate through collection
            foreach (Boundary boundary in collectionOfBoundaries)
            {
                returnBoundary = false;
                switch (boundaryType)
                {
                    case BoundaryType.ALL:
                        returnBoundary = true;
                        break;
                    case BoundaryType.COVERAGE:
                        if(boundary.isCoverageArea)
                        {
                            returnBoundary = true;
                        }
                        break;
                    case BoundaryType.DISPLAY:
                        if (boundary.displayOnMap)
                        {
                            returnBoundary = true;
                        }
                        break;
                }
                if(returnBoundary)
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
            }
            return geoJsonWriter.Write(features);
        }
    }
}