using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace Tenor.IO
{
    /// <summary>
    /// This class represents a binary file.
    /// It provides tools on manipulating mime types, and writing stream to output. 
    /// </summary>
    public class BinaryFile : Web.IResponseObject
    {

        /// <summary>
        /// Gets a mime type based on file extension.
        /// </summary>
        /// <param name="path">Full, partial path or just the extension (with dot) of the desired file.</param>
        /// <returns>A string with the mime type.</returns>
        /// <remarks>
        /// Uses the internal mime.xml to define the mime type.
        /// </remarks>
        public static string GetContentType(string path)
        {
            string ext = new System.IO.FileInfo(path).Extension.ToLower().Substring(1);

            Stream file = (Stream)(typeof(BinaryFile).Assembly.GetManifestResourceStream(Tenor.Configuration.Resources.MimeXML));
            System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
            xml.Load(file);

            System.Xml.XmlNodeList nodes = xml.SelectNodes("MimeTypes/mimetype[@ext=\'" + ext + "\']");
            if (nodes.Count > 0)
            {
                return nodes[0].InnerText;
            }
            else
            {
                return "application/octet-stream";
            }

        }

        /// <summary>
        /// Gets the default extension of the desired mime type.
        /// </summary>
        /// <param name="mimeType">The desired mime type.</param>
        /// <returns>A string with file extension without dot.</returns>
        public static string GetExtension(string mimeType)
        {
            Stream file = (Stream)(typeof(BinaryFile).Assembly.GetManifestResourceStream(Configuration.Resources.MimeXML));
            System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
            xml.Load(file);

            System.Xml.XmlNodeList nodes = xml.SelectNodes("MimeTypes/mimetype[text() = \'" + mimeType + "\']");
            if (nodes.Count > 0)
            {
                return nodes[0].Attributes["ext"].Value;
            }
            else
            {
                return string.Empty;
            }

        }



        /// <summary>
        /// Converts any Stream into an array of bytes.
        /// </summary>
        /// <param name="stream">The desired stream.</param>
        /// <returns>An array of bytes.</returns>
        public static byte[] StreamToBytes(Stream stream)
        {

            stream.Seek(0, SeekOrigin.Begin);

            byte[] data = new byte[System.Convert.ToInt32(stream.Length)];
            int offset = 0;
            int total = (int)stream.Length;
            int remaining = total;

            while (remaining > 0 || total == -1)
            {
                if (total == -1)
                    remaining = 1024 * 2;

                int read = stream.Read(data, offset, remaining);
                if (read <= 0)
                {
                    break;
                }
                remaining -= read;
                offset += read;
            }

            return data;
        }

        /// <summary>
        /// Gets the underlying stream.
        /// </summary>
        public virtual Stream GetStream()
        {
            MemoryStream obj = new MemoryStream(_buffer);
            return obj;
        }

        /// <param name="buffer">An array of bytes.</param>
        /// <param name="mimeType">The mime type of the content.</param>
        public BinaryFile(byte[] buffer, string mimeType)
        {
            _buffer = buffer;
            if (!string.IsNullOrEmpty(mimeType))
                _ContentType = mimeType;
        }


        private byte[] _buffer;

        private string _ContentType = "application/octect-stream";

        /// <summary>
        /// Gets or sets the mime type of the current file.
        /// </summary>
        public string ContentType
        {
            get
            {
                return _ContentType;
            }
            set
            {
                _ContentType = value;
            }
        }

        string Tenor.Web.IResponseObject.ContentType
        {
            get
            {
                return _ContentType;
            }
        }

        System.IO.Stream Tenor.Web.IResponseObject.WriteContent()
        {
            return GetStream();
        }


        /// <summary>
        /// Creates a virtual url to show up this file on a web browser.
        /// </summary>
        /// <returns>The string with the desired url.</returns>
        /// <remarks>The registered url will have the <see cref="Tenor.Configuration.TenorModule.DefaultExpiresTime">DefaultExpiresTime</see> defined.</remarks>
        public string GetFileUrl()
        {
            return GetFileUrl(Tenor.Configuration.TenorModule.DefaultExpiresTime);
        }

        /// <summary>
        /// Creates a virtual url to show up this file on a web browser.
        /// </summary>
        /// <param name="expires">Time in seconds that the url will expire.</param>
        /// <returns>The string with the desired url.</returns>
        public string GetFileUrl(int expires)
        {
            return GetFileUrl(expires, false, null);
        }

        /// <summary>
        /// Creates a virtual url to show up this file on a web browser.
        /// </summary>
        /// <param name="forceDownload">If true, the client browser will download the file instead of trying to show up.</param>
        /// <param name="fileName">Sets the file name shown by client browser.</param>
        /// <returns>The string with the desired url.</returns>
        /// <remarks>The registered url will have the <see cref="Tenor.Configuration.TenorModule.DefaultExpiresTime">DefaultExpiresTime</see> defined.</remarks>
        public string GetFileUrl(bool forceDownload, string fileName)
        {
            return GetFileUrl(Tenor.Configuration.TenorModule.DefaultExpiresTime, forceDownload, fileName);
        }

        /// <summary>
        /// Creates a virtual url to show up this file on a web browser.
        /// </summary>
        /// <param name="expires">Time in seconds that the url will expire.</param>
        /// <param name="forceDownload">If true, the client browser will download the file instead of trying to show up.</param>
        /// <param name="fileName">Sets the file name shown by client browser.</param>
        /// <returns>The string with the desired url.</returns>
        public string GetFileUrl(int expires, bool forceDownload, string fileName)
        {
            return Tenor.Web.TenorModule.RegisterObjectForRequest(this, expires, forceDownload, fileName);
        }


    }

}