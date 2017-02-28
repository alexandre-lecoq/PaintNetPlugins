namespace Al.Pdn.Colorization.Inpainting
{
    using Al.Pdn.Common;
    using PaintDotNet;
    using PaintDotNet.Effects;
    using PaintDotNet.IndirectUI;
    using PaintDotNet.PropertySystem;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Reflection;

    /// <summary>
    /// Implements a colorization plugin.
    /// </summary>
    /// <remarks>
    /// This implements the algorithm described in :
    /// Liron Yatziv and Guillermo Sapiro, "Fast image and video colorization using chrominance blending", IEEE Transactions on Image Processing, VOL. 15, NO. 5, MAY 2006.
    /// 
    /// You may also check :
    /// Guillermo Sapiro, "Inpainting the colors", IMA Preprint Series, May 2004. (www.ima.umn.edu), Also in IEEE International Conference Image Processing, Genoa, Italy, September 2005.
    /// </remarks>
    public class ColorInpaintingFilter : PropertyBasedEffect
    {
        private YCbCrColor[][] picture = null;
        private List<Blend>[][] blendMap = null;
        private bool _initialized = false;
        private int _blendFactor = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorInpaintingFilter"/> class.
        /// </summary>
        public ColorInpaintingFilter()
            : base(StaticName, StaticImage, StaticSubMenuName, EffectFlags.Configurable)
        {
        }

        private static Image StaticImage
        {
            get { return Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Al.Pdn.Colorization.Inpainting.ColorBucketIcon2.png")); }
        }

        private static string StaticName
        {
            get { return "Color inpainting"; }
        }

        private static string StaticSubMenuName
        {
            get { return "Color"; }
        }

        /// <summary>
        /// Gets the effect's property collection.
        /// </summary>
        /// <returns>Returns an empty <see cref="PropertyCollection"/>.</returns>
        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> properties = new List<Property>();

            properties.Add(new Int32Property("BlendFactor", 4, 1, 6));

            return new PropertyCollection(properties);
        }

        /// <summary>
        /// Sets the parameter controls.
        /// </summary>
        /// <param name="props">The property collection.</param>
        /// <returns>The control informations.</returns>
        protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
        {
            ControlInfo info = PropertyBasedEffect.CreateDefaultConfigUI(props);

            info.SetPropertyControlValue("BlendFactor", ControlInfoPropertyNames.DisplayName, "Blend factor");
            
            return info;
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
                picture = ToYCbCr(srcArgs.Surface);
                blendMap = Colorizer.ComputeBlendMap(picture, () => IsCancelRequested);

                if (blendMap != null)
                    _initialized = true;
            }

            _blendFactor = parameters.GetProperty<Int32Property>("BlendFactor").Value;
        }

        private static YCbCrColor[][] ToYCbCr(Surface source)
        {
            YCbCrColor[][] picture = new YCbCrColor[source.Width][];

            for (int x = 0; x < picture.Length; x++)
                picture[x] = new YCbCrColor[source.Height];

            for (int x = 0; x < picture.Length; x++)
                for (int y = 0; y < picture[x].Length; y++)
                    picture[x][y] = YCbCrColor.FromRgb(source[x, y]);

            return picture;
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
                    List<Blend> blendList = blendMap[x][y];

                    double cBnumerator = 0;
                    double cRnumerator = 0;
                    double denominator = 0;

                    foreach (Blend blend in blendList)
                    {
                        double factor = GetWeightFactor(blend.Distance);
                        cBnumerator += factor * (double)blend.Chrominance.Cb;
                        cRnumerator += factor * (double)blend.Chrominance.Cr;
                        denominator += factor;
                    }

                    picture[x][y].Cb = (byte)Math.Round(cBnumerator / denominator);
                    picture[x][y].Cr = (byte)Math.Round(cRnumerator / denominator);

                    dst[x, y] = picture[x][y].ToRgb();
                }
            }
        }

        private double GetWeightFactor(int distance)
        {
            if (distance == 0)
                return 2.0;

            return Math.Pow(distance, -_blendFactor);
        }
    }
}
