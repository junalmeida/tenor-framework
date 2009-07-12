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
        /// <summary>
        /// Register a control to be called later by this module.
        /// </summary>
        /// <param name="control">The desired control.</param>
        /// <param name="expires">Time in seconds that the url will expire.</param>
        /// <returns>A string with the uri to complete this request.</returns>
        /// <remarks>The control must implement the <see cref="Tenor.Web.IResponseObject">IResponseObject</see> interface.</remarks>
        public static string RegisterControlForRequest(System.Web.UI.Control control, int expires)
        {
            return RegisterObjectForRequest(control, expires);
        }

        /// <summary>
        /// Register a control to be called later by this module.
        /// </summary>
        /// <param name="control">The desired control.</param>
        /// <returns>A string with the uri to complete this request.</returns>
        /// <remarks>The control must implement the <see cref="Tenor.Web.IResponseObject">IResponseObject</see> interface.</remarks>
        public static string RegisterControlForRequest(System.Web.UI.Control control)
        {
            return RegisterObjectForRequest(control);
        }

        /// <summary>
        /// Register an object to be called later by this module.
        /// </summary>
        /// <param name="object">The desired object.</param>
        /// <param name="expires">Time in seconds that the url will expire.</param>
        /// <param name="forceDownload">If true, the client browser will download the file instead of trying to show up.</param>
        /// <param name="fileName">Sets the file name shown by client browser.</param>
        /// <returns>A string with the uri to complete this request.</returns>
        /// <remarks>The object must implement the <see cref="Tenor.Web.IResponseObject">IResponseObject</see> interface.</remarks>
        public static string RegisterObjectForRequest(object @object, int expires, bool forceDownload, string fileName)
        {
            CheckHttpModule();
            HttpContext context = HttpContext.Current;
            if (context == null)
                throw new InvalidContextException();

            if (@object as IResponseObject == null)
            {
                throw (new InvalidCastException("This instance must implement \'" + typeof(IResponseObject).FullName + "\' interface."));
            }

            object messagesLock = new object();
            lock (messagesLock)
            {


                string sControlName = System.Guid.NewGuid().ToString();
                //Page.Application(HttpModule.IdPrefix & sControlName) = [Object]
                Dados dados = new Dados();
                dados.Object = @object;
                dados.Expires = expires;
                dados.FileName = fileName;
                dados.ForceDownload = forceDownload;

                context.Cache.Insert(Tenor.Configuration.TenorModule.IdPrefix + sControlName, dados, null, DateTime.UtcNow.AddMinutes(2), System.Web.Caching.Cache.NoSlidingExpiration);

                string uri = Tenor.Configuration.TenorModule.HandlerFileName + "?id=" + sControlName;

                if (context.Request.ApplicationPath.EndsWith("/"))
                {
                    uri = context.Request.ApplicationPath + uri;
                }
                else
                {
                    uri = context.Request.ApplicationPath + "/" + uri;
                }
                return uri;

            }

        }

        /// <summary>
        /// Register an object to be called later by this module.
        /// </summary>
        /// <param name="object">The desired object.</param>
        /// <returns>A string with the uri to complete this request.</returns>
        /// <remarks>The object must implement the <see cref="Tenor.Web.IResponseObject">IResponseObject</see> interface.</remarks>
        public static string RegisterObjectForRequest(object @object)
        {
            return RegisterObjectForRequest(@object, Tenor.Configuration.TenorModule.DefaultExpiresTime, false, null);
        }

        /// <summary>
        /// Register an object to be called later by this module.
        /// </summary>
        /// <param name="object">The desired object.</param>
        /// <param name="expires">Time in seconds that the url will expire.</param>
        /// <returns>A string with the uri to complete this request.</returns>
        /// <remarks>The object must implement the <see cref="Tenor.Web.IResponseObject">IResponseObject</see> interface.</remarks>
        public static string RegisterObjectForRequest(object @object, int expires)
        {
            return RegisterObjectForRequest(@object, expires, false, null);
        }
    }
}