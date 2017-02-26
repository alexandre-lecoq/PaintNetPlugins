namespace Al.Pdn.Thresholding.Otsu1979
{
    using PaintDotNet;
    using PaintDotNet.Effects;
    using PaintDotNet.PropertySystem;
    using System;
    using System.Drawing;

    /// <summary>
    /// Represents an effect thresholding the picture using original Otsu's method.
    /// </summary>
    public class Otsu1979 : PropertyBasedEffect
    {
        private bool _initialized = false;
        private int _threshold;

        /// <summary>
        /// Initializes a new instance of the <see cref="Otsu1979"/> class.
        /// </summary>
        public Otsu1979()
            : base(StaticName, StaticImage, StaticSubMenuName, EffectFlags.None)
        {
        }

        private static Image StaticImage
        {
            get { return null; }
        }

        private static string StaticName
        {
            get { return "Otsu 1979 thresholding"; }
        }

        private static string StaticSubMenuName
        {
            get { return "Stylize"; }
        }

        /// <summary>
        /// Gets the effect's property collection.
        /// </summary>
        /// <returns>Returns an empty <see cref="PropertyCollection"/>.</returns>
        protected override PropertyCollection OnCreatePropertyCollection()
        {
            return PropertyCollection.CreateEmpty();
        }

        /// <summary>
        /// Sets global effect parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="dstArgs">The destination picture arguments.</param>
        /// <param name="srcArgs">The source picture arguments.</param>
        protected override void OnSetRenderInfo(PropertyBasedEffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            if (_initialized == false)
            {
                _threshold = ComputeThresholding(srcArgs.Surface);
                _initialized = true;
            }
        }

        private int ComputeThresholding(Surface sourceSurface)
        {
            // TODO : Normalize histogram to [0:1] double typed value ???
            var histogram = new int[256];

            var w0max = (long)sourceSurface.Width * (long)sourceSurface.Height;

            for (var x = 0; x < sourceSurface.Width; x++)
            {
                for (var y = 0; y < sourceSurface.Height; y++)
                {
                    var greenPixelValue = sourceSurface[x, y].G;
                    histogram[greenPixelValue]++;
                }
            }

            long uT = 0;
            for (var i = 1; i < 255 + 1; i++)
            {
                uT += i * histogram[i];
            }

            long w0u0 = 0;
            long w0 = 0;
            long maxSigma = 0;
            var threshold = 0;

            for (var i = 0; i < 255; ++i)
            {
                w0 += histogram[i];

                if (w0 == 0)
                    continue;

                var w1 = w0max - w0;

                if (w1 == 0)
                    break;

                w0u0 += i * histogram[i];
                var u0 = w0u0 / w0;
                var u1 = (uT - w0u0) / w1;
                var sigma = w0 * w1 * (u0 - u1) * (u0 - u1);

                if (sigma >= maxSigma)
                {
                    threshold = i;
                    maxSigma = sigma;
                }
            }

            return threshold;
        }

        /// <summary>
        /// Renders part of the picture.
        /// </summary>
        /// <param name="rois">The regions to render.</param>
        /// <param name="startIndex">The index of the first region to render.</param>
        /// <param name="length">The number of regions to render.</param>
        protected override void OnRender(Rectangle[] rois, int startIndex, int length)
        {
            if (length != 0)
            {
                for (int i = startIndex; i < (startIndex + length); i++)
                {
                    this.Render(base.DstArgs.Surface, base.SrcArgs.Surface, rois[i]);
                }
            }
        }

        private void Render(Surface dst, Surface src, Rectangle rect)
        {
            for (int x = rect.Left; x < rect.Right; x++)
            {
                for (int y = rect.Top; y < rect.Bottom; y++)
                {
                    if (src.IsVisible(x, y))
                    {
                        var sourceColor = src[x, y];

                        if (sourceColor.G >= _threshold)
                        {
                            dst[x, y] = ColorBgra.White;
                        }
                        else
                        {
                            dst[x, y] = ColorBgra.Black;
                        }
                    }
                }
            }
        }
    }
}
