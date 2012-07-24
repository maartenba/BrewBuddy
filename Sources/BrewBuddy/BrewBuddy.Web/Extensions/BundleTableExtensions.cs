using System.Web.Optimization;

namespace BrewBuddy.Web.Extensions
{
    public static class BundleTableExtensions
    {
        public static void EnableBootstrapBundles(this BundleCollection bundles)
        {
            var bootstrapCss = new Bundle("~/bootstrap/css", new CssMinify());
            bootstrapCss.Include("~/Content/bootstrap.css");
            bootstrapCss.Include("~/Content/bootstrap-responsive.css");
            bootstrapCss.Include("~/Content/application.css");

            bundles.Add(bootstrapCss);

            var bootstrapJs = new Bundle("~/bootstrap/js", new JsMinify());
            bootstrapJs.Include("~/Scripts/jquery-1.7.1.js");
            bootstrapJs.Include("~/Scripts/bootstrap.js");
            bootstrapJs.Include("~/Scripts/jquery.unobtrusive-ajax.js");
            bootstrapJs.Include("~/Scripts/jquery.validate.js");
            bootstrapJs.Include("~/Scripts/jquery.validate.unobtrusive.js");
            bootstrapJs.Include("~/Scripts/knockout.js");
            bootstrapJs.Include("~/Scripts/modernizr-2.5.3.js");
            bootstrapJs.Include("~/Scripts/site.js");

            bundles.Add(bootstrapJs);
        }

        public static void EnableRaphaelBundles(this BundleCollection bundles)
        {

            var raphaelJs = new Bundle("~/raphael/js");
            raphaelJs.Include("~/Scripts/raphael-min.js");
            raphaelJs.Include("~/Scripts/g.raphael-min.js");
            raphaelJs.Include("~/Scripts/g.line-min.js");

            bundles.Add(raphaelJs);
        }
    }
}