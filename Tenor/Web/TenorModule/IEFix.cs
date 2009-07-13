using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Runtime.InteropServices;

namespace Tenor.Web
{
    public partial class TenorModule
    {
        private void IEFixRequest(HttpApplication app)
        {
            //IEFix

            //Load iefix assembly:
            Assembly webUI = @Assembly.Load(new System.Reflection.AssemblyName(Configuration.Resources.AssemblyWebUI));
            if (webUI == null)
            {
                return;
            }

            //get the mimetype from file extension
            string contentType = IO.BinaryFile.GetContentType(app.Request.Path);

            string basePath = "/iefix/";

            string filePath = app.Request.Path.Substring(app.Request.Path.ToLower().IndexOf(basePath) + basePath.Length);

            UnmanagedMemoryStream file = (UnmanagedMemoryStream)(webUI.GetManifestResourceStream(Configuration.Resources.AssemblyRoot + "." + filePath));
            if (file == null || file.Length == 0)
            {
                return; //404
            }
            CacheData data = new CacheData();

            if (string.IsNullOrEmpty(contentType))
            {
                data.ContentType = GetMimeType(file);
                if (string.IsNullOrEmpty(app.Response.ContentType))
                {
                    data.ContentType = "text/plain";
                }
            }
            else
            {
                data.ContentType = contentType;
            }
            WriteHeaders(app, data);

            WriteStream(file, app);
            file.Close();
        }




    }
}
