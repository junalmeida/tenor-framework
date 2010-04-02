using System.Diagnostics;
using System.Collections;
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
        private const string BasePath = AssemblyWebUIWebControls + ".";
        public const string JsWindow = BasePath + "Window.Window.js";

        public const string WindowCssHoverHtc = BasePath + "Window.csshover.htc";
        public const string WindowBottomLeftGif = BasePath + "Window.janela.bottom-left.gif";
        public const string WindowBottomRightGif = BasePath + "Window.janela.bottom-right.gif";
        public const string WindowBottomGif = BasePath + "Window.janela.bottom.gif";
        public const string WindowCloseUpGif = BasePath + "Window.janela.close-up.gif";
        public const string WindowCloseGif = BasePath + "Window.janela.close.gif";
        public const string WindowLeftGif = BasePath + "Window.janela.left.gif";
        public const string WindowRightGif = BasePath + "Window.janela.right.gif";
        public const string WindowTitleLeftGif = BasePath + "Window.janela.title-left.gif";
        public const string WindowTitleRightGif = BasePath + "Window.janela.title-right.gif";
        public const string WindowTitleGif = BasePath + "Window.janela.title.gif";



        public const string JsCalendar = BasePath + "TextBox.Calendar.calendar.js";
        public const string JsCalendarEn = BasePath + "TextBox.Calendar.calendar-en.js";
        public const string JsCalendarBr = BasePath + "TextBox.Calendar.calendar-br.js";
        public const string JsCalendarSetup = BasePath + "TextBox.Calendar.calendar-setup.js";

        public const string CalendarCalDJpeg = BasePath + "TextBox.Calendar.XPStyle.cal-d.jpg";
        public const string CalendarCalHJpeg = BasePath + "TextBox.Calendar.XPStyle.cal-h.jpg";
        public const string CalendarCalNJpeg = BasePath + "TextBox.Calendar.XPStyle.cal-n.jpg";
        public const string CalendarWin2kCss = BasePath + "TextBox.Calendar.W2KStyle.calendar-win2k-2.css";
        public const string CalendarSystemCss = BasePath + "TextBox.Calendar.calendar-system.css";


        public const string AssemblyWebUIWebControls = "Tenor.Web.UI.WebControls";
        public const string AssemblyTinyMCE = "Tenor.TinyMCE";

        public const string JsScrollPanel = BasePath + "ScrollPanel.scrollpanel.js";
        public const string JsSlidingPanel = BasePath + "SlidingPanel.slidingpanel.js";
        public const string JsSortable = BasePath + "SortableBulletedList.sortable.js";


        public const string JsDropdown = BasePath + "ActionDropDown.dropdown.js";
        public const string JsMasks = BasePath + "TextBox.masks.js";
        public const string JsMasks2 = BasePath + "TextBox.masks2.js";
        public const string JsJQuery = BasePath + "TextBox.jQuery.js";
        public const string JsRightClick = BasePath + "ScriptManager.rightclick.js";
        public const string JsSelect = BasePath + "ScriptManager.select.js";
        public const string JsTracking = "Tenor.tracking.js";
        public const string JsIe7Core = BasePath + "ScriptManager.iefix.ie7-core.js";
        public const string JsIe7CSS2 = BasePath + "ScriptManager.iefix.ie7-css2-selectors.js";

        public const string JsAdapterUtils = BasePath + "Adapters.AdapterUtils.js";

        public const string JsCheckBoxList = BasePath + "CheckBox.checkboxlist.js";

        public const string JsContextMenoo = BasePath + "ContextMenu.contextMenoo.js";
        public const string JsContextMenooSrc = BasePath + "ContextMenu.contextMenoo-src.js";


        public const string FlashBmp = BasePath + "Flash.flash.bmp";
        public const string FlashJpg = BasePath + "Flash.flash.jpg";
        public const string JsFlash = BasePath + "Flash.flash.js";
        public const string JsAC_OETags = BasePath + "Flash.AC_OETags.js";


        public const string JsFloatingPanel = BasePath + "FloatingPanel.floatingpanel.js";

        public const string JsPreviewImage = BasePath + "PreviewImage.previewimage.js";
        public const string PreviewBlackWhiteGif = BasePath + "PreviewImage.black-white.gif";
        public const string PreviewWhiteBlackGif = BasePath + "PreviewImage.white-black.gif";
        public const string PreviewGrayGif = BasePath + "PreviewImage.gray.gif";
        public const string PreviewPrevCloseGif = BasePath + "PreviewImage.prev-close.jpg";


        public const string JsReorderButton = BasePath + "ReorderButton.reorderbutton.js";
        public const string JsResizablePanels = BasePath + "ResizablePanels.resizablepanels.js";


        public const string JsMooTools1_11_Full = BasePath + "ScriptManager.mootools.mootools-release-1.11-Full.js";
        public const string JsMooTools1_11_Core = BasePath + "ScriptManager.mootools.mootools-release-1.11-Core.js";
        public const string JsMooTools1_11_CoreTips = BasePath + "ScriptManager.mootools.mootools-release-1.11-CoreTips.js";
        public const string JsMooTools1_11_CoreSortables = BasePath + "ScriptManager.mootools.mootools-release-1.11-CoreSortables.js";
        public const string JsMooTools1_11_CoreSmoothscroll = BasePath + "ScriptManager.mootools.mootools-release-1.11-CoreSmoothscroll.js";
        public const string JsMooTools1_11_CoreRemote = BasePath + "ScriptManager.mootools.mootools-release-1.11-CoreRemote.js";
        public const string JsMooTools1_11_CoreEffectsDrag = BasePath + "ScriptManager.mootools.mootools-release-1.11-CoreEffectsDrag.js";
        public const string JsMooTools1_11_SortableOrder = BasePath + "ScriptManager.mootools.sortableorder.js";
        public const string JsMooTools1_11_SortableOrderSrc = BasePath + "ScriptManager.mootools.sortableorder-src.js";
        public const string JsMooSlimbox = BasePath + "ScriptManager.mootools.slimbox.js";

        public const string JsMooSqueezebox = BasePath + "Tenor.squeezebox.js";
        public const string JsMooSqueezeboxSrc = BasePath + "Tenor.squeezebox-src.js";
        public const string MooSqueezeboxCss = BasePath + "Tenor.squeezebox.css";
        public const string MooSqueezeboxClosebox = BasePath + "Tenor.closebox.png";
        public const string MooSqueezeboxSpinner = BasePath + "Tenor.spinner.png";


        public const string MooRoot111 = BasePath + "ScriptManager.mootools.mootools-release-1.11-";
        public const string MooRoot12 = BasePath + "ScriptManager.mootools.mootools-1.2-";

        public const string JsMooTools1_2_Full = BasePath + "ScriptManager.mootools.mootools-1.2-Full.js";
        public const string JsMooTools1_2_Core = BasePath + "ScriptManager.mootools.mootools-1.2-Core.js";

    }
}