using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;



namespace Tenor.Drawing
{
    /// <summary>
    /// Represents an Image.
    /// </summary>
    public class Image : IO.BinaryFile, ICloneable, IImage
    {

        #region " Constructors "

        /// <summary>
        /// Creates and instance of the current image.
        /// </summary>
        /// <param name="bitmap">An array of bytes of an JPEG image.</param>
        /// <remarks></remarks>
        public Image(byte[] bitmap)
            : base(bitmap, "image/jpeg")
        {

            _Bitmap = new System.Drawing.Bitmap(new MemoryStream(bitmap));
            RawFormat = _Bitmap.RawFormat;

        }


        /// <summary>
        /// Creates and instance of the current image.
        /// </summary>
        /// <param name="bitmap">A bitmap image.</param>
        /// <remarks></remarks>
        public Image(System.Drawing.Bitmap bitmap)
            : base(AttachBitmap(bitmap), "image/jpeg")
        {
            _Bitmap = bitmap;
            RawFormat = bitmap.RawFormat;
        }

        private static byte[] AttachBitmap(System.Drawing.Bitmap bitmap)
        {
            MemoryStream mem = new MemoryStream();
            bitmap.Save(mem, bitmap.RawFormat);
            byte[] data = StreamToBytes(mem);
            return data;
        }

        /// <param name="bitmap">The original bitmap.</param>
        /// <param name="width">New width in pixels.</param>
        /// <param name="height">new height in pixels.</param>
        /// <remarks>Set zero to width or height values to resize proportionally.</remarks>
        public Image(byte[] bitmap, int width, int height)
            : this(bitmap)
        {
            if (width <= 0 && height > 0)
            {
                ResizeByHeight(height);
            }
            else if (width > 0 && height <= 0)
            {
                ResizeByWidth(width);
            }
            else
            {
                Resize(width, height);
            }
        }


        /// <param name="bitmap">The original bitmap.</param>
        public Image(Stream bitmap)
            : this(StreamToBytes(bitmap))
        {
        }

        /// <param name="bitmap">The original bitmap.</param>
        /// <param name="width">New width in pixels.</param>
        /// <param name="height">new height in pixels.</param>
        /// <remarks>Set zero to width or height values to resize proportionally.</remarks>
        public Image(Stream bitmap, int width, int height)
            : this(bitmap)
        {
            if (width <= 0 && height > 0)
            {
                ResizeByHeight(height);
            }
            else if (width > 0 && height <= 0)
            {
                ResizeByWidth(width);
            }
            else
            {
                Resize(width, height);
            }
        }

        /// <param name="bitmap">The original bitmap.</param>
        /// <param name="width">New width in pixels.</param>
        /// <param name="height">new height in pixels.</param>
        /// <remarks>Set zero to width or height values to resize proportionally.</remarks>
        public Image(System.Drawing.Bitmap bitmap, int width, int height)
            : this(bitmap)
        {
            if (width <= 0 && height > 0)
            {
                ResizeByHeight(height);
            }
            else if (width > 0 && height <= 0)
            {
                ResizeByWidth(width);
            }
            else
            {
                Resize(width, height);
            }
        }


        #endregion

        #region " Properties "

        private System.Drawing.Bitmap _Bitmap;
        private System.Drawing.Imaging.ImageFormat RawFormat;

        /// <summary>
        /// Gets the underlying bitmap of this instance.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public System.Drawing.Bitmap Bitmap
        {
            get
            {
                return _Bitmap;
            }
        }

        /// <summary>
        /// The image width in pixels.
        /// </summary>
        public int Width
        {
            get
            {
                return Bitmap.Width;
            }
        }

        /// <summary>
        /// The image height in pixels.
        /// </summary>
        public int Height
        {
            get
            {
                return Bitmap.Height;
            }

        }

        #endregion

        #region " Resizing and Crop "

