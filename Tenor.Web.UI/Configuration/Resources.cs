using System.Diagnostics;
using System.Collections;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System;
using System.Web.UI;


[assembly:WebResource(Tenor.Configuration.Resources.JsAdapterUtils, "text/javascript")]


[assembly:WebResource(Tenor.Configuration.Resources.JsCheckBoxList, "text/javascript")]


[assembly:WebResource(Tenor.Configuration.Resources.JsContextMenoo, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsContextMenooSrc, "text/javascript")]



[assembly:WebResource(Tenor.Configuration.Resources.FlashJpg, "image/jpeg")]
[assembly:WebResource(Tenor.Configuration.Resources.JsFlash, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsAC_OETags, "text/javascript")]



[assembly:WebResource(Tenor.Configuration.Resources.JsFloatingPanel, "text/javascript")]



[assembly:WebResource(Tenor.Configuration.Resources.JsPreviewImage, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.PreviewBlackWhiteGif, "image/gif")]
[assembly:WebResource(Tenor.Configuration.Resources.PreviewWhiteBlackGif, "image/gif")]
[assembly:WebResource(Tenor.Configuration.Resources.PreviewGearGif, "image/gif")]
[assembly:WebResource(Tenor.Configuration.Resources.PreviewPrevCloseGif, "image/jpeg")]
[assembly:WebResource(Tenor.Configuration.Resources.PreviewGrayGif, "image/gif")]


[assembly:WebResource(Tenor.Configuration.Resources.JsReorderButton, "text/javascript")]


[assembly:System.Web.UI.WebResource(Tenor.Configuration.Resources.JsResizablePanels, "text/javascript")]


[assembly:WebResource(Tenor.Configuration.Resources.JsMooTools1_11_Full, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsMooTools1_11_Core, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsMooTools1_11_CoreTips, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsMooTools1_11_CoreSortables, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsMooTools1_11_CoreSmoothscroll, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsMooTools1_11_CoreRemote, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsMooTools1_11_CoreEffectsDrag, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsMooTools1_11_SortableOrder, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsMooTools1_11_SortableOrderSrc, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsMooSlimbox, "text/javascript")]

[assembly:WebResource(Tenor.Configuration.Resources.JsMooSqueezebox, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsMooSqueezeboxSrc, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.MooSqueezeboxCss, "text/css")]
[assembly:WebResource(Tenor.Configuration.Resources.MooSqueezeboxClosebox, "image/png")]
[assembly:WebResource(Tenor.Configuration.Resources.MooSqueezeboxSpinner, "image/gif")]

[assembly:WebResource(Tenor.Configuration.Resources.JsMooTools1_2_Full, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsMooTools1_2_Core, "text/javascript")]

[assembly:WebResource(Tenor.Configuration.Resources.JsDropdown, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsMasks, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsMasks2, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsJQuery, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsRightClick, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsSelect, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsTracking, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsIe7Core, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsIe7CSS2, "text/javascript", PerformSubstitution=true)]


[assembly:WebResource(Tenor.Configuration.Resources.JsScrollPanel, "text/javascript")]

[assembly:WebResource(Tenor.Configuration.Resources.JsSlidingPanel, "text/javascript")]

[assembly:WebResource(Tenor.Configuration.Resources.JsSortable, "text/javascript")]


[assembly:WebResource(Tenor.Configuration.Resources.CalendarCalDJpeg, "image/jpeg")]
[assembly:WebResource(Tenor.Configuration.Resources.CalendarCalHJpeg, "image/jpeg")]
[assembly:WebResource(Tenor.Configuration.Resources.CalendarCalNJpeg, "image/jpeg")]
[assembly:WebResource(Tenor.Configuration.Resources.JsCalendar, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsCalendarEn, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsCalendarBr, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.JsCalendarSetup, "text/javascript")]
[assembly:WebResource(Tenor.Configuration.Resources.CalendarWin2kCss, "text/css")]
[assembly:WebResource(Tenor.Configuration.Resources.CalendarSystemCss, "text/css")]

[assembly:WebResource(Tenor.Configuration.Resources.WindowBottomLeftGif, "image/gif")]
[assembly:WebResource(Tenor.Configuration.Resources.WindowBottomRightGif, "image/gif")]
[assembly:WebResource(Tenor.Configuration.Resources.WindowBottomGif, "image/gif")]
[assembly:WebResource(Tenor.Configuration.Resources.WindowCloseUpGif, "image/gif")]
[assembly:WebResource(Tenor.Configuration.Resources.WindowCloseGif, "image/gif")]
[assembly:WebResource(Tenor.Configuration.Resources.WindowLeftGif, "image/gif")]
[assembly:WebResource(Tenor.Configuration.Resources.WindowRightGif, "image/gif")]
[assembly:WebResource(Tenor.Configuration.Resources.WindowTitleLeftGif, "image/gif")]
[assembly:WebResource(Tenor.Configuration.Resources.WindowTitleRightGif, "image/gif")]
[assembly:WebResource(Tenor.Configuration.Resources.WindowTitleGif, "image/gif")]

[assembly:WebResource(Tenor.Configuration.Resources.WindowCssHoverHtc, "text/x-component")]
[assembly:WebResource(Tenor.Configuration.Resources.JsWindow, "text/javascript")]


namespace Tenor.Configuration
{

    internal class Resources
    {

        public const string JsWindow = "Tenor.Window.js";

