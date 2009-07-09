using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;
//using System.Web.UI.Design;
using System.Web;
//using System.Web.Configuration;
using System.Runtime.InteropServices;

namespace Tenor.Web
{
    public partial class TenorModule
    {

        /// <summary>
        /// Renderiza a imagem de acordo com o AccessCode gerado.
        /// </summary>
        /// <param name="captcha"></param>
        /// <remarks>
        /// Buscar no Cache do asp.net o valor armazenado na variavel passada
        /// </remarks>
        private void CaptchaRequest(string captcha)
        {
            throw new NotImplementedException();

            //Dim Cache As Caching.Cache = HttpContext.Current.Cache

            //Dim rnd As Security.Captcha
            //'If Cache(captcha) Is Nothing Then
            //rnd = New Security.Captcha
            //Cache(captcha) = rnd.AccessCode
            //'Else
            //'rnd = New Security.Captcha(CStr(Cache(captcha)))
            //'End If


            //Dim mem As New MemoryStream
            //rnd.GenerateImage().Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg)



            //Dim dados As New Dados
            //dados.ContentType = "image/jpeg"
            //dados.Expires = 0
            //dados.FileName = "captcha.jpg"

            //WriteHeaders(HttpContext.Current.ApplicationInstance, dados)
            //WriteStream(mem, HttpContext.Current.ApplicationInstance)


        }

        /// <summary>
        /// Renderiza o audio de acordo com o AccessCode gerado.
        /// </summary>
        /// <param name="captcha"></param>
        /// <remarks>
        /// Buscar no Cache do asp.net o valor armazenado na variavel passada
        /// </remarks>
        private void CaptchaAudioRequest(string captcha)
		{
				throw new NotImplementedException();
				
				//Dim Cache As Caching.Cache = HttpContext.Current.Cache
				
				//Dim rnd As Security.Captcha
				//If Cache(captcha) Is Nothing Then
				//    'rnd = New Security.Captcha
				//    'Cache(captcha) = rnd.AccessCode
				//    Throw New Exception("You must generate the image before downloading audio")
				//Else
				//    rnd = New Security.Captcha(CStr(Cache(captcha)))
				//End If
				
				
				//Dim mem As New MemoryStream( _
				//    rnd.GenerateAudio() _
				//)
				
				
				//Dim dados As New Dados
				//dados.ContentType = "audio/x-wav"
				//dados.Expires = 0
				//dados.FileName = "captcha.wav"
				
				//WriteHeaders(HttpContext.Current.ApplicationInstance, dados)
				//WriteStream(mem, HttpContext.Current.ApplicationInstance)
				
				
        }
    }
}