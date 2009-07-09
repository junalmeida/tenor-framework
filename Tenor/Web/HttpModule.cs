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
//using System.Web.SessionState;
using System.Web.Configuration;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;


namespace Tenor.Web
{

    /// <summary>
    /// The TenorModule is an HttpModule to handle database, graphical and other http requests.
    /// </summary>
    public partial class TenorModule : IHttpModule //, System.Web.SessionState.IReadOnlySessionState
    {
        /*
        #region " Gambiarra - Session State "

        private class MyHttpHandler : IHttpHandler, IRequiresSessionState
        {


            internal readonly IHttpHandler OriginalHandler;
            public MyHttpHandler(IHttpHandler originalHandler)
            {
                originalHandler = originalHandler;
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
        //Public Const PublicadorExtensao As String = ".axd/"



        private void Application_PostAcquireRequestState(object sender, EventArgs e)
        {

            HttpApplication app = (HttpApplication)sender;
            if (app.Context.Request.RawUrl.ToLower().Contains(Configuration.HttpModule.HandlerFileName.ToLower()))
            {

                MyHttpHandler resourceHttpHandler = HttpContext.Current.Handler as MyHttpHandler;



                if (resourceHttpHandler != null)
                {
                    HttpContext.Current.Handler = resourceHttpHandler.OriginalHandler;
                }
                //System.Diagnostics.Debug.Assert(app.Session Is Nothing, "it did not work :(")
            }
        }

        private void Application_PostMapRequestHandler(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            if (app.Context.Request.RawUrl.ToLower().Contains(Configuration.HttpModule.HandlerFileName.ToLower()))
            {

                if (app.Context.Handler is IReadOnlySessionState || app.Context.Handler is IRequiresSessionState)
                {
                    return;
                }
                app.Context.Handler = new MyHttpHandler(app.Context.Handler);
            }
        }
        #endregion
        */



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




        public void Dispose()
        {
        }


        public void Init(System.Web.HttpApplication app)
        {
            //' SessÃ£o.
            //app.PostAcquireRequestState += new System.EventHandler(Application_PostAcquireRequestState);
            //app.PostMapRequestHandler += new System.EventHandler(Application_PostMapRequestHandler);
            //'---

            app.AcquireRequestState += new System.EventHandler(AcquireRequestState);
            app.Error += new System.EventHandler(Application_Error);
        }

        /// <summary>
        /// FunÃ§Ã£o chamada a cada requisiÃ§Ã£o ao servidor.
        /// Este Ã© o mÃ©todo responsÃ¡vel pela maior parte do trabalho deste HttpModule.
        /// EstÃ¡ dividido em trÃªs partes:
        /// 1: TinyMCE, IEFix
        ///   Nesta seÃ§Ã£o ele irÃ¡ retornar o arquivo requisitado pelo componente javascript TinyMCE, presente na dll TinyMCE.
        ///   Esta dll Ã© requerida pelo controle TextBox da Web.UI quando utilizado como um RichText control.
        /// 2: Controle do tipo IResponseObject
        ///   Nesta seÃ§Ã£o ele irÃ¡ preparar para manipular a requisiÃ§Ã£o. Ã‰ neste momento que a imagem ou arquivo estÃ¡ sendo requisitado pelo navegador.
        ///   Se for a primeira chamada deste objeto, o mesmo estarÃ¡ no cache (Dados.Object) e serÃ¡ chamado o metodo WriteContent.
        ///   O resultado serÃ¡ armazenado no cache no mesmo campo Dados.Object para posteriores chamadas de cache.
        /// 3: RequisiÃ§Ã£o direta de dados, chamado aqui de InstanceRequest
        ///   Nesta seÃ§Ã£o ele irÃ¡ procurar uma classe fornecida pela querystring (class), passando um parÃ¢metro (Integer) para um construtor da classe fazendo assim
        ///   a conexÃ£o com o banco para pegar o id selecionado. A classe fornecida precisa implementar IResponseObject ou possuir uma propriedade que retorne uma instancia
        ///   de iResponseObject. Para este segundo caso, a propriedade precisa ter o atributo ResponseProperty.
        /// </summary>
        /// <param name="sender">HttpApplication</param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        protected void AcquireRequestState(object sender, EventArgs e)
        {

            HttpApplication app = (HttpApplication)sender;
            string path = app.Request.Path.ToLower();
            if (path.Contains(Tenor.Configuration.TenorModule.HandlerFileName.ToLower() + "/tiny_mce/"))
            {
                //RequisiÃ§Ã£o do TinyMCE
                //Ver arquivo TinyMCE.vb
                TinyMCERequest(app);
            }
            else if (path.Contains(Tenor.Configuration.TenorModule.HandlerFileName.ToLower() + "/iefix/"))
            {
                //RequisiÃ§Ã£o do IEFix
                //Ver arquivo IEFix.vb
                IEFixRequest(app);
            }
            else if (path.Contains(Tenor.Configuration.TenorModule.HandlerFileName.ToLower()))
            {
                if (!string.IsNullOrEmpty(QueryString("id")))
                {
                    //Caso contenha um ID que seja uma GUID, requisiÃ§Ã£o de Cache.
                    //ver ObjectRequest.vb
                    ResponseObjectRequest(app);
                }
                else if (!string.IsNullOrEmpty(QueryString("c")))
                {
                    //Caso contenha dados em C, RequisiÃ§Ã£o de Chart
                    //ver Chart.vb
                    //ChartRequest(app);

                }
                else if (!string.IsNullOrEmpty(QueryString("cl")))
                {
                    //Caso contenha um nome de classe em CL.
                    //ver InstanceRequest.vb
                    InstanceRequest(app);
                }
                else if (!string.IsNullOrEmpty(QueryString("captcha")))
                {
                    //RequisiÃ§Ãµes de Capcha. Ainda nÃ£o implementado 100%.
                    if (string.IsNullOrEmpty(QueryString("audio")))
                    {
                        CaptchaRequest(QueryString("captcha"));
                    }
                    else
                    {
                        CaptchaAudioRequest(QueryString("captcha"));
                    }
                }
                else if (!string.IsNullOrEmpty(QueryString("clear")))
                {
                    //clear cache
                    //ver Cache.vb
                    ClearCache(app, QueryString("clear"), QueryString("p1"), true);
                }
                else if (!string.IsNullOrEmpty(QueryString("bt")))
                {

                    this.DynamicImageRequest(app, QueryString("bt"));

                }

            }
        }


