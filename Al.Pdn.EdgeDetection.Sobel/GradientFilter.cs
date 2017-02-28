namespace Al.Pdn.EdgeDetection.Sobel
{
    using Common;
    using PaintDotNet.Effects;
    using PaintDotNet.PropertySystem;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Reflection;

    /// <summary>
    /// Represents an effect detecting edges in the picture using a Sobel filter.
    /// </summary>
    public class GradientFilter : PropertyBasedEffect
    {
        private SobelConvolutionPool _convolutionPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientFilter"/> class.
        /// </summary>
        public GradientFilter()
            : base(StaticName, StaticImage, StaticSubMenuName, EffectFlags.SingleThreaded)
        {
            _convolutionPool = new SobelConvolutionPool();
        }

        private static Image StaticImage
        {
            get { return Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Al.Pdn.EdgeDetection.Sobel.GradientIcon.png")); }
        }

        private static string StaticName
        {
            get { return "Gradient filter"; }
        }

        private static string StaticSubMenuName
        {
            get { return "Stylize"; }
        }

        /// <summary>
        /// Gets the effect's property collection.
        /// </summary>
        /// <returns>Returns the <see cref="PropertyCollection"/>.</returns>
        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            return new PropertyCollection(props);
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
                if (IsCancelRequested)
                    return;
                
                var convolution = _convolutionPool.GetConvolution();

                for (int i = startIndex; i < (startIndex + length); i++)
                {
                    convolution.Render(base.DstArgs.Surface, base.SrcArgs.Surface, rois[i]);
                }
                
                _convolutionPool.Release(convolution);
            }
        }
    }
}
