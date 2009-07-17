using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace Tenor
{
	namespace Drawing
	{
		/// <summary>
        /// Defines how the image should be resized.
		/// </summary>
		public enum ResizeMode
		{
			/// <summary>
            /// Streches the image to the given width and height values.
			/// </summary>
			Stretch,
			/// <summary>
            /// Resizes the image proportionally using the given width and height values.
			/// </summary>
			Proportional,
			/// <summary>
            /// Resizes the image cropping and centering it. The image should fill entire width and height values.
			/// </summary>
			Crop
		}
		

        /// <summary>
        /// Represents a class that will have image methods.
        /// </summary>
		public interface IImage
		{			
			/// <summary>
            /// Resizes the current image by a porcentage value.
            /// </summary>
			/// <param name="percent">How much the image will be resized. You can use positive and negative values.</param>
			void ResizeByPercent(int percent);
			
			/// <summary>
			/// Resizes the current image.
			/// </summary>
			/// <param name="width">The new image width in pixels.</param>
			/// <param name="height">The new image height in pixels.</param>
			/// <param name="mode">One of the ResizeMode values.</param>
			void Resize(int width, int height, ResizeMode mode);
			
			/// <summary>
            /// Draws another image on the current image.
			/// </summary>
			/// <param name="fileName">A file name of the image that will be drawed.</param>
			/// <param name="position">One of the Position values.</param>
			/// <param name="margin">The margin in pixels.</param>
			void AddPicture(string fileName, Position position, int margin);
			
			
			/// <summary>
            /// Gets or sets a boolean to determine if this is a low qualit image.
			/// </summary>
			bool LowQuality{
				get;
				set;
			}
			
		}
	}
	
}