        /// <summary>
        /// Envia para o cliente os headers do documento
        /// </summary>
        /// <param name="app">Aplicativo atual</param>
        /// <param name="dados">Uma instancia de Dados</param>
        /// <remarks></remarks>
        private void WriteHeaders(HttpApplication app, Dados dados)
        {
            app.Response.ContentType = dados.ContentType;
            app.Response.AddHeader("Content-Length", dados.ContentLength.ToString());

            string header = "";
            if (dados.ForceDownload)
            {
                //o cliente irÃ¡ obrigatÃ³riamente baixar o arquivo
                header += "attachment; ";
            }
            if (!string.IsNullOrEmpty(dados.FileName))
            {
                //o nome do arquivo foi definido
                header += " filename=\"" + dados.FileName + "\"";
            }
            else
            {
                //especificar um nome padrÃ£o
                header += " filename=\"file." + GetExtension(dados.ContentType) + "\"";

            }

            if (header.Length > 0)
            {
                app.Response.AddHeader("content-disposition", header);
            }
            //define o cache do navegador do cliente
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
        /// Envia para o cliente a stream.
        /// </summary>
        /// <param name="stream">Stream para enviar</param>
        /// <param name="app">Application</param>
        /// <remarks></remarks>
        private void WriteStream(Stream stream, HttpApplication app)
        {
            app.Context.ClearError();
            app.Response.Clear();
            //nÃ£o usar buffers de saÃ­da.
            app.Response.BufferOutput = false;


            MemoryStream mem = stream as MemoryStream;
            if (mem != null)
            {
                //usar o mÃ©todo WriteTo da memorystream
                mem.WriteTo(app.Response.OutputStream);
            }
            else if (stream.CanRead && stream.Length > 0)
            {
                //converter a stream em bytes e enviar para o cliente
                byte[] data = IO.BinaryFile.StreamToBytes(stream);
                app.Response.OutputStream.Write(data, 0, data.Length);
            }
            else
            {
                throw (new IOException("Invalid stream."));
            }
        }


        /// <summary>
        /// Armazena os dados necessÃ¡rio para o ciclo de vida do HttpModule.
        /// </summary>
        /// <remarks></remarks>
        private class Dados
        {

            public Dados()
            {
                LastModified = DateTime.Now;
            }

            /// <summary>
            /// ContÃ©m o objeto Ã  ser reproduzido na primeira chamada.
            /// ApÃ³s a primeira chamada, este campo guarda o stream gerado pelo objeto para ser utilizado durante a vida do cache.
            /// </summary>
            /// <remarks></remarks>
            public object @Object;

            /// <summary>
            /// Tempo de vida do cache em segundos.
            /// </summary>
            /// <remarks></remarks>
            public int Expires;

            /// <summary>
            /// Indica se o cliente irÃ¡ sempre baixar o arquivo ou exibi-lo na janela do navegador quando possÃ­vel
            /// </summary>
            /// <remarks></remarks>
            public bool ForceDownload;

            /// <summary>
            /// Indica o nome do arquivo para enviar ao navegador do cliente.
            /// </summary>
            /// <remarks></remarks>
            public string FileName;

            /// <summary>
            /// Indica o tipo mime que serÃ¡ reconhecido pelo navegador do cliente.
            /// </summary>
            /// <remarks></remarks>
            public string ContentType;

            /// <summary>
            /// Indica o tamanho em bytes do arquivo
            /// </summary>
            /// <remarks></remarks>
            public long ContentLength;

            /// <summary>
            /// Data de modificaÃ§Ã£o do arquivo.
            /// </summary>
            /// <remarks></remarks>
            public DateTime LastModified;

            /// <summary>
            /// ETag
            /// </summary>
            /// <remarks></remarks>
            public string ETag
            {
                get
                {
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