using System.Web;
using System.Web.Optimization;

namespace CMSManagementConsole
    {
    public class BundleConfig
        {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
            {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.1.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/metisMenu.min.js",
                      "~/Scripts/startmin.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/startmin.css",
                      "~/Content/metisMenu.min.css",
                      "~/Content/font-awesome/css/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/logincss").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/bootstrap-theme.min.css",
                      "~/Content/site.css"));
            }

            
        }
    }
