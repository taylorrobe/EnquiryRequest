using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Spatial;

namespace GeometryHelpers
{
    public class Converters
    {
        public static DbGeometry GetDbGeometryFromWkt()
        {
            DbGeometry returnValue = new DbGeometry.FromText();
            return returnValue;
        }
    }
}