        public const string WindowCssHoverHtc = "Tenor.csshover.htc";
        public const string WindowBottomLeftGif = "Tenor.bottom-left.gif";
        public const string WindowBottomRightGif = "Tenor.bottom-right.gif";
        public const string WindowBottomGif = "Tenor.bottom.gif";
        public const string WindowCloseUpGif = "Tenor.close-up.gif";
        public const string WindowCloseGif = "Tenor.close.gif";
        public const string WindowLeftGif = "Tenor.left.gif";
        public const string WindowRightGif = "Tenor.right.gif";
        public const string WindowTitleLeftGif = "Tenor.title-left.gif";
        public const string WindowTitleRightGif = "Tenor.title-right.gif";
        public const string WindowTitleGif = "Tenor.title.gif";



        public const string JsCalendar = "Tenor.calendar.js";
        public const string JsCalendarEn = "Tenor.calendar-en.js";
        public const string JsCalendarBr = "Tenor.calendar-br.js";
        public const string JsCalendarSetup = "Tenor.calendar-setup.js";

        public const string CalendarCalDJpeg = "Tenor.cal-d.jpg";
        public const string CalendarCalHJpeg = "Tenor.cal-h.jpg";
        public const string CalendarCalNJpeg = "Tenor.cal-n.jpg";
        public const string CalendarWin2kCss = "Tenor.calendar-win2k-2.css";
        public const string CalendarSystemCss = "Tenor.calendar-system.css";


        public const string AssemblyWebUIWebControls = "Tenor.Web.UI.WebControls";
        public const string AssemblyTinyMCE = "Tenor.TinyMCE";

        public const string JsScrollPanel = "Tenor.scrollpanel.js";
        public const string JsSlidingPanel = "Tenor.slidingpanel.js";
        public const string JsSortable = "Tenor.sortable.js";


        public const string JsDropdown = "Tenor.dropdown.js";
        public const string JsMasks = "Tenor.masks.js";
        public const string JsMasks2 = "Tenor.masks2.js";
        public const string JsJQuery = "Tenor.jQuery.js";
        public const string JsRightClick = "Tenor.rightclick.js";
        public const string JsSelect = "Tenor.select.js";
        public const string JsTracking = "Tenor.tracking.js";
        public const string JsIe7Core = "Tenor.ie7-core.js";
        public const string JsIe7CSS2 = "Tenor.ie7-css2-selectors.js";

        public const string JsAdapterUtils = "Tenor.AdapterUtils.js";

        public const string JsCheckBoxList = "Tenor.checkboxlist.js";

        public const string JsContextMenoo = "Tenor.contextMenoo.js";
        public const string JsContextMenooSrc = "Tenor.contextMenoo-src.js";


        public const string FlashBmp = "Tenor.flash.bmp";
        public const string FlashJpg = "Tenor.flash.jpg";
        public const string JsFlash = "Tenor.flash.js";
        public const string JsAC_OETags = "Tenor.AC_OETags.js";


        public const string JsFloatingPanel = "Tenor.floatingpanel.js";

        public const string JsPreviewImage = "Tenor.previewimage.js";
        public const string PreviewBlackWhiteGif = "Tenor.black-white.gif";
        public const string PreviewWhiteBlackGif = "Tenor.white-black.gif";
        public const string PreviewGrayGif = "Tenor.gray.gif";
        public const string PreviewPrevCloseGif = "Tenor.prev-close.jpg";
        public const string PreviewGearGif = "Tenor.gear.gif";


        public const string JsReorderButton = "Tenor.reorderbutton.js";
        public const string JsResizablePanels = "Tenor.resizablepanels.js";


        public const string JsMooTools1_11_Full = "Tenor.mootools-release-1.11-Full.js";
        public const string JsMooTools1_11_Core = "Tenor.mootools-release-1.11-Core.js";
        public const string JsMooTools1_11_CoreTips = "Tenor.mootools-release-1.11-CoreTips.js";
        public const string JsMooTools1_11_CoreSortables = "Tenor.mootools-release-1.11-CoreSortables.js";
        public const string JsMooTools1_11_CoreSmoothscroll = "Tenor.mootools-release-1.11-CoreSmoothscroll.js";
        public const string JsMooTools1_11_CoreRemote = "Tenor.mootools-release-1.11-CoreRemote.js";
        public const string JsMooTools1_11_CoreEffectsDrag = "Tenor.mootools-release-1.11-CoreEffectsDrag.js";
        public const string JsMooTools1_11_SortableOrder = "Tenor.sortableorder.js";
        public const string JsMooTools1_11_SortableOrderSrc = "Tenor.sortableorder-src.js";
        public const string JsMooSlimbox = "Tenor.slimbox.js";

        public const string JsMooSqueezebox = "Tenor.squeezebox.js";
        public const string JsMooSqueezeboxSrc = "Tenor.squeezebox-src.js";
        public const string MooSqueezeboxCss = "Tenor.squeezebox.css";
        public const string MooSqueezeboxClosebox = "Tenor.closebox.png";
        public const string MooSqueezeboxSpinner = "Tenor.spinner.png";


        public const string MooRoot111 = "Tenor.mootools-release-1.11-";
        public const string MooRoot12 = "Tenor.mootools-1.2-";

        public const string JsMooTools1_2_Full = "Tenor.mootools-1.2-Full.js";
        public const string JsMooTools1_2_Core = "Tenor.mootools-1.2-Core.js";

    }
}