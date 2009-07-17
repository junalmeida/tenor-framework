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
/*using System.Web.SessionState;*/
using System.Web.Configuration;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;


namespace Tenor.Web
{

    /// <summary>
    /// The TenorModule is an HttpModule to handle database, graphical and other http requests.
    /// </summary>
    public sealed partial class TenorModule : IHttpModule, System.Web.SessionState.IReadOnlySessionState
    {
        
        #region " Hack - Session State "

        private class MyHttpHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
        {


            internal readonly IHttpHandler originalHandler;
            public MyHttpHandler(IHttpHandler originalHandler)
            {
                this.originalHandler = originalHandler;
            }

            public bool IsReusable
            {
                get
                {
                    return false;
                }
            }

            public void ProcessRequest(System.Web.HttpContext context)
            {
                throw (new InvalidOperationException("MyHttpHandler cannot process requests."));

            }
        }

        private void Application_PostAcquireRequestState(object sender, EventArgs e)
        {

            HttpApplication app = (HttpApplication)sender;
            if (app.Context.Request.RawUrl.ToLower().Contains(Configuration.TenorModule.HandlerFileName.ToLower()))
            {

                MyHttpHandler resourceHttpHandler = HttpContext.Current.Handler as MyHttpHandler;



                if (resourceHttpHandler != null)
                {
                    HttpContext.Current.Handler = resourceHttpHandler.originalHandler;
                }
                /*System.Diagnostics.Debug.Assert(app.Session == null, "oops, it did not work :(");*/

            }
        }

        private void Application_PostMapRequestHandler(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            if (app.Context.Request.RawUrl.ToLower().Contains(Configuration.TenorModule.HandlerFileName.ToLower()))
            {

                if (app.Context.Handler is System.Web.SessionState.IReadOnlySessionState || app.Context.Handler is System.Web.SessionState.IRequiresSessionState)
                {
                    return;
                }
                app.Context.Handler = new MyHttpHandler(app.Context.Handler);
            }
        }
        #endregion
        



        /// <summary>
        /// This is a workaround to HtmlEncode bug that we don't know why it happens.
        /// </summary>
        private string QueryString(string Key)
        {
            HttpRequest Request = HttpContext.Current.Request;

            string res = Request.QueryString["amp;" + Key];
            if (string.IsNullOrEmpty(res))
            {
                res = Request.QueryString[Key];
            }
            if (string.IsNullOrEmpty(res))
            {
                return string.Empty;
            }
            else
            {
                return res;
            }
        }




        void IHttpModule.Dispose()
        {

        }


        void IHttpModule.Init(System.Web.HttpApplication app)
        {
            //-- Session Hack.
            //will be called before AcquireRequestState
            app.PostMapRequestHandler += new System.EventHandler(Application_PostMapRequestHandler);
            //will be called after AcquireRequestState
            app.PostAcquireRequestState += new System.EventHandler(Application_PostAcquireRequestState);
            //---

            app.AcquireRequestState += new System.EventHandler(AcquireRequestState);
            app.Error += new System.EventHandler(Application_Error);
        }

        /// <summary>
        /// <para>
        /// This method is called on each server request. Everything starts here.
        /// This module have three sections:</para>
        /// <list type="square">
        /// <item>
        /// 1: TinyMCE, IEFix
        ///   The requested file of the javascript component will be processed by this section.
        /// </item>
        /// <item>
        /// 2: IResponseObject 
        ///   The file or image is being requested by the client. If this is the first call to this object, it will not be on the cache and WriteContent will be called. Otherwised, the cache instance will be provided.
        /// </item>
        /// <item>
        /// 3: InstanceRequest
        ///   Here, an instance of the desired class will be created using the provided parameter. 
        ///   The class must implement IResponseObject or declare a Property with a ResponsePropertyAttribute defined.
        /// </item>
        /// </list>
        /// </summary>
        void AcquireRequestState(object sender, EventArgs e)
        {

            HttpApplication app = (HttpApplication)sender;
            string path = app.Request.Path.ToLower();
            if (path.Contains(Tenor.Configuration.TenorModule.HandlerFileName.ToLower() + "/tiny_mce/"))
            {
                //TinyMCE
                //See TinyMCE.cs
                TinyMCERequest(app);
            }
            else if (path.Contains(Tenor.Configuration.TenorModule.HandlerFileName.ToLower() + "/iefix/"))
            {
                //IEFix
                //See IEFix.cs
                IEFixRequest(app);
            }
            else if (path.Contains(Tenor.Configuration.TenorModule.HandlerFileName.ToLower()))
            {
                if (!string.IsNullOrEmpty(QueryString("id")))
                {
                    //If we have a GUID, tries ResponseObjectRequest:
                    //See ObjectRequest.cs
                    ResponseObjectRequest(app);
                }
                else if (!string.IsNullOrEmpty(QueryString("c")))
                {
                    //TODO: Check if we really need this component.
                    //If we have a 'c' (class) tries ChartRequest
                    //See Chart.cs
                    /*ChartRequest(app);*/
                }
                else if (!string.IsNullOrEmpty(QueryString("cl")))
                {
                    //If we have a CL, tries IntanceRequest.
                    //See InstanceRequest.cs
                    InstanceRequest(app);
                }
                else if (!string.IsNullOrEmpty(QueryString("captcha")))
                {
                    //If we have captcha, tries CaptchaRequest.
                    if (string.IsNullOrEmpty(QueryString("audio")))
                    {
                        //Image captcha request
                        CaptchaRequest(QueryString("captcha"));
                    }
                    else
                    {
                        //Audio captcha request
                        CaptchaAudioRequest(QueryString("captcha"));
                    }
                    //See Captcha.cs
                }
                else if (!string.IsNullOrEmpty(QueryString("clear")))
                {
                    //clear cache
                    //see Cache.cs
                    ClearCache(app, QueryString("clear"), QueryString("p1"), true);
                }
                else if (!string.IsNullOrEmpty(QueryString("bt")))
                {
                    //Placeholder Image
                    //See DynamicImageButton.cs
                    this.DynamicImageRequest(app, QueryString("bt"));

                }

            }
        }


