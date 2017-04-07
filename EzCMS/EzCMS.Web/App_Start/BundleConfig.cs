using System.Web.Optimization;

namespace EzCMS.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Plugin styles
            bundles.Add(new StyleBundle("~/styles/plugins")
                .Include("~/Content/Plugins/Bootstrap/bootstrap.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/Plugins/FontAwesome/css/font-awesome.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/Plugins/jqueryui/jquery-ui.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/Plugins/colorbox.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/Plugins/Multiselect/Select2/select2.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/Plugins/Multiselect/BootstrapDuallistbox/bootstrap-duallistbox.min.css",
                    "~/Content/Plugins/jquery.gritter.min.css")
                .Include("~/Content/Plugins/fancybox/jquery.fancybox.css", new CssRewriteUrlTransform())
                .Include("~/Content/Plugins/Bootstrap/bootstrap-editable.css",
                    "~/Scripts/Plugins/Datetimepicker/css/bootstrap-datetimepicker.min.css"));

            //Ace styles
            bundles.Add(new StyleBundle("~/styles/ace")
                .Include("~/Content/Plugins/Ace/ace-fonts.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/Plugins/Ace/ace.min.css",
                        "~/Content/Plugins/Ace/ace-rtl.min.css",
                        "~/Content/Plugins/Ace/ace-skins.min.css"));

            //Ace styles
            bundles.Add(new StyleBundle("~/styles/mediabrowser")
                .Include("~/Content/Media/fileuploader.css",
                    "~/Content/Media/mediaBrowser.css"));

            //Code Mirror
            bundles.Add(new StyleBundle("~/styles/codemirror").Include(
                        "~/Scripts/Plugins/CodeMirror/codemirror.css",
                        "~/Scripts/Plugins/CodeMirror/addon/fold/foldgutter.css",
                        "~/Scripts/Plugins/CodeMirror/addon/hint/show-hint.css",
                        "~/Scripts/Plugins/CodeMirror/addon/display/fullscreen.css",
                        "~/Scripts/Plugins/CodeMirror/theme/pastel-on-dark.css",
                        "~/Scripts/Plugins/CodeMirror/theme/eclipse.css"));

            //Plugin scripts
            bundles.Add(new ScriptBundle("~/js/plugins").Include(
                        "~/Scripts/Plugins/x-editable/bootstrap-editable.min.js",
                        "~/Scripts/Plugins/x-editable/ace-editable.min.js",
                        "~/Scripts/Plugins/BootBox/bootbox.min.js",
                        "~/Scripts/Plugins/typeahead-bs2.min.js",
                        "~/Scripts/Plugins/colorbox.js",
                        "~/Scripts/Plugins/moment/moment.min.js",
                        "~/Scripts/Plugins/moment/moment-timezone-with-data.min.js",
                        "~/Scripts/Plugins/jquery.maskedinput.min.js",
                        "~/Scripts/Plugins/fancybox/jquery.fancybox.js",
                        "~/Scripts/Plugins/Datetimepicker/bootstrap-datetimepicker.min.js",
                        "~/Scripts/Plugins/Bootstrap/bootstrap-datepicker.js",
                        "~/Scripts/Plugins/Multiselect/Select2/select2.min.js",
                        "~/Scripts/Plugins/Multiselect/BootstrapDuallistbox/jquery.bootstrap-duallistbox.min.js",
                        "~/Scripts/Plugins/jquery.cookie.js",
                        "~/Scripts/Plugins/Timezone/jstz.js",
                        "~/Scripts/Plugins/spin.min.js",
                        "~/Scripts/Plugins/jquery.gritter.min.js",
                        "~/Scripts/Plugins/JavascriptToolbox/date.js"));


            //Ace scripts
            bundles.Add(new ScriptBundle("~/js/ace").Include(
                        "~/Scripts/Plugins/Ace/ace-elements.min.js",
                        "~/Scripts/Plugins/Ace/ace.min.js"));

            //Jquery validation
            bundles.Add(new ScriptBundle("~/js/validation").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            //Jquery validation
            bundles.Add(new ScriptBundle("~/js/validation").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            //Jquery validation
            bundles.Add(new ScriptBundle("~/js/mediabrowser").Include(
                        "~/Scripts/Media/fileuploader.js",
                        "~/Scripts/Media/jquery.hotkeys.js",
                        "~/Scripts/Media/jquery.jstree.js"));

            //Code Mirror
            bundles.Add(new ScriptBundle("~/js/codemirror").Include(
                        "~/Scripts/Plugins/CodeMirror/codemirror.js",
                        "~/Scripts/Plugins/CodeMirror/addon/fold/foldcode.js",
                        "~/Scripts/Plugins/CodeMirror/addon/fold/foldgutter.js",
                        "~/Scripts/Plugins/CodeMirror/addon/fold/xml-fold.js",
                        "~/Scripts/Plugins/CodeMirror/addon/edit/matchtags.js",
                        "~/Scripts/Plugins/CodeMirror/mode/javascript/javascript.js",
                        "~/Scripts/Plugins/CodeMirror/mode/xml/xml.js",
                        "~/Scripts/Plugins/CodeMirror/mode/css/css.js",
                        "~/Scripts/Plugins/CodeMirror/mode/htmlmixed/htmlmixed.js",
                        "~/Scripts/Plugins/CodeMirror/display/fullscreen.js"));

            //Jquery validation
            bundles.Add(new ScriptBundle("~/js/administrator").Include(
                "~/Scripts/Shared/Initialize/validation.init.js",
                "~/Scripts/Shared/Initialize/admin.init.js",
                "~/Scripts/Shared/Helpers/siteHelper.js"));

            //CKeditor
            bundles.Add(new ScriptBundle("~/js/ckeditor").Include(
                "~/Scripts/Plugins/ckeditor/ckeditor.js",
                "~/Scripts/Shared/Initialize/ckeditor.init.js"));

            //jqGrid
            bundles.Add(new ScriptBundle("~/js/jqgrid").Include(
                        "~/Scripts/Plugins/jqGrid/jquery.jqGrid.min.js",
                        "~/Scripts/Plugins/jqGrid/i18n/grid.locale-en.js",
                        "~/Scripts/Plugins/jqGrid/jquery.jqGrid.showHideColumnMenu.js",
                        "~/Scripts/Shared/Helpers/jqGridHelper.js"));

#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}