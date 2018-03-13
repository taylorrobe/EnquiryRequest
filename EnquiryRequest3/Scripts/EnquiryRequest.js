
function GMapPolygonToWKT(poly)
{
 // Start the Polygon Well Known Text (WKT) expression
 var wkt = "POLYGON(";

 var paths = poly.getPaths();
 for(var i=0; i<paths.getLength(); i++)
 {
  var path = paths.getAt(i);
  
  // Open a ring grouping in the Polygon Well Known Text
  wkt += "(";
  for(var j=0; j<path.getLength(); j++)
  {
   // add each vertice and anticipate another vertice (trailing comma)
   wkt += path.getAt(j).lng().toString() +" "+ path.getAt(j).lat().toString() +",";
  }
  
  // Google's approach assumes the closing point is the same as the opening
  // point for any given ring, so we have to refer back to the initial point
  // and append it to the end of our polygon wkt, properly closing it.
  //
  // Also close the ring grouping and anticipate another ring (trailing comma)
  wkt += path.getAt(0).lng().toString() + " " + path.getAt(0).lat().toString() + "),";
 }
 
 // resolve the last trailing "," and close the Polygon
 wkt = wkt.substring(0, wkt.length - 1) + ")";
 
 return wkt;
}