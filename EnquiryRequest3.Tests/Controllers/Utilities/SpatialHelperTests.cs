using EnquiryRequest3.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.Entity.Spatial;

namespace EnquiryRequest3.Controllers.Utilities.Tests
{
    [TestClass()]
    public class SpatialHelperTests
    {


        [TestMethod()]
        public void GetGeoJsonListFromArrayTest()
        {
            var wkt = "Polygon((0 0 , 1 0, 1 1, 0 1, 0 0))";
            DbGeometry dBGeometry = DbGeometry.FromText(wkt);
            var wkt2 = "Polygon((0 0 , 2 0, 2 2, 0 2, 0 0))";
            DbGeometry dBGeometry2 = DbGeometry.FromText(wkt2);
            var geomArray = new DbGeometry[] { dBGeometry, dBGeometry2 };
            SpatialHelper spatialHelper = new SpatialHelper();
            var result = spatialHelper.GetGeoJsonListFromArray(geomArray);
            List<string> expectedResult = new List<string>();
            expectedResult.Add("{\"Geometry\":{\"WellKnownText\":\"Polygon((0 0 , 1 0, 1 1, 0 1, 0 0))\"}}");
            expectedResult.Add("{\"Geometry\":{\"WellKnownText\":\"Polygon((0 0 , 2 0, 2 2, 0 2, 0 0))\"}}");
            CollectionAssert.AreEqual(expectedResult, result);
        }

        [TestMethod()]
        public void GetGeoJsonFromGeometryTest()
        {
            var wkt = "Polygon((0 0 , 1 0, 1 1, 0 1, 0 0))";
            DbGeometry dBGeometry = DbGeometry.FromText(wkt);
            SpatialHelper spatialHelper = new SpatialHelper();
            var result = spatialHelper.GetGeoJsonFromGeometry(dBGeometry);
            string expectedResult = "{\"Geometry\":{\"WellKnownText\":\"Polygon((0 0 , 1 0, 1 1, 0 1, 0 0))\"}}";
            Assert.AreEqual(result, expectedResult);
        }

