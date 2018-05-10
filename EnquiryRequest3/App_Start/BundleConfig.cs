using System.Web;
using System.Web.Optimization;

namespace EnquiryRequest3
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;   //enable CDN support

            //add link to jquery on the CDN
            var jqueryCdnPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.3.1.min.js";

            bundles.Add(new ScriptBundle("~/bundles/jquery",
                        jqueryCdnPath).Include(
                        "~/Scripts/jquery-3.3.1.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/enquiryRequest").Include(
            "~/Scripts/jsts.js",
            "~/Scripts/latlon.js",
            "~/Scripts/latlon2bng.js",
            "~/Scripts/EnquiryRequest2.js"));

            //add link to openLayers on the CDN
            var openLayersCdnPath = "https://openlayers.org/en/v4.6.4/build/ol.js";

            //add link to proj4js on the CDN
            var proj4jsCdnPath = "https://cdnjs.cloudflare.com/ajax/libs/proj4js/2.4.4/proj4.js";

            bundles.Add(new ScriptBundle("~/bundles/openLayers", openLayersCdnPath).Include(
                "~/Scripts/ol.js"));

            bundles.Add(new ScriptBundle("~/bundles/proj4js", proj4jsCdnPath).Include(
                "~/Scripts/proj4.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
