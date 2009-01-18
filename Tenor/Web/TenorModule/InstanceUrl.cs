using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
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

        //Funções Url de Instãncia



        /// <summary>
        /// Retorna uma string com a url que representa a instancia.
        /// </summary>
        public static string GetInstanceUrl(Type Type, int Parameter)
        {
            return GetInstanceUrl(HttpContext.Current, Type, Parameter, string.Empty, false, Drawing.ResizeMode.Stretch, 0, 0);
        }

        /// <summary>
        /// Retorna uma string com a url que representa a instancia.
        /// </summary>
        public static string GetInstanceUrl(Type Type, int Parameter, string FileName, bool Download)
        {
            return GetInstanceUrl(HttpContext.Current, Type, Parameter, FileName, Download, Drawing.ResizeMode.Stretch, 0, 0);
        }


        /// <summary>
        /// Retorna uma string com a url que representa a instancia.
        /// </summary>
        public static string GetInstanceUrl(HttpContext Context, Type Type, int Parameter)
        {
            return GetInstanceUrl(Context, Type, Parameter, string.Empty, false, Drawing.ResizeMode.Stretch, 0, 0);
        }

        /// <summary>
        /// Retorna uma string com a url que representa a instancia.
        /// </summary>
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

        /// <summary>
        /// Retorna uma string com a url que representa a instancia.
        /// </summary>
        public static string GetInstanceUrl(Type Type, int Parameter, ResizeMode ResizeMode, int Width, int Height)
        {
            return GetInstanceUrl(HttpContext.Current, Type, Parameter, string.Empty, false, ResizeMode, Width, Height);
        }

        /// <summary>
        /// Retorna uma string com a url que representa a instancia.
        /// </summary>
        public static string GetInstanceUrl(HttpContext Context, Type Type, int Parameter, ResizeMode ResizeMode, int Width, int Height)
        {
            return GetInstanceUrl(Context, Type, Parameter, string.Empty, false, ResizeMode, Width, Height);
        }


        /// <summary>
        /// Retorna uma string com a url que representa a instancia.
        /// </summary>
        /// <param name="Context">Context</param>
        /// <param name="Type">Tipo da classe desejada</param>
        /// <param name="Parameter">Parametro do construtor</param>
        /// <param name="FileName">Nome do arquivo desejado</param>
        /// <param name="Download">Forçar Download?</param>
        /// <param name="Crop">Cortar a imagem?</param>
        /// <param name="Width">Largura</param>
        /// <param name="Height">Altura</param>
        /// <returns></returns>
        /// <remarks></remarks>
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

        /// <summary>
        /// Retorna uma string com a url que representa a instancia.
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Parameter"></param>
        /// <param name="FileName"></param>
        /// <param name="Download"></param>
        /// <param name="ResizeMode"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetInstanceUrl(Type Type, int Parameter, string FileName, bool Download, ResizeMode ResizeMode, int Width, int Height)
        {
            return GetInstanceUrl(HttpContext.Current, Type, Parameter, FileName, Download);
        }

        /// <summary>
        /// Retorna uma string com a url que representa a instancia.
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Parameter"></param>
        /// <param name="FileName"></param>
        /// <param name="Download"></param>
        /// <param name="ResizeMode"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetInstanceUrl(HttpContext Context, Type Type, int Parameter, string FileName, bool Download, ResizeMode ResizeMode, int Width, int Height)
        {
            CheckHttpModule();





            string uri = Tenor.Configuration.TenorModule.HandlerFileName;
            if (Context.Request.ApplicationPath.EndsWith("/"))
            {
                uri = Context.Request.ApplicationPath + uri;
            }
            else
            {
                uri = Context.Request.ApplicationPath + "/" + uri;
            }



            uri += "?p1=" + Convert.ToString(Parameter);


            if (!string.IsNullOrEmpty(FileName))
            {
                uri += "&fn=" + Context.Server.UrlEncode(FileName);
            }
            if (Download)
            {
                uri += "&dl=1";
            }


            if (Width > 0 || Height > 0)
            {
                if ((ResizeMode == Drawing.ResizeMode.Crop) || (ResizeMode == Drawing.ResizeMode.Stretch))
                {
                    if (Height <= 0)
                    {
                        throw (new ArgumentException("Invalid height.", "Height"));
                    }
                    if (Width <= 0)
                    {
                        throw (new ArgumentException("Invalid width.", "Width"));
                    }
                }
                else if (ResizeMode == Drawing.ResizeMode.Proportional)
                {
                    if (Width <= 0 && Height <= 0)
                    {
                        throw (new ArgumentException("Invalid width and height."));
                    }
                }
                if (ResizeMode != Drawing.ResizeMode.Proportional)
                {
                    uri += "&m=" + (System.Convert.ToInt32(ResizeMode)).ToString();
                }
                if (Width > 0)
                {
                    uri += "&w=" + Width.ToString();
                }
                if (Height > 0)
                {
                    uri += "&h=" + Height.ToString();
                }

            }

            //Metodo antigo
            //Dim anamespace As String = Context.Server.UrlEncode(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Type.FullName)))
            //Novo metodo
            string anamespace = Tenor.Text.Strings.ToAscHex(Type.FullName);

            uri += "&cl=" + anamespace;

            System.Reflection.ConstructorInfo ctorInfo = Type.GetConstructor(new Type[] { typeof(int) });
            if (ctorInfo == null)
            {
                throw (new ArgumentException("Cannot find any constructor that maches the parameters values", "Parameters", null));
            }


            return uri;

        }

        /// <summary>
        /// Retorna a Url de limpeza de cache.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="Type"></param>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetClearCacheUrl(HttpContext Context, Type Type, int Parameter)
        {
            CheckHttpModule();

            string uri = Tenor.Configuration.TenorModule.HandlerFileName;
            if (Context.Request.ApplicationPath.EndsWith("/"))
            {
                uri = Context.Request.ApplicationPath + uri;
            }
            else
            {
                uri = Context.Request.ApplicationPath + "/" + uri;
            }
            string anamespace = Tenor.Text.Strings.ToAscHex(Type.FullName);


            uri += "?p1=" + Convert.ToString(Parameter);
            uri += "&clear=" + anamespace;

            return uri;
        }




    }


}