        [TestMethod()]
        public void GetGeoJsonCollectionFromBoundaryCollectionTestBoundaryTypeAll()
        {
            //first boundary coverage area only
            Boundary boundary1 = new Boundary();
            var wkt1 = "Polygon((0 0 , 1 0, 1 1, 0 1, 0 0))";
            DbGeometry dBGeometry1 = DbGeometry.FromText(wkt1);
            boundary1.Area = dBGeometry1;
            boundary1.BoundaryId = 1;
            boundary1.Name = "boundary1";
            boundary1.displayOnMap = false;
            boundary1.isCoverageArea = true;

            //second boundary display only
            Boundary boundary2 = new Boundary();
            var wkt2 = "Polygon((0 0 , 1 0, 1 1, 0 1, 0 0))";
            DbGeometry dBGeometry2 = DbGeometry.FromText(wkt2);
            boundary2.Area = dBGeometry2;
            boundary2.BoundaryId = 2;
            boundary2.Name = "boundary2";
            boundary2.displayOnMap = true;
            boundary2.isCoverageArea = false;

            //third boundary neither
            Boundary boundary3 = new Boundary();
            var wkt3 = "Polygon((0 0 , 1 0, 1 1, 0 1, 0 0))";
            DbGeometry dBGeometry3 = DbGeometry.FromText(wkt3);
            boundary3.Area = dBGeometry3;
            boundary3.BoundaryId = 3;
            boundary3.Name = "boundary3";
            boundary3.displayOnMap = false;
            boundary3.isCoverageArea = false;

            //fourth boundary both
            Boundary boundary4 = new Boundary();
            var wkt4 = "Polygon((0 0 , 1 0, 1 1, 0 1, 0 0))";
            DbGeometry dBGeometry4 = DbGeometry.FromText(wkt4);
            boundary4.Area = dBGeometry4;
            boundary4.BoundaryId = 4;
            boundary4.Name = "boundary4";
            boundary4.displayOnMap = true;
            boundary4.isCoverageArea = true;

            ICollection<Boundary> boundaries = new List<Boundary>
            {
                boundary1,
                boundary2,
                boundary3,
                boundary4
            };

            SpatialHelper spatialHelper = new SpatialHelper();
            var result = spatialHelper.GetGeoJsonCollectionFromBoundaryCollection
                (boundaries, SpatialHelper.BoundaryType.ALL);
            //TODO set up expected result
            var expectedResult = "";

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod()]
        public void GetGeoJsonCollectionFromBoundaryCollectionTestBoundaryTypeCoverage()
        {
            //first boundary coverage area only
            Boundary boundary1 = new Boundary();
            var wkt1 = "Polygon((0 0 , 1 0, 1 1, 0 1, 0 0))";
            DbGeometry dBGeometry1 = DbGeometry.FromText(wkt1);
            boundary1.Area = dBGeometry1;
            boundary1.BoundaryId = 1;
            boundary1.Name = "boundary1";
            boundary1.displayOnMap = false;
            boundary1.isCoverageArea = true;

            //second boundary display only
            Boundary boundary2 = new Boundary();
            var wkt2 = "Polygon((0 0 , 1 0, 1 1, 0 1, 0 0))";
            DbGeometry dBGeometry2 = DbGeometry.FromText(wkt2);
            boundary2.Area = dBGeometry2;
            boundary2.BoundaryId = 2;
            boundary2.Name = "boundary2";
            boundary2.displayOnMap = true;
            boundary2.isCoverageArea = false;

            //third boundary neither
            Boundary boundary3 = new Boundary();
            var wkt3 = "Polygon((0 0 , 1 0, 1 1, 0 1, 0 0))";
            DbGeometry dBGeometry3 = DbGeometry.FromText(wkt3);
            boundary3.Area = dBGeometry3;
            boundary3.BoundaryId = 3;
            boundary3.Name = "boundary3";
            boundary3.displayOnMap = false;
            boundary3.isCoverageArea = false;

            //fourth boundary both
            Boundary boundary4 = new Boundary();
            var wkt4 = "Polygon((0 0 , 1 0, 1 1, 0 1, 0 0))";
            DbGeometry dBGeometry4 = DbGeometry.FromText(wkt4);
            boundary4.Area = dBGeometry4;
            boundary4.BoundaryId = 4;
            boundary4.Name = "boundary4";
            boundary4.displayOnMap = true;
            boundary4.isCoverageArea = true;

            ICollection<Boundary> boundaries = new List<Boundary>
            {
                boundary1,
                boundary2,
                boundary3,
                boundary4
            };

            SpatialHelper spatialHelper = new SpatialHelper();
            var result = spatialHelper.GetGeoJsonCollectionFromBoundaryCollection
                (boundaries, SpatialHelper.BoundaryType.COVERAGE);
            //TODO set up expected result
            var expectedResult = "";

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod()]
        public void GetGeoJsonCollectionFromBoundaryCollectionTestBoundaryTypeDisplay()
        {
            //first boundary coverage area only
            Boundary boundary1 = new Boundary();
            var wkt1 = "Polygon((0 0 , 1 0, 1 1, 0 1, 0 0))";
            DbGeometry dBGeometry1 = DbGeometry.FromText(wkt1);
            boundary1.Area = dBGeometry1;
            boundary1.BoundaryId = 1;
            boundary1.Name = "boundary1";
            boundary1.displayOnMap = false;
            boundary1.isCoverageArea = true;

            //second boundary display only
            Boundary boundary2 = new Boundary();
            var wkt2 = "Polygon((0 0 , 1 0, 1 1, 0 1, 0 0))";
            DbGeometry dBGeometry2 = DbGeometry.FromText(wkt2);
            boundary2.Area = dBGeometry2;
            boundary2.BoundaryId = 2;
            boundary2.Name = "boundary2";
            boundary2.displayOnMap = true;
            boundary2.isCoverageArea = false;

            //third boundary neither
            Boundary boundary3 = new Boundary();
            var wkt3 = "Polygon((0 0 , 1 0, 1 1, 0 1, 0 0))";
            DbGeometry dBGeometry3 = DbGeometry.FromText(wkt3);
            boundary3.Area = dBGeometry3;
            boundary3.BoundaryId = 3;
            boundary3.Name = "boundary3";
            boundary3.displayOnMap = false;
            boundary3.isCoverageArea = false;

            //fourth boundary both
            Boundary boundary4 = new Boundary();
            var wkt4 = "Polygon((0 0 , 1 0, 1 1, 0 1, 0 0))";
            DbGeometry dBGeometry4 = DbGeometry.FromText(wkt4);
            boundary4.Area = dBGeometry4;
            boundary4.BoundaryId = 4;
            boundary4.Name = "boundary4";
            boundary4.displayOnMap = true;
            boundary4.isCoverageArea = true;

            ICollection<Boundary> boundaries = new List<Boundary>
            {
                boundary1,
                boundary2,
                boundary3,
                boundary4
            };

            SpatialHelper spatialHelper = new SpatialHelper();
            var result = spatialHelper.GetGeoJsonCollectionFromBoundaryCollection
                (boundaries, SpatialHelper.BoundaryType.DISPLAY);
            //TODO set up expected result
            var expectedResult = "";

            Assert.AreEqual(expectedResult, result);
        }
    }
    
}