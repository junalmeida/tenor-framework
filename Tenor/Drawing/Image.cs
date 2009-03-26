using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;



namespace Tenor.Drawing
{
    public class Image : IO.BinaryFile, ICloneable, IImage
    {




        #region " Contrutores "

        /// <summary>
        /// Instancia a classe imagem
        /// </summary>
        /// <param name="Bitmap">Imagem original</param>
        /// <remarks></remarks>
        public Image(byte[] Bitmap)
            : base(Bitmap, "image/jpeg")
        {

            _Bitmap = new System.Drawing.Bitmap(new MemoryStream(Bitmap));
            RawFormat = _Bitmap.RawFormat;

        }


        /// <summary>
        /// Instancia a classe imagem
        /// </summary>
        /// <param name="Bitmap">Imagem original</param>
        /// <remarks></remarks>
        public Image(System.Drawing.Bitmap Bitmap)
            : base(null, "image/jpeg")
        {
            MemoryStream mem = new MemoryStream();
            Bitmap.Save(mem, Bitmap.RawFormat);
            _buffer = StreamToBytes(mem);

            _Bitmap = Bitmap;
            RawFormat = Bitmap.RawFormat;
        }

        /// <summary>
        /// Instancia a classe imagem
        /// </summary>
        /// <param name="Bitmap">Imagem original</param>
        /// <param name="Width">Largura nova em pixels</param>
        /// <param name="Height">Altura nova em pixels</param>
        /// <remarks>Especifique 0 (zero) para largura ou altura para fazer um redimensionamento proporcional</remarks>
        public Image(byte[] Bitmap, int Width, int Height)
            : this(Bitmap)
        {
            if (Width <= 0 && Height > 0)
            {
                ResizeByHeight(Height);
            }
            else if (Width > 0 && Height <= 0)
            {
                ResizeByWidth(Width);
            }
            else
            {
                Resize(Width, Height);
            }
        }

        /// <summary>
        /// Instancia a classe imagem
        /// </summary>
        /// <param name="Bitmap">Imagem original</param>
        /// <remarks></remarks>
        public Image(Stream Bitmap)
            : this(StreamToBytes(Bitmap))
        {
        }

        /// <summary>
        /// Instancia a classe imagem
        /// </summary>
        /// <param name="Bitmap">Imagem original</param>
        /// <param name="Width">Largura nova em pixels</param>
        /// <param name="Height">Altura nova em pixels</param>
        /// <remarks>Especifique 0 (zero) para largura ou altura para fazer um redimensionamento proporcional</remarks>
        public Image(Stream Bitmap, int Width, int Height)
            : this(Bitmap)
        {
            if (Width <= 0 && Height > 0)
            {
                ResizeByHeight(Height);
            }
            else if (Width > 0 && Height <= 0)
            {
                ResizeByWidth(Width);
            }
            else
            {
                Resize(Width, Height);
            }
        }

        /// <summary>
        /// Instancia a classe imagem
        /// </summary>
        /// <param name="Bitmap">Imagem original</param>
        /// <param name="Width">Largura nova em pixels</param>
        /// <param name="Height">Altura nova em pixels</param>
        /// <remarks>Especifique 0 (zero) para largura ou altura para fazer um redimensionamento proporcional</remarks>
        public Image(System.Drawing.Bitmap Bitmap, int Width, int Height)
            : this(Bitmap)
        {
            if (Width <= 0 && Height > 0)
            {
                ResizeByHeight(Height);
            }
            else if (Width > 0 && Height <= 0)
            {
                ResizeByWidth(Width);
            }
            else
            {
                Resize(Width, Height);
            }
        }


        #endregion

        #region " Propriedades "

        private System.Drawing.Bitmap _Bitmap;
        private System.Drawing.Imaging.ImageFormat RawFormat;

        /// <summary>
        /// Retorna a imagem representada por este objeto
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

        public int Width
        {
            get
            {
                return Bitmap.Width;
            }
        }

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
        /// Redimensiona a imagem proporcionalmente por largura
        /// </summary>
        /// <param name="Width">Nova largura em pixels</param>
        /// <remarks></remarks>
        public void ResizeByWidth(int Width)
        {
            Resize(Width, 0, ResizeMode.Proportional);

        }

