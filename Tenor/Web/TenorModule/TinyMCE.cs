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


        private void TinyMCERequest(HttpApplication app)
        {
            //Loads TinyMCE assembly.
            Assembly tinymce = @Assembly.Load(new System.Reflection.AssemblyName(Configuration.Resources.AssemblyTinyMCE));
            if (tinymce == null)
            {
                return;
            }

            //get the mimetype from file extension
            string contenttype = IO.BinaryFile.GetContentType(app.Request.Path);
            //change pathSeparator to underscore (/ -> _) to load Embbeded Resources.
            string filepath = app.Request.Path.Substring(app.Request.Path.ToLower().IndexOf("/tiny_mce/")).Replace("/", "_");

            UnmanagedMemoryStream file = (UnmanagedMemoryStream)(tinymce.GetManifestResourceStream(Configuration.Resources.AssemblyRoot + "." + filepath));
            if (file == null || file.Length == 0)
            {
                return; //404
            }
            CacheData cacheData = new CacheData();

            if (string.IsNullOrEmpty(contenttype))
            {
                cacheData.ContentType = GetMimeType(file);
                if (string.IsNullOrEmpty(app.Response.ContentType))
                {
                    cacheData.ContentType = "text/plain";
                }
            }
            else
            {
                cacheData.ContentType = contenttype;
            }
            WriteHeaders(app, cacheData);

            WriteStream(file, app);
            file.Close();

        }

    }
}