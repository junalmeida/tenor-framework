using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Tenor.Drawing
{
    public enum BarCodeMode 
    {
        /// <summary>
        /// Generates a barcode on a 2 of 5 way.
        /// </summary>
        TwoOfFive
    }

    /// <summary>
    /// Generates a bitmap with a barcode representation.
    /// </summary>
    public sealed class BarCode
    {
        public BarCode(int number)
            : this(Convert.ToDecimal(number))
        { }

        public BarCode(long number)
            : this(Convert.ToDecimal(number))
        { }


        public BarCode(decimal number)
            : this(number, BarCodeMode.TwoOfFive)
        { }

        public BarCode(decimal number, BarCodeMode mode)
        {
            //if (number < 0)
                //number = System.Math.Abs(number);
            if (number == 0)
                throw new ArgumentOutOfRangeException("number");

            this.number = number;
            this.mode = mode;
        }

        decimal number;
        BarCodeMode mode;


        private int height = 50;

        /// <summary>
        /// Gets or sets the height of this barcode.
        /// </summary>
        public int Height
        {
            get { return height; }
            set
            {
                if (value < 10)
                    throw new ArgumentOutOfRangeException();
                height = value;
            }
        }

        private System.Drawing.Image GenerateTwoOfFive()
        {
            const int thin = 1;
            const int large = 3;

            string[] barcodes = new string[100];
            barcodes[0] = "00110";
            barcodes[1] = "10001";
            barcodes[2] = "01001";
            barcodes[3] = "11000";
            barcodes[4] = "00101";
            barcodes[5] = "10100";
            barcodes[6] = "01100";
            barcodes[7] = "00011";
            barcodes[8] = "10010";
            barcodes[9] = "01010";
            for (int f1 = 9; f1 >= 0; f1--)
                for (int f2 = 9; f2 >= 0; f2--)
                {
                    int f = f1 * 10 + f2;
                    string text = string.Empty;
                    for (int i = 0; i < 4; i++)
                        text = text + barcodes[f1].Substring(i, 1) + barcodes[f2].Substring(i, 1);

                    barcodes[f] = text;
                }
            //Creating bitmap, graphics and brushes for barcode
            SolidBrush brushBlack = new SolidBrush(Color.Black);
            SolidBrush brushWhite = new SolidBrush(Color.White);


            int left = 0;
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(500, height);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
            {
                //Generating barcode header
                for (int i = 0; i < 4; i++)
                {
                    Brush brush = (i % 2 == 0 ? brushBlack : brushWhite);
                    g.FillRectangle(brush, new Rectangle(left, 0, thin, height));
                    left += thin;
                }
                //Generating barcode contents
                string text = number.ToString();
                if (text.Length % 2 != 0)
                    text = "0" + text;
                while (text.Length > 0)
                {
                    int index = Convert.ToInt32(text.Substring(0, 2));

                    text = text.Substring(2);
                    string f = barcodes[index];

                    for (int i = 0; i < f.Length; i += 2)
                    {

                        int bSize;
                        if (f.Substring(i, 1) == "0")
                            bSize = thin;
                        else
                            bSize = large;

                        g.FillRectangle(brushBlack, new Rectangle(left, 0, bSize, height));
                        left += bSize;

                        if (f.Substring(i + 1, 1) == "0")
                            bSize = thin;
                        else
                            bSize = large;

                        g.FillRectangle(brushWhite, new Rectangle(left, 0, bSize, height));
                        left += bSize;
                    }
                }
                //Generating barcode trailing
                g.FillRectangle(brushBlack, new Rectangle(left, 0, large, height));
                left += large;
                g.FillRectangle(brushWhite, new Rectangle(left, 0, thin, height));
                left += thin;
                g.FillRectangle(brushBlack, new Rectangle(left, 0, 1, height));
                left += 1;
            }

            Bitmap bmp2 = new Bitmap(left, height);
            using (Graphics g = Graphics.FromImage(bmp2))
            {
                g.DrawImage(bmp, 0, 0);
            }

            bmp.Dispose();
            return bmp2;
        }

        /// <summary>
        /// Generates an image that contains the barcode representation of this instance.
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Image Generate()
        {
            switch (mode)
            {
                case BarCodeMode.TwoOfFive:
                    return GenerateTwoOfFive();
                default:
                    throw new InvalidOperationException();
            }
        }

        public override string ToString()
        {
            return number.ToString();
        }
    }
}
