using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnquiryRequest3.Controllers.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Spatial;

namespace EnquiryRequest3.Controllers.Utilities.Tests
{
    [TestClass()]
    public class SpatialTests
    {
        [TestMethod()]
        public void GeomToArrayOfCoordinatesTest()
        {
            DbGeometry geom = DbGeometry.FromText("POLYGON((-2.6911762336203537 53.298070565906265,-2.5277546027609787 53.29889130834331,-2.5250080207297287 53.27508337365367,-2.5318744758078537 53.256191784672914,-2.5538471320578537 53.23893565266658,-2.5977924445578537 53.232360057460326,-2.6403644660422287 53.234004050957,-2.6719501594016037 53.243044886754575,-2.6884296515891037 53.25372704902184,-2.6952961066672287 53.27344095810511,-2.6911762336203537 53.298070565906265))");
            Spatial.GetArrayOfCoordinatesFromGeom(geom);
            Assert.Fail();
        }

        //[TestMethod()]
        //public void GetArrayOfCoordinatesFromGeomTest()
        //{
        //    Geometry
        //    Spatial.GetArrayOfCoordinatesFromGeom();
        //    Assert.Fail();
        //}
    }
}