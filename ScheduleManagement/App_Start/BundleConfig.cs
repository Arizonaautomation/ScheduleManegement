using System.Web;
using System.Web.Optimization;

namespace TrainningManagement
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            //CSS
            bundles.Add(new StyleBundle("~/bundle/LayoutCSS").Include(
                "~/Content/assets/css/bootstrap.min.css",
                "~/Content/assets/css/bootstrap-datetimepicker.min.css",
                "~/Content/assets/font-awesome/4.5.0/css/font-awesome.min.css",
                "~/Content/assets/css/fonts.googleapis.com.css",
                "~/Content/assets/css/ace.min.css",
                "~/Content/assets/css/ace-skins.min.css",
                "~/Content/assets/css/ace-rtl.min.css",
                "~/Content/FormStyle.css"
                ));

            //Js For LayOut
            bundles.Add(new ScriptBundle("~/bundles/LayoutJs").Include(
                "~/Content/assets/js/ace-extra.min.js",
                //"~/Content/assets/js/jquery-2.1.4.min.js",
                "~/Content/assets/js/bootstrap.min.js",
                "~/Content/assets/js/bootstrap-datepicker.min.js",
                "~/Content/assets/js/moment.min.js",
                "~/Content/assets/js/bootstrap-timepicker.min.js",
                "~/Content/assets/js/bootstrap-datetimepicker.min.js",
                "~/assets/global/plugins/bootbox/bootbox.min.js",
                "~/Content/assets/js/jquery-ui.custom.min.js",
                "~/Content/assets/js/jquery.ui.touch-punch.min.js",
                "~/Content/assets/js/jquery.easypiechart.min.js",
                "~/Content/assets/js/jquery.sparkline.index.min.js",
                "~/Content/assets/js/jquery.flot.min.js",
                "~/Content/assets/js/jquery.flot.pie.min.js",
                "~/Content/assets/js/jquery.flot.resize.min.js",
                "~/Content/assets/js/dataTables.select.min.js",
                "~/Content/assets/js/dataTables.buttons.min.js",
                "~/Content/assets/js/daterangepicker.min.js",
                "~/Content/assets/js/ace-elements.min.js",
                "~/Content/assets/js/ace.min.js"
                ));

        }
    }
}
