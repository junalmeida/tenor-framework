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
using Tenor.Drawing;

namespace Tenor.Web
{
    public partial class TenorModule
    {

        /// <summary>
        /// Creates a string with an url that will serve an instance base on a single integer parameter. 
        /// </summary>
        public static string GetInstanceUrl(Type type, int parameter)
        {
            return GetInstanceUrl(type, parameter, string.Empty, false, Drawing.ResizeMode.Stretch, 0, 0);
        }

        /// <summary>
        /// Creates a string with an url that will serve an instance base on a single integer parameter. 
        /// </summary>
        public static string GetInstanceUrl(Type type, int parameter, string fileName, bool forceDownload)
        {
            return GetInstanceUrl(type, parameter, fileName, forceDownload, Drawing.ResizeMode.Stretch, 0, 0);
        }

        /*
        [Obsolete]
        public static string GetInstanceUrl(HttpContext Context, Type Type, int Parameter)
        {
            return GetInstanceUrl(, Type, Parameter, string.Empty, false, Drawing.ResizeMode.Stretch, 0, 0);
        }

        [Obsolete]
        public static string GetInstanceUrl(HttpContext Context, Type Type, int Parameter, string FileName, bool Download)
        {
            return GetInstanceUrl(Context, Type, Parameter, FileName, Download, Drawing.ResizeMode.Stretch, 0, 0);
        }

        [Obsolete("Use GetInstanceUrl with resize mode.")]
        public static string GetInstanceUrl(HttpContext Context, Type Type, int Parameter, bool Crop, int Width, int Height)
        {
            ResizeMode mode = ResizeMode.Proportional;
            if (Crop)
            {
                mode = ResizeMode.Crop;
            }
            return GetInstanceUrl(Context, Type, Parameter, string.Empty, false, mode, Width, Height);
        }
        */

        /// <summary>
        /// Creates a string with an url that will serve an instance base on a single integer parameter. 
        /// </summary>
        public static string GetInstanceUrl(Type type, int parameter, ResizeMode resizeMode, int width, int height)
        {
            return GetInstanceUrl(type, parameter, string.Empty, false, resizeMode, width, height);
        }

        /*
        [Obsolete]
        public static string GetInstanceUrl(HttpContext Context, Type Type, int Parameter, ResizeMode ResizeMode, int Width, int Height)
        {
            return GetInstanceUrl(Context, Type, Parameter, string.Empty, false, ResizeMode, Width, Height);
        }



        [Obsolete("Use GetInstanceUrl with resize mode.")]
        public static string GetInstanceUrl(HttpContext Context, Type Type, int Parameter, string FileName, bool Download, bool Crop, int Width, int Height)
        {
            ResizeMode mode = ResizeMode.Proportional;
            if (Crop)
            {
                mode = ResizeMode.Crop;
            }
            return GetInstanceUrl(Context, Type, Parameter, FileName, Download, mode, Width, Height);
        }
         */

        /// <summary>
        /// Creates a string with an url that will serve an instance base on a single integer parameter. 
        /// </summary>
        /// <param name="type">A type that implements IResponseObject.</param>
        /// <param name="parameter">An integer value that will be passed to the constructor.</param>
        /// <param name="height">The desired height to resize the image. If zero, it will be resized proportionally.</param>
        /// <param name="width">The desired width to resize the image. If zero, it will be resized proportionally.</param>
        /// <param name="resizeMode">One of the ResizeMode values.</param>
        /// <param name="forceDownload">If true, the client browser will download the file instead of trying to show up.</param>
        /// <param name="fileName">Sets the file name shown by client browser.</param>
        /// <returns>The string with the desired url.</returns>
        public static string GetInstanceUrl(Type type, int parameter, string fileName, bool forceDownload, ResizeMode resizeMode, int width, int height)
        {
            CheckHttpModule();
            if (type == null)
                throw new ArgumentNullException("type");

            HttpContext context = HttpContext.Current;
            if (context == null)
                throw new InvalidContextException();

            System.Reflection.ConstructorInfo ctorInfo = type.GetConstructor(new Type[] { typeof(int) });
            if (ctorInfo == null)
            {
                throw (new ArgumentException("Cannot find any constructor that maches the parameters values", "Parameters", null));
            }


            string uri = Tenor.Configuration.TenorModule.HandlerFileName;
            if (context.Request.ApplicationPath.EndsWith("/"))
            {
                uri = context.Request.ApplicationPath + uri;
            }
            else
            {
                uri = context.Request.ApplicationPath + "/" + uri;
            }



            uri += "?p1=" + Convert.ToString(parameter);


            if (!string.IsNullOrEmpty(fileName))
            {
                uri += "&fn=" + context.Server.UrlEncode(fileName);
            }
            if (forceDownload)
            {
                uri += "&dl=1";
            }


            if (width > 0 || height > 0)
            {
                if ((resizeMode == Drawing.ResizeMode.Crop) || (resizeMode == Drawing.ResizeMode.Stretch))
                {
                    if (height <= 0)
                    {
                        throw (new ArgumentException("Invalid height.", "Height"));
                    }
                    if (width <= 0)
                    {
                        throw (new ArgumentException("Invalid width.", "Width"));
                    }
                }
                else if (resizeMode == Drawing.ResizeMode.Proportional)
                {
                    if (width <= 0 && height <= 0)
                    {
                        throw (new ArgumentException("Invalid width and height."));
                    }
                }
                if (resizeMode != Drawing.ResizeMode.Proportional)
                {
                    uri += "&m=" + (System.Convert.ToInt32(resizeMode)).ToString();
                }
                if (width > 0)
                {
                    uri += "&w=" + width.ToString();
                }
                if (height > 0)
                {
                    uri += "&h=" + height.ToString();
                }

            }

            //Metodo antigo
            //Dim anamespace As String = Context.Server.UrlEncode(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Type.FullName)))
            //Novo metodo
            string anamespace = Tenor.Text.Strings.ToAscHex(type.FullName);

            uri += "&cl=" + anamespace;




            return uri;

        }

        /// <summary>
        /// Gets an url that clears the Tenor Cache for the instance when called
        /// </summary>
        public static string GetClearCacheUrl(Type type, int parameter)
        {
            CheckHttpModule();
            HttpContext context = HttpContext.Current;
            if (context == null)
                throw new InvalidContextException();

            string uri = Tenor.Configuration.TenorModule.HandlerFileName;
            if (context.Request.ApplicationPath.EndsWith("/"))
            {
                uri = context.Request.ApplicationPath + uri;
            }
            else
            {
                uri = context.Request.ApplicationPath + "/" + uri;
            }
            string anamespace = Tenor.Text.Strings.ToAscHex(type.FullName);


            uri += "?p1=" + Convert.ToString(parameter);
            uri += "&clear=" + anamespace;

            return uri;
        }

    }
}