namespace Al.Pdn.Common
{
    using PaintDotNet;
    using System;

    /// <summary>
    /// Represents a color in the YCbCr color space.
    /// </summary>
    /// <remarks>
    /// Y, Cb, and Cr are converted from R, G, and B as defined in CCIR Recommendation 601 but are normalized so as to occupy the full 256 levels of a 8-bit binary encoding.
    /// </remarks>
    public class YCbCrColor
    {
        /// <summary>
        /// The alpha channel value.
        /// </summary>
        public byte A;

        /// <summary>
        /// The Y channel value.
        /// </summary>
        public byte Y;

        /// <summary>
        /// The Cb channel value.
        /// </summary>
        public byte Cb;

        /// <summary>
        /// The Cr channel value.
        /// </summary>
        public byte Cr;

        /// <summary>
        /// Converts the YCbCr color to RGB.
        /// </summary>
        /// <returns>
        /// The RGB value of the color.
        /// </returns>
        /// <remarks>
        /// The RGB color is computed as :
        /// 
        /// R = Y + 1.402 (Cr - 128)
        /// G = Y - 0.34414 (Cb - 128) - 0.71414 (Cr - 128)
        /// B = Y + 1.772 (Cb - 128)
        /// </remarks>
        public ColorBgra ToRgb()
        {
            ColorBgra rgb = new ColorBgra();

            rgb.A = this.A;
            rgb.R = (byte)Math.Min(255, Math.Max(0, Math.Round((double)this.Y + 1.402 * (double)(this.Cr - 128))));
            rgb.G = (byte)Math.Min(255, Math.Max(0, Math.Round((double)this.Y - 0.34414 * (double)(this.Cb - 128) - 0.71414 * (double)(this.Cr - 128))));
            rgb.B = (byte)Math.Min(255, Math.Max(0, Math.Round((double)this.Y + 1.772 * (double)(this.Cb - 128))));

            return rgb;
        }

        /// <summary>
        /// Converts the RGB color YCbCr color.
        /// </summary>
        /// <param name="rgb">The RGB color to convert.</param>
        /// <returns>The YCbCr color.</returns>
        /// <remarks>
        /// The YCbCr is computed as :
        /// 
        /// Y = 0.299 R + 0.587 G + 0.114 B
        /// Cb = -0.1687 R - 0.3313 G + 0.5 B + 128
        /// Cr = 0.5 R - 0.4187 G - 0.0813 B + 128
        /// </remarks>
        public static YCbCrColor FromRgb(ColorBgra rgb)
        {
            YCbCrColor ycc = new YCbCrColor();

            ycc.A = rgb.A;
            ycc.Y = (byte)Math.Round(0.299 * (double)rgb.R + 0.587 * (double)rgb.G + 0.114 * (double)rgb.B);
            ycc.Cb = (byte)Math.Round(-0.1687 * (double)rgb.R - 0.3313 * (double)rgb.G + 0.5 * (double)rgb.B + 128);
            ycc.Cr = (byte)Math.Round(0.5 * (double)rgb.R - 0.4187 * (double)rgb.G - 0.0813 * (double)rgb.B + 128);

            return ycc;
        }
    }
}