        /// <summary>
        /// Write document headers to the client.
        /// </summary>
        private void WriteHeaders(HttpApplication app, CacheData dados)
        {
            app.Response.ContentType = dados.ContentType;
            app.Response.AddHeader("Content-Length", dados.ContentLength.ToString());

            string header = "";
            if (dados.ForceDownload)
            {
                //defines a header to force download
                header += "attachment; ";
            }
            if (!string.IsNullOrEmpty(dados.FileName))
            {
                //defines a header to set a filename.
                header += " filename=\"" + dados.FileName + "\"";
            }
            else
            {
                //defines a header to set a default filename.
                header += " filename=\"file." + GetExtension(dados.ContentType) + "\"";

            }

            if (header.Length > 0)
            {
                app.Response.AddHeader("content-disposition", header);
            }

            //sets cache parameters.
            if (app.Request[Tenor.Configuration.TenorModule.NoCache] != string.Empty)
            {
                app.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            }
            else
            {
                app.Response.Cache.SetLastModified(dados.LastModified);
                app.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                app.Response.Cache.SetCacheability(HttpCacheability.Public);
                app.Response.Cache.SetSlidingExpiration(true);
                app.Response.Cache.SetExpires(DateTime.Now.AddSeconds(dados.Expires));
                app.Response.Cache.SetETag(dados.ETag);
            }
        }



        /// <summary>
        /// Writes the data to the Output Stream.
        /// </summary>
        /// <param name="stream">The stream with data.</param>
        /// <param name="app">The HttpApplication.</param>
        private void WriteStream(Stream stream, HttpApplication app)
        {
            app.Context.ClearError();
            app.Response.Clear();
            //disabling output buffer.
            app.Response.BufferOutput = false;


            MemoryStream mem = stream as MemoryStream;
            if (mem != null)
            {
                //using writeTo of the memory stream.
                mem.WriteTo(app.Response.OutputStream);
            }
            else if (stream.CanRead && stream.Length > 0)
            {
                //TODO: improve reading and writing code.
                byte[] data = IO.BinaryFile.StreamToBytes(stream);
                app.Response.OutputStream.Write(data, 0, data.Length);
            }
            else
            {
                throw (new IOException("Invalid stream."));
            }
        }


        /// <summary>
        /// This is an internal DTO to keep cache data of this module.
        /// </summary>
        private class CacheData
        {

            public CacheData()
            {
                LastModified = DateTime.Now;
            }

            /// <summary>
            /// Stores an instance of IResponsObject that will be called to generate the stream on first call.
            /// </summary>
            public object @Object;

            /// <summary>
            /// The time in seconds that this object will expires.
            /// </summary>
            public int Expires;

            /// <summary>
            /// If true, tries to force the web browser to download data intead of showing it.
            /// </summary>
            public bool ForceDownload;

            /// <summary>
            /// Defines the file name to send to the web browser.
            /// </summary>
            public string FileName;

            /// <summary>
            /// Defines the mime type of this file.
            /// </summary>
            public string ContentType;

            /// <summary>
            /// Defines the content lenth in bytes.
            /// </summary>
            public long ContentLength;

            /// <summary>
            /// Defines the file modificated date.
            /// </summary>
            public DateTime LastModified;

            /// <summary>
            /// The ETag. An ETag (entity tag) is an HTTP response header returned by an HTTP/1.1 compliant web server used to determine change in content at a given URL.
            /// When a new HTTP response contains the same ETag as an older HTTP response, the contents are considered to be the same without further downloading.
            /// </summary>
            public string ETag
            {
                get
                {
                    //TODO: Check if this code is ok.

                    StringBuilder builder = new StringBuilder();
                    long num = DateTime.Now.ToFileTime();
                    long num2 = LastModified.ToFileTime();
                    builder.Append("\"");
                    builder.Append(num2.ToString("X8", CultureInfo.InvariantCulture));
                    builder.Append(":");
                    builder.Append(num.ToString("X8", CultureInfo.InvariantCulture));
                    builder.Append("\"");
                    if ((DateTime.Now.ToFileTime() - num2) <= 0x1C9C380)
                    {
                        return ("W/" + builder.ToString());
                    }
                    return builder.ToString();

                }
            }
        }

    }
}