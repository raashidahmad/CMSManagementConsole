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

            bundles.Add(new ScriptBundle("~/bundles/fileuploadjs").Include(
                     //<!-- The Templates plugin is included to render the upload/download listings -->
                     "~/Scripts/jQuery.FileUpload/vendor/jquery.ui.widget.js",
                     "~/Scripts/jQuery.FileUpload/tmpl.min.js",
                     //<!-- The Load Image plugin is included for the preview images and image resizing functionality -->
                     "~/Scripts/jQuery.FileUpload/load-image.all.min.js",
                    //<!-- The Canvas to Blob plugin is included for image resizing functionality -->
                    "~/Scripts/jQuery.FileUpload/canvas-to-blob.min.js",
                    //"~/Scripts/file-upload/jquery.blueimp-gallery.min.js",
                    //<!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
                    "~/Scripts/jQuery.FileUpload/jquery.iframe-transport.js",
                    //<!-- The basic File Upload plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload.js",
                    //<!-- The File Upload processing plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-process.js",
                    //<!-- The File Upload image preview & resize plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-image.js",
                                    //<!-- The File Upload audio preview plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-audio.js",
                                    //<!-- The File Upload video preview plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-video.js",
                                    //<!-- The File Upload validation plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-validate.js",
                                    //!-- The File Upload user interface plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-ui.js",
                                    //Blueimp Gallery 2 
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-video.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-indicator.js",
                    "~/Scripts/blueimp-gallery2/js/jquery.blueimp-gallery.js"
                ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/startmin.css",
                      "~/Content/css/metisMenu.min.css",
                      "~/Content/css/font-awesome.min.css"));

            bundles.Add(new StyleBundle("~/Content/fileuploadcss").Include(
                    "~/Content/css/jquery.fileupload.css",
                    "~/Content/css/jquery.fileupload-ui.css",
                    "~/Content/css/blueimp/blueimp-gallery.css",
                    "~/Content/css/blueimp/blueimp-gallery-video.css",
                    "~/Content/css/blueimp/blueimp-gallery-indicator.css"
                ));

            bundles.Add(new StyleBundle("~/Content/logincss").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/bootstrap-theme.min.css",
                      "~/Content/site.css"));
            }

            
        }
    }