        /// <summary>
        /// Resizes the image proportionally by width.
        /// </summary>
        /// <param name="width">The new width in pixels.</param>
        /// <remarks></remarks>
        public void ResizeByWidth(int width)
        {
            Resize(width, 0, ResizeMode.Proportional);

        }

        /// <summary>
        /// Resizes the image proportionally by width.
        /// </summary>
        /// <param name="height">The new height in pixels.</param>
        /// <remarks></remarks>
        public void ResizeByHeight(int height)
        {
            Resize(0, height, ResizeMode.Proportional);
        }

        /// <summary>
        /// Resizes the image proportionally.
        /// </summary>
        /// <param name="percent">The percentage to resize the image.</param>
        /// <remarks></remarks>
        public void ResizeByPercent(int percent)
        {
            int width = percent * this.Width / 100;
            int height = percent * this.Height / 100;
            Resize(width, height, ResizeMode.Stretch);
        }


        /// <summary>
        /// Resizes the image to a new size.
        /// </summary>
        /// <param name="size">The new size in pixels.</param>
        public void Resize(System.Drawing.Size size)
        {
            Resize(size.Width, size.Height, ResizeMode.Stretch);
        }

        /// <summary>
        /// Resizes the image to a new size.
        /// </summary>
        /// <param name="width">The new width in pixels.</param>
        /// <param name="height">The new height in pixels.</param>
        /// <param name="mode">One of the ResizeMode values.</param>
        /// <remarks></remarks>
        public void Resize(int width, int height, ResizeMode mode)
        {
            Rectangle rect = new Rectangle();
            Size imagemSize = new Size();
            if (width <= 0 && height <= 0)
            {
                throw (new ArgumentException("Invalid resizing parameters. Check width and height values."));
            }

            switch (mode)
            {
                case Drawing.ResizeMode.Stretch:
                case Drawing.ResizeMode.Crop:
                    if (width <= 0 || height <= 0)
                    {
                        throw (new ArgumentException("Invalid image parameters. Check width and height values."));
                    }
                    break;
                case Drawing.ResizeMode.Proportional:
                    if (width <= 0 && height <= 0)
                    {
                        throw (new ArgumentException("Invalid image parameters. Check width and height values."));
                    }
                    break;
            }



            switch (mode)
            {
                case ResizeMode.Stretch:
                    rect = new Rectangle(0, 0, width, height);
                    //this will be the new size
                    imagemSize = new Size(width, height);
                    break;
                case ResizeMode.Crop:
                    //New size based on defined width:
                    Size sizeL = new Size();
                    sizeL.Width = width;
                    sizeL.Height = Convert.ToInt32((double)width / (double)this.Bitmap.Width * (double)this.Bitmap.Height);

                    //New size based on defined height:
                    Size sizeH = new Size();
                    sizeH.Height = height;
                    sizeH.Width = Convert.ToInt32((double)height / (double)this.Bitmap.Height * (double)this.Bitmap.Width);


                    rect = new Rectangle();
                    //Choose if it will be based on width or height. This is necessary to avoid blank rectangles on the new image.
                    if (sizeL.Width >= width && sizeL.Height >= height)
                    {
                        rect.Size = sizeL;
                    }
                    else
                    {
                        rect.Size = sizeH;
                    }

                    //Sets the origin point to center the new image.
                    rect.Location = new Point(System.Convert.ToInt32(-(rect.Size.Width - width) / 2), System.Convert.ToInt32(-(rect.Size.Height - height) / 2));

                    //the new size
                    imagemSize = new Size(width, height);
                    break;

                case ResizeMode.Proportional:
                    if (width > 0)
                    {


                        int percent = 100 * width / this.Width;
                        rect = new Rectangle(0, 0, width, percent * this.Height / 100);

                        if (height > 0 && rect.Height > height)
                        {
                            percent = 100 * height / this.Height;
                            rect = new Rectangle(0, 0, percent * this.Width / 100, height);
                        }


                    }
                    else if (height > 0)
                    {
                        int percent = 100 * height / this.Height;
                        rect = new Rectangle(0, 0, percent * this.Width / 100, height);

                        if (width > 0 && rect.Width > width)
                        {
                            percent = 100 * width / this.Width;
                            rect = new Rectangle(0, 0, width, percent * this.Height / 100);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }

                    imagemSize = rect.Size;
                    break;
            }

            System.Drawing.Bitmap bmpT = new System.Drawing.Bitmap(imagemSize.Width, imagemSize.Height);
            System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmpT);


            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;



            gr.DrawImage(Bitmap, rect);

            Bitmap.Dispose();
            _Bitmap = bmpT;

            gr.Dispose();

        }


