using DotSpatial.Projections;
using DotSpatial.Topology;
using DotSpatial.Topology.Utilities;
using Proj4Net;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace EnquiryRequest3.Controllers.Utilities
{
    public static class Spatial
    {
        public static DbGeometry TransformGeom(DbGeometry geom, int srid) //(EPSG:27700) is OSGB, (EPSG:3857) google maps geometric, wgs84 (EPSG:4326) google geographic
        {
            DbGeometry TransformedGeom;
            double[] xy = GetArrayOfCoordinatesFromGeom(geom);
            //potentially use dotspatial:
            if (geom.PointCount != null)
            {
                double[] zArr = new double[1];
                zArr[0] = 1;
                ProjectionInfo srcProjection = new ProjectionInfo();
                srcProjection.AuthorityCode = geom.CoordinateSystemId;
                ProjectionInfo trgProjection = new ProjectionInfo();
                trgProjection.AuthorityCode = srid;
                Reproject.ReprojectPoints(xy, zArr, srcProjection, trgProjection, 0, (int)geom.PointCount);
                
            }
            TransformedGeom = null; //ArrayOfCoordinatesToGeom(xy, srid);

            //use Proj4Net:
            //Proj4Net.CoordinateReferenceSystemFactory coordinateReferenceSystemFactory = new CoordinateReferenceSystemFactory();
            //var crsSource = coordinateReferenceSystemFactory.CreateFromParametersCreateFromName("EPSG:3857");
            //var crsTarget = coordinateReferenceSystemFactory.CreateFromName("EPSG:3857");
            //CoordinateTransformFactory coordinateTransformFactory = new CoordinateTransformFactory();
            //var transform = coordinateTransformFactory.CreateTransform(crsSource, crsTarget);
            //GeoAPI.Geometries.Coordinate srcCoord = new GeoAPI.Geometries.Coordinate();
            //GeoAPI.Geometries.Coordinate tgtCoord = new GeoAPI.Geometries.Coordinate();
            //transform.Transform(srcCoord, tgtCoord);

            return TransformedGeom;
        }
        
        public static Geometry DbGeomToDotSpatialGeom(DbGeometry inputGeom)
        {
            var binGeom = inputGeom.AsBinary();
            List<byte> list = binGeom.ToList();
            Geometry dotSpatialGeom = (Geometry)Geometry.DefaultFactory.BuildGeometry(list);
            return dotSpatialGeom;
        }

        public static double[] GetArrayOfCoordinatesFromGeom(DbGeometry geom)
        {
            if (!geom.IsValid || geom.IsEmpty)
            {
                throw new Exception();
            }

            WktWriter wktwriter = new WktWriter();
            var wkt = wktwriter.Write(DbGeomToDotSpatialGeom(geom));
            wkt.Remove(0, wkt.IndexOf('('));
            
            List<double> list = new List<double>();
            int? numOfPoints = geom.PointCount;
            if(numOfPoints != null)
            {
                for (int i = 1; i <= numOfPoints; i++)
                {
                    list.Add((double)geom.PointAt(i).XCoordinate);
                    list.Add((double)geom.PointAt(i).YCoordinate);
                };
            }

            return list.ToArray<double>();
        }

        //public static DbGeometry ArrayOfCoordinatesToGeom(double[] arrCoord, int srid)
        //{
        //    List<double> list = new List<double>();
        //    for (int i = 0; i < arrCoord.Length / 2; i++)
        //    {
        //        list.Add((double)geom.PointAt(i).XCoordinate);
        //        list.Add((double)geom.PointAt(i).YCoordinate);
        //    };

        //    DbGeometry geom = DbGeometry.FromText()
        //    return geom;
        //}
    }
}