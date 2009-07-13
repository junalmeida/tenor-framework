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
using System.Runtime.InteropServices;
using System.Web.SessionState;

namespace Tenor.Web
{
    public partial class TenorModule
    {

        /// <summary>
        /// Draws a captcha image based on <see cref="Tenor.Security.Captcha"/> on session state.
        /// </summary>
        private void CaptchaRequest(string captchaSessionState)
        {
            if (HttpContext.Current == null)
                throw new InvalidContextException();
            HttpSessionState session = HttpContext.Current.Session;
            if (session == null)
                throw new InvalidContextException();

            Tenor.Security.Captcha cap = session[captchaSessionState] as Tenor.Security.Captcha;
            if (cap == null)
            {
                cap = new Tenor.Security.Captcha();
                session[captchaSessionState] = cap;
            }

            MemoryStream mem = new MemoryStream();
            cap.GenerateImage().Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);

            CacheData dados = new CacheData();
            dados.ContentType = "image/jpeg";
            dados.Expires = 0;
            dados.ContentLength = mem.Length;
            dados.FileName = "captcha.jpg";

            WriteHeaders(HttpContext.Current.ApplicationInstance, dados);
            WriteStream(mem, HttpContext.Current.ApplicationInstance);

        }

        /// <summary>
        /// Creates an audio stream bases on <see cref="Tenor.Security.Captcha"/> on session state.
        /// </summary>
        private void CaptchaAudioRequest(string captchaSessionState)
        {
            if (HttpContext.Current != null)
                throw new InvalidContextException();
            HttpSessionState session = HttpContext.Current.Session;
            if (session == null)
                throw new InvalidContextException();

            Tenor.Security.Captcha cap = session[captchaSessionState] as Tenor.Security.Captcha;
            if (cap == null)
            {
                cap = new Tenor.Security.Captcha();
                session[captchaSessionState] = cap;
            }

            MemoryStream mem = new MemoryStream(cap.GenerateAudio());

            CacheData dados = new CacheData();
            dados.ContentType = "audio/x-wav";
            dados.Expires = 0;
            dados.ContentLength = mem.Length;
            dados.FileName = "captcha.wav";

            WriteHeaders(HttpContext.Current.ApplicationInstance, dados);
            WriteStream(mem, HttpContext.Current.ApplicationInstance);
        }
    }
}