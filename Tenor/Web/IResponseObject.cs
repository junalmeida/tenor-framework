using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace Tenor.Web
{
    /// <summary>
    /// Defines methods that can be used on a web context to obtain stream and mime type.
    /// </summary>
    public interface IResponseObject
    {
        /// <summary>
        /// Gets the mime type of this object.
        /// </summary>
        /// <returns>A string with a mime type.</returns>
        /// <remarks></remarks>
        string ContentType
        {
            get;
        }

        /// <summary>
        /// Return the underlying stream of this object.
        /// </summary>
        /// <returns>A System.IO.Stream with contents.</returns>
        /// <remarks></remarks>
        Stream WriteContent();
    }

    /// <summary>
    /// Specifies whether a member returns a <see cref="IResponseObject"/> that will be used on a <see cref="TenorModule"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ResponsePropertyAttribute : Attribute
    {
    }
}
