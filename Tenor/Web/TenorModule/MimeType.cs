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



        /// <summary>
        /// Returns the mime type based on file first bytes.
        /// </summary>
        /// <param name="pBC">Pointer to the IBindCtx interface. This can be set to NULL.</param>
        /// <param name="pwzUrl">Pointer to a string value that contains the URL of the data. This can be set to NULL if pBuffer contains the data to be sniffed.</param>
        /// <param name="pBuffer">Pointer to the buffer containing the data to be sniffed. This can be set to NULL if pwzUrl contains a valid URL.</param>
        /// <param name="cbSize">Unsigned long integer value that contains the size of the buffer.</param>
        /// <param name="pwzMimeProposed">Pointer to a string value containing the proposed MIME type. This can be set to NULL.</param>
        /// <param name="dwMimeFlags">
        /// One of the following values:
        /// FMFD_DEFAULT
        ///     Value is 0x00000000.
        /// FMFD_URLASFILENAME
        ///     Value is 0x00000001.
        /// FMFD_ENABLEMIMESNIFFING
        ///     Value is 0x00000002. Enable MIME sniffing.
        /// FMFD_IGNOREMIMETEXTPLAIN
        ///     Value is 0x00000004.
        /// </param>
        /// <param name="ppwzMimeOut">Address of a string value containing the suggested MIME type.</param>
        /// <param name="dwReserved">Reserved. Must be set to 0.</param>
        /// <returns>Returns one of the following values:
        /// E_INVALIDARG	One or more of the arguments passed to the function were invalid.
        /// E_OUTOFMEMORY	The function could not allocate enough memory to complete the call.
        /// NOERROR	The call was completed successfully.</returns>
        /// <remarks></remarks>
        [DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
        private static extern int FindMimeFromData(IntPtr pBC, [MarshalAs(UnmanagedType.LPWStr)]string pwzUrl, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)]byte[] pBuffer, int cbSize, [MarshalAs(UnmanagedType.LPWStr)]string pwzMimeProposed, int dwMimeFlags, ref IntPtr ppwzMimeOut, int dwReserved);

        /// <summary>
        /// Returns the mime type based on file bytes.
        /// </summary>
        /// <param name="buffer">Array of bytes with file content.</param>
        /// <returns>A system string with file mime type, or a String.Empty value when buffer is unknown.</returns>
        public static string GetMimeType(byte[] buffer)
        {
            //Dim mime As String = "application/octect-stream"
            string mime = string.Empty;
            try
            {
                IntPtr mimeout = IntPtr.Zero;

                int result = FindMimeFromData(IntPtr.Zero, "", buffer, buffer.Length, null, 0, ref mimeout, 0);

                if (result != 0)
                {
                    throw (System.Runtime.InteropServices.Marshal.GetExceptionForHR(result));

                }

                mime = System.Runtime.InteropServices.Marshal.PtrToStringUni(mimeout);
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(mimeout);
                if (mime == "application/octet-stream" || mime == "application/octect-stream")
                {
                    mime = string.Empty;
                }
            }
            catch (Exception)
            {
            }
            return mime;
        }

        /// <summary>
        /// Returns the mime type based on file stream.
        /// </summary>
        /// <param name="stream">A Stream with the file contents.</param>
        /// <returns>A system string with file mime type, or a String.Empty value when buffer is unknown.</returns>
        public static string GetMimeType(Stream stream)
        {
            return GetMimeType(IO.BinaryFile.StreamToBytes(stream));
        }

        /// <summary>
        /// Gets a mime type based on file extension.
        /// </summary>
        /// <param name="filePath">Full, partial path or just the extension (with dot) of the desired file.</param>
        /// <returns>A string with the mime type.</returns>
        /// <remarks>
        /// Uses an internal mime type mapping to define the mime type.
        /// </remarks>
        public static string GetMimeType(string filePath)
        {
            return IO.BinaryFile.GetContentType(filePath);
        }
        /// <summary>
        /// Gets the default extension of the desired mime type.
        /// </summary>
        /// <param name="mimeType">The desired mime type.</param>
        /// <returns>A string with file extension without dot.</returns>
        public static string GetExtension(string mimeType)
        {
            return IO.BinaryFile.GetExtension(mimeType);
        }
    }
}