        /// <summary>
        /// Resizes the image to a new size. 
        /// </summary>
        /// <param name="width">The new width in pixels.</param>
        /// <param name="height">The new height in pixels.</param>
        public void Resize(int width, int height)
        {
            Resize(width, height, ResizeMode.Stretch);
        }


        ///// <summary>
        ///// Redimensiona a imagem respeitando as proporÃ§Ãµes e corta as sobras de forma a centralizar a imagem.
        ///// </summary>
        ///// <param name="Size">Uma estrutura <see cref="Size">Size</see> que define o novo tamanho da imagem.</param>
        ///// <remarks></remarks>
        //[Obsolete("Use Resize instead", false)]
        //public void ResizeAndCrop(Size Size)
        //{
        //    Resize(Size.Width, Size.Height, ResizeMode.Stretch);
        //}



        ///// <summary>
        ///// Redimensiona a imagem respeitando as proporÃ§Ãµes e corta as sobras de forma a centralizar a imagem.
        ///// </summary>
        ///// <param name="width">Nova largura em pixels</param>
        ///// <param name="height">Nova altura em pixels</param>
        ///// <remarks></remarks>
        //[Obsolete("Use Resize instead", false)]
        //public void ResizeAndCrop(int width, int height)
        //{
        //    Resize(width, height, ResizeMode.Stretch);


        //    //Dim bmp As New Bitmap(width, height)

        //    //'Novo tamanho baseado na largura especificada
        //    //Dim sizeL As New Size
        //    //sizeL.Width = width
        //    //sizeL.Height = CInt(width / Me.Bitmap.Width * Me.Bitmap.Height)

        //    //'Novo tamanho baseado na altura especificada
        //    //Dim sizeH As New Size
        //    //sizeH.Height = height
        //    //sizeH.Width = CInt(height / Me.Bitmap.Height * Me.Bitmap.Width)


        //    //Dim rect As New Rectangle
        //    //'Selecionar por largura ou por altura para nÃ£o haver sobras (espaÃ§os em branco) na nova imagem
        //    //If sizeL.Width >= width And sizeL.Height >= height Then
        //    //    rect.Size = sizeL
        //    //Else
        //    //    rect.Size = sizeH
        //    //End If

        //    //'Definir ponto de origem para que a imagem reduzida fique centralizada
        //    //rect.Location = New Point(CInt(-(rect.Size.Width - width) / 2), CInt(-(rect.Size.Height - height) / 2))


        //    //'ConfiguraÃ§Ãµes de renderizaÃ§Ã£o
        //    //Dim gr As Graphics = Graphics.FromImage(bmp)
        //    //gr.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
        //    //gr.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
        //    //gr.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
        //    //gr.CompositingQuality = Drawing2D.CompositingQuality.HighQuality

        //    //'Desenhar a nova imagem
        //    //gr.DrawImage(Me.Bitmap, rect)



        //    //gr.Dispose()

        //    //Me._Bitmap.Dispose()
        //    //Me._Bitmap = bmp


        //}


        #endregion

        #region " IResponseObject "
        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();

            j = 0;
            while (j < encoders.Length)
            {
                if (encoders[j].FormatID == format.Guid)
                {
                    return encoders[j];
                }
                j++;
            }
            return null;

        } //GetEncoderInfo

