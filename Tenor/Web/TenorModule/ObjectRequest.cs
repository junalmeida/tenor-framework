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
using System.Threading;

namespace Tenor.Web
{
    public partial class TenorModule
    {


        private void ResponseObjectRequest(HttpApplication app)
        {
            //IResponseObject

            object messagesLock = new object();
            CacheData obj;
            lock (messagesLock)
            {
                obj = (CacheData)(app.Context.Cache.Get(Tenor.Configuration.TenorModuleSection.IdPrefix + QueryString("id")));
            }

            if (obj == null)
            {
                //404 - File Not Found

                throw (new TenorException("Object not found"));

                /*return;*/

            }
            else if ((obj.Object as Stream) != null)
            {
                Stream memres = (Stream)obj.Object;
                //Write headers and the stream to output.
                WriteHeaders(app, obj);
                WriteStream(memres, app);

                return;

            }
            else if ((obj.Object as IResponseObject) != null)
            {
                //We found an invalid obj.Object 
                //WTF? This should never happen.

                //lets clear the cache.
                lock (messagesLock)
                {
                    app.Context.Cache.Remove(Tenor.Configuration.TenorModuleSection.IdPrefix + QueryString("id"));

                    app.Context.ClearError();
                    app.Context.AddError(new HttpException(500, "server error", new InvalidCastException()));
                    return;
                }
            }
            else
            {
                //We found a IResponseObject on cache, lets get a Stream


                lock (messagesLock)
                {

                    app.Context.Cache.Remove(Tenor.Configuration.TenorModuleSection.IdPrefix + QueryString("id"));

                    Web.IResponseObject rObj = (IResponseObject)obj.Object;
                    app.Response.Clear();
                    Stream memres;
                    //Calls WriteContent to get a stream
                    memres = rObj.WriteContent();

                    if (memres == null || memres.Length <= 0)
                    {
                        //We got an invalid stream
                        app.Context.AddError(new HttpException(404, "file not found"));
                        return; //404
                    }
                    //check mime type bases on file content.
                    obj.ContentType = GetMimeType(memres);
                    if (string.IsNullOrEmpty(obj.ContentType))
                    {
                        //get mime type from the object
                        obj.ContentType = rObj.ContentType;
                    }

                    //write headers and stream
                    WriteHeaders(app, obj);
                    WriteStream(memres, app);

                    //keeps the data on cache.
                    obj.Object = memres;
                    app.Context.Cache.Insert(Tenor.Configuration.TenorModuleSection.IdPrefix + QueryString("id"), obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, obj.Expires));
                    /*app.Context.Cache.Insert(IdPrefix & app.Request.QueryString("id"), obj, Nothing, Caching.Cache.NoAbsoluteExpiration, New TimeSpan(0, 0, obj.Expires), Caching.CacheItemPriority.Normal, AddressOf Cache_onItemRemoved)*/
                    return;
                }
            }
        }

    }
}
