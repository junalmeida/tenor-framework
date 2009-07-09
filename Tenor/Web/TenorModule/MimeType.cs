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

        //Funções para definição de tipo mime do arquivo



        /// <summary>
        /// Função que retorna o tipo mime baseado no conteúdo de um arquivo.
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
        /// Utiliza a função do sistema FindMimeFromData para avaliar o mime type apropriado ao conteúdo do buffer
        ///
        /// </summary>
        /// <param name="buffer">Buffer em array de byte com o arquivo.</param>
        /// <returns>Uma String com o tipo mime proposto.</returns>
        /// <remarks>
        /// Caso o conteúdo do buffer não seja reconhecido, é retornado uma string vazia (String.Empty)
        /// </remarks>
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
                return mime;
            }
            catch (Exception)
            {
                return mime;
            }
        }

        /// <summary>
        /// Utiliza a função do sistema FindMimeFromData para avaliar o mime type apropriado ao conteúdo do buffer
        ///
        /// </summary>
        /// <param name="stream">Buffer em stream com o arquivo.</param>
        /// <returns>Uma String com o tipo mime proposto.</returns>
        /// <remarks>
        /// Caso o conteúdo do buffer não seja reconhecido, é retornado uma string vazia (String.Empty)
        /// </remarks>
        public static string GetMimeType(Stream stream)
        {
            return GetMimeType(IO.BinaryFile.StreamToBytes(stream));
        }

        /// <summary>
        /// Determina o Mime Type através da extensão do arquivo
        /// </summary>
        /// <param name="filePath">Nome do arquivo para determinar o tipo.</param>
        /// <returns>Uma String com o mime type sugerido</returns>
        /// <remarks>
        /// Utiliza o arquivo mime.xml para determinar o mimetype através da extensão do arquivo.
        /// </remarks>
        public static string GetMimeType(string filePath)
        {
            return IO.BinaryFile.GetContentType(filePath);
        }
        /// <summary>
        /// Retorna a extensão associada ao mime type.
        /// </summary>
        /// <param name="MimeType">Mime Type desejado</param>
        /// <returns>Extensão sem o ponto.</returns>
        /// <remarks></remarks>
        public static string GetExtension(string MimeType)
        {
            return IO.BinaryFile.GetExtension(MimeType);
        }
    }
}