        public override System.IO.Stream GetStream()
        {

            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
            {
                ContentType = "image/jpeg";
            }
            else if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
            {
                ContentType = "image/gif";
            }
            else if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
            {
                ContentType = "image/bmp";
            }
            else if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Tiff))
            {
                ContentType = "image/tiff";
            }
            else if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
            {
                ContentType = "image/x-png";
            }
            else if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Wmf))
            {
                ContentType = "image/wmf";
            }
            else
            {
                throw (new NotSupportedException("Unsupported image format \'" + RawFormat.ToString() + "\'"));
            }

            if (LowQuality && ContentType == "image/jpeg")
            {
                try
                {
                    ImageCodecInfo myImageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);
                    Encoder myEncoder;
                    EncoderParameter myEncoderParameter;
                    EncoderParameters myEncoderParameters;


                    // Create an Encoder object based on the GUID
                    // for the Quality parameter category.
                    myEncoder = Encoder.Quality;

                    // Create an EncoderParameters object.
                    // An EncoderParameters object has an array of EncoderParameter
                    // objects. In this case, there is only one
                    // EncoderParameter object in the array.
                    myEncoderParameters = new EncoderParameters(1);


                    MemoryStream mem = new MemoryStream();

                    // Save the bitmap as a JPEG file with quality level 25.
                    myEncoderParameter = new EncoderParameter(myEncoder, 25);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    Bitmap.Save(mem, myImageCodecInfo, myEncoderParameters);


                    return mem;


                }
                catch (Exception)
                {
                    MemoryStream mem = new MemoryStream();
                    Bitmap.Save(mem, RawFormat);
                    return mem;
                }
            }
            else
            {
                MemoryStream mem = new MemoryStream();
                Bitmap.Save(mem, RawFormat);
                return mem;

            }


        }
        #endregion

        #region " Thumbs "

        ///// <summary>
        ///// Cria uma miniatura da imagem atual
        ///// </summary>
        ///// <param name="Page">PÃ¡gina onde serÃ¡ exibida</param>
        ///// <param name="Width">Largura da miniatura desejada</param>
        ///// <param name="Height">Altura da miniatura desejada</param>
        ///// <returns></returns>
        ///// <remarks>Defina a largura ou altura em zero para fazer o redimensinamento proporcional. Definir ambos em zero cria uma imagem com 110 pixels de largura</remarks>
        //[Obsolete("Usa o outro overload")]
        //public string GetThumbImageUrl(System.Web.UI.Page Page, int Width, int Height)
        //{
        //    Image img = new Image((System.Drawing.Bitmap)(this.Bitmap.Clone()));

        //    if (Width > 0 && Height > 0)
        //    {
        //        img.Resize(Width, Height);
        //    }
        //    else if (Width > 0)
        //    {
        //        img.ResizeByWidth(Width);
        //    }
        //    else if (Height > 0)
        //    {
        //        img.ResizeByHeight(Height);
        //    }
        //    else
        //    {
        //        img.ResizeByWidth(110);
        //    }
        //    return img.GetFileUrl();

        //}

        /// <summary>
        /// Creates a thumbnail of the current image.
        /// Cria uma miniatura da imagem atual
        /// </summary>
        /// <param name="width">The desired thumbnail width.</param>
        /// <param name="height">The desired thumbnail height.</param>
        /// <returns></returns>
        /// <remarks>
        /// You can set width or height values to zero to make a proportional thumbnail.
        /// If you set both values to zero or less than zero, an image with 110 pixels will be created.
        ///</remarks>
        public string GetThumbImageUrl(int width, int height)
        {
            Image img = new Image((System.Drawing.Bitmap)(this.Bitmap.Clone()));

            if (width > 0 && height > 0)
            {
                img.Resize(width, height);
            }
            else if (width > 0)
            {
                img.ResizeByWidth(width);
            }
            else if (height > 0)
            {
                img.ResizeByHeight(height);
            }
            else
            {
                img.ResizeByWidth(110);
            }
            return img.GetFileUrl();
        }

        #endregion

        #region " Text "


        /// <summary>
        /// Draws an string with a text on the current image.
        /// </summary>
        /// <param name="text">A string with a text to be drawed.</param>
        /// <param name="fontFamily">The font family used.</param>
        /// <param name="size">Font size in points (pt).</param>
        /// <param name="fontStyle">One of the System.Drawing.FontStyle values.</param>
        /// <param name="align">One of the System.Drawing.ContentAlignment values.</param>
        /// <param name="color">A System.Drawing.Color to be used.</param>
        public void AddText(string text, string fontFamily, float size, System.Drawing.FontStyle fontStyle, System.Drawing.Color color, System.Drawing.ContentAlignment align)
        {
            System.Drawing.Font font = new System.Drawing.Font(fontFamily, size, fontStyle, System.Drawing.GraphicsUnit.Point);
            AddText(text, font, color, align);
        }

        /// <summary>
        /// Draws an string with a text on the current image.
        /// </summary>
        /// <param name="text">A string with a text to be drawed.</param>
        /// <param name="font">The font to be used.</param>
        /// <param name="align">One of the System.Drawing.ContentAlignment values.</param>
        /// <param name="color">A System.Drawing.Color to be used.</param>
        /// <param name="offsetX">Defines how much the text will be offset horizontally. Negative values sets the text to its left side.</param>
        /// <param name="offsetY">Defines how much the text will be offset vertically. Negative values sets the text to its top side.</param>
        public void AddText(string text, System.Drawing.Font font, System.Drawing.Color color, System.Drawing.ContentAlignment align, int offsetX, int offsetY)
        {
            Graphics gr = System.Drawing.Graphics.FromImage(Bitmap);
            SizeF size = gr.MeasureString(text, font, Bitmap.Width);
            PointF lic = new PointF();
            //positioning:
            switch (align)
            {
                case System.Drawing.ContentAlignment.TopLeft:
                    lic = new System.Drawing.PointF(0, 0);
                    break;
                case System.Drawing.ContentAlignment.TopCenter:
                    lic = new System.Drawing.PointF(System.Convert.ToSingle((Bitmap.Width / 2) - (size.Width / 2)), 0);
                    break;
                case System.Drawing.ContentAlignment.TopRight:
                    lic = new System.Drawing.PointF(Bitmap.Width - size.Width, 0);
                    break;
                case System.Drawing.ContentAlignment.MiddleLeft:
                    lic = new System.Drawing.PointF(0, System.Convert.ToSingle((Bitmap.Height / 2) - (size.Height / 2)));
                    break;
                case System.Drawing.ContentAlignment.MiddleCenter:
                    lic = new System.Drawing.PointF(System.Convert.ToSingle((Bitmap.Width / 2) - (size.Width / 2)), System.Convert.ToSingle((Bitmap.Height / 2) - (size.Height / 2)));
                    break;
                case System.Drawing.ContentAlignment.MiddleRight:
                    lic = new System.Drawing.PointF(Bitmap.Width - size.Width, System.Convert.ToSingle((Bitmap.Height / 2) - (size.Height / 2)));
                    break;
                case System.Drawing.ContentAlignment.BottomLeft:
                    lic = new System.Drawing.PointF(0, Bitmap.Height - size.Height);
                    break;
                case System.Drawing.ContentAlignment.BottomCenter:
                    lic = new System.Drawing.PointF(System.Convert.ToSingle((Bitmap.Width / 2) - (size.Width / 2)), Bitmap.Height - size.Height);
                    break;
                case System.Drawing.ContentAlignment.BottomRight:
                    lic = new System.Drawing.PointF(Bitmap.Width - size.Width, Bitmap.Height - size.Height);
                    break;
            }
            //calculate offset
            lic.X += offsetX;
            lic.Y += offsetY;
            //draws the text
            gr.DrawString(text, font, new System.Drawing.SolidBrush(color), lic);
            gr.Dispose();
        }

        /// <summary>
        /// Draws an string with a text on the current image.
        /// </summary>
        /// <param name="text">A string with a text to be drawed.</param>
        /// <param name="font">The font to be used.</param>
        /// <param name="align">One of the System.Drawing.ContentAlignment values.</param>
        /// <param name="color">A System.Drawing.Color to be used.</param>
        /// <remarks></remarks>
        public void AddText(string text, System.Drawing.Font font, System.Drawing.Color color, System.Drawing.ContentAlignment align)
        {
            AddText(text, font, color, align, 0, 0);
        }

        /// <summary>
        /// Draws an string with a single shadow on the current image. 
        /// </summary>
        /// <param name="text">A string with a text to be drawed.</param>
        /// <param name="font">The font to be used.</param>
        /// <param name="align">One of the System.Drawing.ContentAlignment values.</param>
        /// <param name="color">A System.Drawing.Color to be used on text color.</param>
        /// <param name="shadowColor">A System.Drawing.Color to be used on shadow color.</param>
        /// <param name="shadowOffset">How much the shadow will be offset of the text. Any value in pixels.</param>
        /// <remarks></remarks>
        public void AddTextWithShadow(string text, System.Drawing.Font font, System.Drawing.Color color, System.Drawing.ContentAlignment align, System.Drawing.Color shadowColor, int shadowOffset)
        {
            AddText(text, font, shadowColor, align, 0, 0);
            AddText(text, font, color, align, -shadowOffset, -shadowOffset);
        }


        #endregion

        #region " Borders "


        /// <summary>
        /// Draws a gradiend border.
        /// </summary>
        /// <param name="position">A bitwise combination of Position values.</param>
        /// <param name="color">The final color of the gradient.</param>
        /// <param name="size">Size in pixels of the border.</param>
        /// <remarks></remarks>
        public void AddGradientBorder(Position position, System.Drawing.Color color, int size)
        {
            Rectangle rect = new Rectangle();
            if (position == Position.None)
            {
                throw (new ArgumentException("Please provide one or more positions to add a gradient."));
            }

            if ((position | Position.Left) == position)
            {
                rect = new System.Drawing.Rectangle(0, 0, size, Bitmap.Height);
                AddGradientBorder(rect, color, System.Drawing.Color.Transparent, System.Drawing.Drawing2D.LinearGradientMode.Horizontal, 0, 0);
            }
            if ((position | Position.Right) == position)
            {
                rect = new System.Drawing.Rectangle(Bitmap.Width - size, 0, size, Bitmap.Height);
                AddGradientBorder(rect, System.Drawing.Color.Transparent, color, System.Drawing.Drawing2D.LinearGradientMode.Horizontal, -2, 0);
            }
            if ((position | Position.Top) == position)
            {
                rect = new System.Drawing.Rectangle(0, 0, Bitmap.Width, size);
                AddGradientBorder(rect, color, System.Drawing.Color.Transparent, System.Drawing.Drawing2D.LinearGradientMode.Vertical, 0, 0);
            }
            if ((position | Position.Bottom) == position)
            {
                rect = new System.Drawing.Rectangle(0, Bitmap.Height - size, Bitmap.Width, size);
                AddGradientBorder(rect, System.Drawing.Color.Transparent, color, System.Drawing.Drawing2D.LinearGradientMode.Vertical, 0, -2);
            }

        }

        private void AddGradientBorder(System.Drawing.Rectangle rect, System.Drawing.Color Color1, System.Drawing.Color Color2, System.Drawing.Drawing2D.LinearGradientMode Mode, int XOffset, int YOffset)
        {
            System.Drawing.Rectangle brect = new System.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);

            brect.X += XOffset;
            brect.Y += YOffset;

            System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(brect, Color1, Color2, Mode);
            brush.WrapMode = System.Drawing.Drawing2D.WrapMode.TileFlipXY;
            Graphics gr = System.Drawing.Graphics.FromImage(Bitmap);
            gr.FillRectangle(brush, rect);
            //gr.DrawRectangle(System.Drawing.Pens.Red, New System.Drawing.Rectangle(CInt(rect.X), CInt(rect.Y), CInt(rect.Width), CInt(rect.Height)))
            gr.Dispose();
        }


        #endregion

        #region " WaterMark "
        /// <summary>
        /// Draws a picture on the current image.
        /// </summary>
        /// <param name="bitmap">A bitmap.</param>
        /// <param name="position">A bitwise combination of Position values.</param>
        /// <remarks></remarks>
        public void AddPicture(Bitmap bitmap, Position position)
        {
            AddPicture(bitmap, position, 0);
        }


        /// <summary>
        /// Draws a picture on the current image.
        /// </summary>
        /// <param name="fileName">A filename of a file.</param>
        /// <param name="position">A bitwise combination of Position values.</param>
        /// <param name="margin">Some margin in pixels.</param>
        public void AddPicture(string fileName, Position position, int margin)
        {
            System.Drawing.Image bitmap = System.Drawing.Image.FromFile(fileName);
            AddPicture(((Bitmap)bitmap), position, margin);
        }

        /// <summary>
        /// Draws a picture on the current image.
        /// </summary>
        /// <param name="bitmap">A bitmap.</param>
        /// <param name="position">A bitwise combination of Position values.</param>
        /// <param name="margin">Some margin in pixels.</param>
        public void AddPicture(Bitmap bitmap, Position position, int margin)
        {
            Size size = bitmap.Size;
            Point pos = new Point();

            if ((position | Drawing.Position.Left) == position)
            {
                pos.X = 0 + margin;
            }
            else if ((position | Drawing.Position.Right) == position)
            {
                pos.X = this.Bitmap.Size.Width - size.Width + margin;
            }
            else
            {
                pos.X = (System.Convert.ToInt32((this.Bitmap.Size.Width / 2) - (size.Width / 2))) + margin;
            }

            if ((position | Drawing.Position.Top) == position)
            {
                pos.Y = 0 + margin;
            }
            else if ((position | Drawing.Position.Bottom) == position)
            {
                pos.Y = this.Bitmap.Size.Height - size.Height + margin;
            }
            else
            {
                pos.Y = (System.Convert.ToInt32((this.Bitmap.Size.Height / 2) - (size.Height / 2))) + margin;
            }

            Graphics gr = Graphics.FromImage(this.Bitmap);
            gr.DrawImage(bitmap, pos);
            gr.Dispose();
        }

        #endregion


        /// <summary>
        /// Creates a clone of the current image.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public object Clone()
        {
            Image res = new Image(Bitmap);
            res._Bitmap = (System.Drawing.Bitmap)(this._Bitmap.Clone());
            return res;
        }


        private bool _LowQuality = false;
        /// <summary>
        /// Gets or sets a boolean indicating that this image will be rendered in a low quality mode.
        /// </summary>
        public bool LowQuality
        {
            get
            {
                return _LowQuality;
            }
            set
            {
                _LowQuality = value;
            }
        }
    }


    #region " Enums "

    /// <summary> 
    /// Sets the alignment.
    /// </summary>
    /// <remarks></remarks>
    [Flags()]
    public enum Position
    {
        /// <summary>
        /// No alignment or center.
        /// </summary>
        /// <remarks></remarks>
        None,
        /// <summary>
        /// Left aligned.
        /// </summary>
        /// <remarks></remarks>
        Left,
        /// <summary>
        /// Right aligned.
        /// </summary>
        /// <remarks></remarks>
        Right,
        /// <summary>
        /// Top aligned.
        /// </summary>
        /// <remarks></remarks>
        Top,
        /// <summary>
        /// Bottom aligned.
        /// </summary>
        /// <remarks></remarks>
        Bottom
    }
    #endregion

}