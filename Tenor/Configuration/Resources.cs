
//using System.Web.UI;
//[assembly:WebResource(Configuration.Resources.ChartBarExample, "image/png")]
//[assembly:WebResource(Configuration.Resources.ChartPieExample, "image/png")]
//[assembly:WebResource(Configuration.Resources.GaugeSimplePNG, "image/png")]
//[assembly:WebResource(Configuration.Resources.GaugeSimpleGIF, "image/gif")]

namespace Tenor.Configuration
{
    /// <summary>
    /// Represents all resource files attached on this assembly.
    /// </summary>
    internal class Resources
    {
        internal const string MimeXML = "Tenor.Web.mime.xml";

        internal const string AssemblyRoot = "Tenor";

        internal const string AssemblyWebUI = "Tenor.Web.UI";
        internal const string AssemblyTinyMCE = "Tenor.TinyMCE";
        internal const string ScriptManagerClass = "Tenor.Web.UI.WebControls.ScriptManager";

        internal const string ChartBmp = "Tenor.Chart.bmp";
        internal const string ChartBarExample = "Tenor.barexample.PNG";
        internal const string ChartPieExample = "Tenor.pieexample.PNG";

        internal const string GaugeBmp = "Tenor.Gauge.bmp";
        internal const string GaugeSimplePNG = "Tenor.simplegauge.png";
        internal const string GaugeSimpleGIF = "Tenor.simplegauge.gif";
    }
}