        /// <summary>
        /// Redimensiona a imagem proporcionalmente por altura
        /// </summary>
        /// <param name="Height">Nova altura em pixels</param>
        /// <remarks></remarks>
        public void ResizeByHeight(int Height)
        {
            Resize(0, Height, ResizeMode.Proportional);
        }

        /// <summary>
        /// Redimensiona a imagem proporcionalmente
        /// </summary>
        /// <param name="Percent">Porcentagem para redimensionamento</param>
        /// <remarks></remarks>
        public void ResizeByPercent(int Percent)
        {
            int width = Percent * this.Width / 100;
            int height = Percent * this.Height / 100;
            Resize(width, height, ResizeMode.Stretch);
        }


        /// <summary>
        /// Redimensiona a imagem
        /// </summary>
        /// <param name="Size">Estrutura com largura e altura definidas</param>
        /// <remarks></remarks>
        public void Resize(System.Drawing.Size Size)
        {
            Resize(Size.Width, Size.Height, ResizeMode.Stretch);
        }

        /// <summary>
        /// Redimensiona a imagem
        /// </summary>
        /// <param name="Width">Nova largura em pixels</param>
        /// <param name="Height">Nova altura em pixels</param>
        /// <param name="Mode">Um dos modos de redimensionamento</param>
        /// <remarks></remarks>
        public void Resize(int Width, int Height, ResizeMode Mode)
        {
            Rectangle rect = new Rectangle();
            Size imagemSize = new Size();
            if (Width <= 0 && Height <= 0)
            {
                throw (new ArgumentException("Invalid resizing parameters. Check width and height values"));
            }

            switch (Mode)
            {
                case Drawing.ResizeMode.Stretch:
                case Drawing.ResizeMode.Crop:
                    if (Width <= 0 || Height <= 0)
                    {
                        throw (new ArgumentException("Invalid image parameters. Check width and height values"));
                    }
                    break;
                case Drawing.ResizeMode.Proportional:
                    if (Width <= 0 && Height <= 0)
                    {
                        throw (new ArgumentException("Invalid image parameters. Check width and height values"));
                    }
                    break;
            }



            switch (Mode)
            {
                case ResizeMode.Stretch:
                    rect = new Rectangle(0, 0, Width, Height);
                    //tamanho da imagem final
                    imagemSize = new Size(Width, Height);
                    break;
                case ResizeMode.Crop:
                    //Novo tamanho baseado na largura especificada

                    Size sizeL = new Size();
                    sizeL.Width = Width;
                    sizeL.Height = Convert.ToInt32((double)Width / (double)this.Bitmap.Width * (double)this.Bitmap.Height);

                    //Novo tamanho baseado na altura especificada
                    Size sizeH = new Size();
                    sizeH.Height = Height;
                    sizeH.Width = Convert.ToInt32((double)Height / (double)this.Bitmap.Height * (double)this.Bitmap.Width);


                    rect = new Rectangle();
                    //Selecionar por largura ou por altura para nÃ£o haver sobras (espaÃ§os em branco) na nova imagem
                    if (sizeL.Width >= Width && sizeL.Height >= Height)
                    {
                        rect.Size = sizeL;
                    }
                    else
                    {
                        rect.Size = sizeH;
                    }

                    //Definir ponto de origem para que a imagem reduzida fique centralizada
                    rect.Location = new Point(System.Convert.ToInt32(-(rect.Size.Width - Width) / 2), System.Convert.ToInt32(-(rect.Size.Height - Height) / 2));

                    //tamanho da imagem final
                    imagemSize = new Size(Width, Height);
                    break;

                case ResizeMode.Proportional:
                    if (Width > 0)
                    {


                        int percent = 100 * Width / this.Width;
                        rect = new Rectangle(0, 0, Width, percent * this.Height / 100);

                        if (Height > 0 && rect.Height > Height)
                        {
                            percent = 100 * Height / this.Height;
                            rect = new Rectangle(0, 0, percent * this.Width / 100, Height);
                        }


                    }
                    else if (Height > 0)
                    {
                        int percent = 100 * Height / this.Height;
                        rect = new Rectangle(0, 0, percent * this.Width / 100, Height);

                        if (Width > 0 && rect.Width > Width)
                        {
                            percent = 100 * Width / this.Width;
                            rect = new Rectangle(0, 0, Width, percent * this.Height / 100);
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
        /// Redimensiona a imagem
        /// </summary>
        /// <param name="Width">Nova largura em pixels</param>
        /// <param name="Height">Nova altura em pixels</param>
        /// <remarks></remarks>
        public void Resize(int Width, int Height)
        {
            Resize(Width, Height, ResizeMode.Stretch);
        }


        /// <summary>
        /// Redimensiona a imagem respeitando as proporÃ§Ãµes e corta as sobras de forma a centralizar a imagem.
        /// </summary>
        /// <param name="Size">Uma estrutura <see cref="Size">Size</see> que define o novo tamanho da imagem.</param>
        /// <remarks></remarks>
        [Obsolete("Use Resize instead", false)]
        public void ResizeAndCrop(Size Size)
        {
            Resize(Size.Width, Size.Height, ResizeMode.Stretch);
        }



        /// <summary>
        /// Redimensiona a imagem respeitando as proporÃ§Ãµes e corta as sobras de forma a centralizar a imagem.
        /// </summary>
        /// <param name="width">Nova largura em pixels</param>
        /// <param name="height">Nova altura em pixels</param>
        /// <remarks></remarks>
        [Obsolete("Use Resize instead", false)]
        public void ResizeAndCrop(int width, int height)
        {
            Resize(width, height, ResizeMode.Stretch);


            //Dim bmp As New Bitmap(width, height)

            //'Novo tamanho baseado na largura especificada
            //Dim sizeL As New Size
            //sizeL.Width = width
            //sizeL.Height = CInt(width / Me.Bitmap.Width * Me.Bitmap.Height)

            //'Novo tamanho baseado na altura especificada
            //Dim sizeH As New Size
            //sizeH.Height = height
            //sizeH.Width = CInt(height / Me.Bitmap.Height * Me.Bitmap.Width)


            //Dim rect As New Rectangle
            //'Selecionar por largura ou por altura para nÃ£o haver sobras (espaÃ§os em branco) na nova imagem
            //If sizeL.Width >= width And sizeL.Height >= height Then
            //    rect.Size = sizeL
            //Else
            //    rect.Size = sizeH
            //End If

            //'Definir ponto de origem para que a imagem reduzida fique centralizada
            //rect.Location = New Point(CInt(-(rect.Size.Width - width) / 2), CInt(-(rect.Size.Height - height) / 2))


            //'ConfiguraÃ§Ãµes de renderizaÃ§Ã£o
            //Dim gr As Graphics = Graphics.FromImage(bmp)
            //gr.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            //gr.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            //gr.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
            //gr.CompositingQuality = Drawing2D.CompositingQuality.HighQuality

            //'Desenhar a nova imagem
            //gr.DrawImage(Me.Bitmap, rect)



            //gr.Dispose()

            //Me._Bitmap.Dispose()
            //Me._Bitmap = bmp


        }


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


        override public System.IO.Stream WriteContent()
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

        /// <summary>
        /// Cria uma miniatura da imagem atual
        /// </summary>
        /// <param name="Page">PÃ¡gina onde serÃ¡ exibida</param>
        /// <param name="Width">Largura da miniatura desejada</param>
        /// <param name="Height">Altura da miniatura desejada</param>
        /// <returns></returns>
        /// <remarks>Defina a largura ou altura em zero para fazer o redimensinamento proporcional. Definir ambos em zero cria uma imagem com 110 pixels de largura</remarks>
        [Obsolete("Usa o outro overload")]
        public string GetThumbImageUrl(System.Web.UI.Page Page, int Width, int Height)
        {
            Image img = new Image((System.Drawing.Bitmap)(this.Bitmap.Clone()));

            if (Width > 0 && Height > 0)
            {
                img.Resize(Width, Height);
            }
            else if (Width > 0)
            {
                img.ResizeByWidth(Width);
            }
            else if (Height > 0)
            {
                img.ResizeByHeight(Height);
            }
            else
            {
                img.ResizeByWidth(110);
            }
            return img.GetFileUrl(Page);

        }

        /// <summary>
        /// Cria uma miniatura da imagem atual
        /// </summary>
        /// <param name="Context">Contexto</param>
        /// <param name="Width">Largura da miniatura desejada</param>
        /// <param name="Height">Altura da miniatura desejada</param>
        /// <returns></returns>
        /// <remarks>Defina a largura ou altura em zero para fazer o redimensinamento proporcional. Definir ambos em zero cria uma imagem com 110 pixels de largura</remarks>
        public string GetThumbImageUrl(System.Web.HttpContext Context, int Width, int Height)
        {
            Image img = new Image((System.Drawing.Bitmap)(this.Bitmap.Clone()));

            if (Width > 0 && Height > 0)
            {
                img.Resize(Width, Height);
            }
            else if (Width > 0)
            {
                img.ResizeByWidth(Width);
            }
            else if (Height > 0)
            {
                img.ResizeByHeight(Height);
            }
            else
            {
                img.ResizeByWidth(110);
            }
            return img.GetFileUrl(Context);
        }

        #endregion

        #region " Text "


        /// <summary>
        /// Adiciona um texto à imagem atual
        /// </summary>
        /// <param name="Text">Texto a ser adicionado</param>
        /// <param name="FontFamily">Nome da fonte usada no texto</param>
        /// <param name="Size">Tamanho em Pontos (pt)</param>
        /// <param name="FontStyle">Um dos valores de System.Drawing.FontStyle</param>
        /// <param name="Align">Um dos valores de System.Drawing.ContentAlignment</param>
        /// <param name="Color">A Cor usada para renderizar o texto</param>
        /// <remarks></remarks>
        public void AddText(string Text, string FontFamily, float Size, System.Drawing.FontStyle FontStyle, System.Drawing.Color Color, System.Drawing.ContentAlignment Align)
        {
            System.Drawing.Font font = new System.Drawing.Font(FontFamily, Size, FontStyle, System.Drawing.GraphicsUnit.Point);
            AddText(Text, font, Color, Align);
        }

        /// <summary>
        /// Adiciona um texto à imagem atual
        /// </summary>
        /// <param name="Text">Texto a ser adicionado</param>
        /// <param name="Align">Um dos valores de System.Drawing.ContentAlignment</param>
        /// <param name="Color">A Cor usada para o texto</param>
        /// <param name="Font">A fonte usada no texto</param>
        /// <param name="OffsetX">Pixels para deslocar o texto horizontalmente. Valores negativos deslocam para esquerda.</param>
        /// <param name="OffsetY">Pixels para deslocar o texto verticalmente. Valores negativos deslocam para cima.</param>
        /// <remarks></remarks>
        public void AddText(string Text, System.Drawing.Font Font, System.Drawing.Color Color, System.Drawing.ContentAlignment Align, int OffsetX, int OffsetY)
        {
            Graphics gr = System.Drawing.Graphics.FromImage(Bitmap);
            SizeF size = gr.MeasureString(Text, Font, Bitmap.Width);
            PointF lic = new PointF();
            switch (Align)
            {
                case System.Drawing.ContentAlignment.TopLeft:
                    //alinhar acima e à esquerda
                    lic = new System.Drawing.PointF(0, 0);
                    break;
                case System.Drawing.ContentAlignment.TopCenter:
                    //alinhar acima e ao centro
                    lic = new System.Drawing.PointF(System.Convert.ToSingle((Bitmap.Width / 2) - (size.Width / 2)), 0);
                    break;
                case System.Drawing.ContentAlignment.TopRight:
                    //alinhar acima e à direita
                    lic = new System.Drawing.PointF(Bitmap.Width - size.Width, 0);
                    break;
                case System.Drawing.ContentAlignment.MiddleLeft:
                    //alinhar ao meio e à esquerda
                    lic = new System.Drawing.PointF(0, System.Convert.ToSingle((Bitmap.Height / 2) - (size.Height / 2)));
                    break;
                case System.Drawing.ContentAlignment.MiddleCenter:
                    //alinhar ao meio e ao centro
                    lic = new System.Drawing.PointF(System.Convert.ToSingle((Bitmap.Width / 2) - (size.Width / 2)), System.Convert.ToSingle((Bitmap.Height / 2) - (size.Height / 2)));
                    break;
                case System.Drawing.ContentAlignment.MiddleRight:
                    //alinhar ao meio e à direita
                    lic = new System.Drawing.PointF(Bitmap.Width - size.Width, System.Convert.ToSingle((Bitmap.Height / 2) - (size.Height / 2)));
                    break;
                case System.Drawing.ContentAlignment.BottomLeft:
                    //alinhar abaixo e à esquerda
                    lic = new System.Drawing.PointF(0, Bitmap.Height - size.Height);
                    break;
                case System.Drawing.ContentAlignment.BottomCenter:
                    //alinhar abaixo e ao centro
                    lic = new System.Drawing.PointF(System.Convert.ToSingle((Bitmap.Width / 2) - (size.Width / 2)), Bitmap.Height - size.Height);
                    break;
                case System.Drawing.ContentAlignment.BottomRight:
                    //alinhar abaixo e à direita
                    lic = new System.Drawing.PointF(Bitmap.Width - size.Width, Bitmap.Height - size.Height);
                    break;
            }
            //renderiza o texto na tela
            lic.X += OffsetX;
            lic.Y += OffsetY;
            gr.DrawString(Text, Font, new System.Drawing.SolidBrush(Color), lic);
            gr.Dispose();
        }

        /// <summary>
        /// Adiciona um texto à imagem atual
        /// </summary>
        /// <param name="Text">Texto a ser adicionado</param>
        /// <param name="Align">Um dos valores de System.Drawing.ContentAlignment</param>
        /// <param name="Color">A Cor usada para o texto</param>
        /// <param name="Font">A fonte usada no texto</param>
        /// <remarks></remarks>
        public void AddText(string Text, System.Drawing.Font Font, System.Drawing.Color Color, System.Drawing.ContentAlignment Align)
        {
            AddText(Text, Font, Color, Align, 0, 0);
        }

        /// <summary>
        /// Adiciona um texto com sombra à imagem atual
        /// </summary>
        /// <param name="Text">Texto a ser adicionado</param>
        /// <param name="Align">Um dos valores de System.Drawing.ContentAlignment</param>
        /// <param name="Font">A fonte usada no texto</param>
        /// <param name="Color">A Cor usada para o texto</param>
        /// <param name="ShadowColor">A Cor usada para a sombra</param>
        /// <remarks></remarks>
        public void AddTextWithShadow(string Text, System.Drawing.Font Font, System.Drawing.Color Color, System.Drawing.ContentAlignment Align, System.Drawing.Color ShadowColor, int ShadowOffset)
        {
            AddText(Text, Font, ShadowColor, Align, 0, 0);
            AddText(Text, Font, Color, Align, -ShadowOffset, -ShadowOffset);
        }


        #endregion

        #region " Borders "


        /// <summary>
        /// Adiciona uma borda com degradÃª de transparente atÃ© a cor escolhida
        /// </summary>
        /// <param name="Position">Um ou mais itens de Position. Utilize combinaÃ§Ã£o Ã  bits para escolher suas opÃ§Ãµes. (Or no VB, | no C#)</param>
        /// <param name="Color">A cor de destino do degradÃª</param>
        /// <param name="Size">Tamanho em pixels da borda</param>
        /// <remarks></remarks>
        public void AddGradientBorder(Position Position, System.Drawing.Color Color, int Size)
        {
            Rectangle rect = new Rectangle();
            if (Position == Position.None)
            {
                throw (new ArgumentException("Please provide one or more positions to add a gradient."));
            }

            if ((Position | Position.Left) == Position)
            {
                //Se esquerda foi selecionada:
                rect = new System.Drawing.Rectangle(0, 0, Size, Bitmap.Height);
                AddGradientBorder(rect, Color, System.Drawing.Color.Transparent, System.Drawing.Drawing2D.LinearGradientMode.Horizontal, 0, 0);
            }
            if ((Position | Position.Right) == Position)
            {
                //Se direita foi selecionada:
                rect = new System.Drawing.Rectangle(Bitmap.Width - Size, 0, Size, Bitmap.Height);
                AddGradientBorder(rect, System.Drawing.Color.Transparent, Color, System.Drawing.Drawing2D.LinearGradientMode.Horizontal, -2, 0);
            }
            if ((Position | Position.Top) == Position)
            {
                //Se topo foi selectionado:
                rect = new System.Drawing.Rectangle(0, 0, Bitmap.Width, Size);
                AddGradientBorder(rect, Color, System.Drawing.Color.Transparent, System.Drawing.Drawing2D.LinearGradientMode.Vertical, 0, 0);
            }
            if ((Position | Position.Bottom) == Position)
            {
                //Se fundo foi selecionado:
                rect = new System.Drawing.Rectangle(0, Bitmap.Height - Size, Bitmap.Width, Size);
                AddGradientBorder(rect, System.Drawing.Color.Transparent, Color, System.Drawing.Drawing2D.LinearGradientMode.Vertical, 0, -2);
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
        /// Adiciona uma imagem sobre a imagem atual.
        /// Use preferencialmente imagens PNG.
        /// </summary>
        /// <param name="Bitmap">Imagem a ser adicionada.</param>
        /// <param name="Position">PosiÃ§Ã£o desejada.</param>
        /// <remarks></remarks>
        public void AddPicture(Bitmap Bitmap, Position Position)
        {
            AddPicture(Bitmap, Position, 0);
        }


        /// <summary>
        /// Adiciona uma imagem sobre a imagem atual.
        /// Use preferencialmente imagens PNG.
        /// </summary>
        /// <param name="FileName">Imagem a ser adicionada.</param>
        /// <param name="Position">PosiÃ§Ã£o desejada.</param>
        /// <param name="Margin">Margem desejada</param>
        /// <remarks></remarks>
        public void AddPicture(string FileName, Position Position, int Margin)
        {
            System.Drawing.Image bitmap = System.Drawing.Image.FromFile(FileName);
            AddPicture(((Bitmap)bitmap), Position, Margin);
        }

        /// <summary>
        /// Adiciona uma imagem sobre a imagem atual.
        /// Use preferencialmente imagens PNG.
        /// </summary>
        /// <param name="Bitmap">Imagem a ser adicionada.</param>
        /// <param name="Position">PosiÃ§Ã£o desejada.</param>
        /// <param name="Margin">Margem desejada</param>
        /// <remarks></remarks>
        public void AddPicture(Bitmap Bitmap, Position Position, int Margin)
        {
            Size size = Bitmap.Size;
            Point pos = new Point();

            if ((Position | Drawing.Position.Left) == Position)
            {
                pos.X = 0 + Margin;
            }
            else if ((Position | Drawing.Position.Right) == Position)
            {
                pos.X = this.Bitmap.Size.Width - size.Width + Margin;
            }
            else
            {
                pos.X = (System.Convert.ToInt32((this.Bitmap.Size.Width / 2) - (size.Width / 2))) + Margin;
            }

            if ((Position | Drawing.Position.Top) == Position)
            {
                pos.Y = 0 + Margin;
            }
            else if ((Position | Drawing.Position.Bottom) == Position)
            {
                pos.Y = this.Bitmap.Size.Height - size.Height + Margin;
            }
            else
            {
                pos.Y = (System.Convert.ToInt32((this.Bitmap.Size.Height / 2) - (size.Height / 2))) + Margin;
            }

            Graphics gr = Graphics.FromImage(this.Bitmap);
            gr.DrawImage(Bitmap, pos);
            gr.Dispose();
        }

        #endregion

        /// <summary>
        /// Retorna uma nova Stream com a imagem no seu estado atual.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override System.IO.Stream GetStream()
        {
            return WriteContent();
        }

        /// <summary>
        /// Faz uma cÃ³pia da instancia atual da imagem.
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
    /// ContÃ©m posiÃ§Ãµes de posicionamento de itens em geral
    /// </summary>
    /// <remarks></remarks>
    [Flags()]
    public enum Position
    {
        /// <summary>
        /// Sem alinhamento ou centralizado
        /// </summary>
        /// <remarks></remarks>
        None,
        /// <summary>
        /// Alinha à esquerda
        /// </summary>
        /// <remarks></remarks>
        Left,
        /// <summary>
        /// Alinha à direita
        /// </summary>
        /// <remarks></remarks>
        Right,
        /// <summary>
        /// Alinha ao topo
        /// </summary>
        /// <remarks></remarks>
        Top,
        /// <summary>
        /// Alinha ao fundo
        /// </summary>
        /// <remarks></remarks>
        Bottom
    }
    #endregion

}