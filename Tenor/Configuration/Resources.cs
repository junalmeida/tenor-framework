using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;

//using System.Web.UI;
//[assembly:WebResource(Configuration.Resources.ChartBarExample, "image/png")]
//[assembly:WebResource(Configuration.Resources.ChartPieExample, "image/png")]
//[assembly:WebResource(Configuration.Resources.GaugeSimplePNG, "image/png")]
//[assembly:WebResource(Configuration.Resources.GaugeSimpleGIF, "image/gif")]

namespace Tenor.Configuration
{

    internal class Resources
    {

        public const string MimeXML = "Tenor.Web.mime.xml";

        public const string AssemblyRoot = "Tenor";

        public const string AssemblyWebUI = "Tenor.Web.UI";
        public const string AssemblyTinyMCE = "Tenor.TinyMCE";
        public const string ScriptManagerClass = "Tenor.Web.UI.WebControls.ScriptManager";

        public const string ChartBmp = "Tenor.Chart.bmp";
        public const string ChartBarExample = "Tenor.barexample.PNG";
        public const string ChartPieExample = "Tenor.pieexample.PNG";

        public const string GaugeBmp = "Tenor.Gauge.bmp";
        public const string GaugeSimplePNG = "Tenor.simplegauge.png";
        public const string GaugeSimpleGIF = "Tenor.simplegauge.gif";


    }
}