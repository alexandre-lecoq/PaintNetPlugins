namespace Al.Pdn.EdgeDetection.Gabor
{
    using Common;
    using PaintDotNet;
    using PaintDotNet.Effects;
    using PaintDotNet.IndirectUI;
    using PaintDotNet.PropertySystem;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Reflection;

    /// <summary>
    /// Represents an effect detecting edges in the picture using a Gabor filter.
    /// </summary>
    public class GaborFilter : PropertyBasedEffect
    {
        private GaborConvolutionPool _convolutionPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="GaborFilter"/> class.
        /// </summary>
        public GaborFilter()
            : base(StaticName, StaticImage, StaticSubMenuName, EffectFlags.Configurable)
        {
        }
        
        private static Image StaticImage
        {
            get { return Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Al.Pdn.EdgeDetection.Gabor.GaborIcon.png")); }
        }

        private static string StaticName
        {
            get { return "Gabor filter"; }
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
            List<Property> properties = new List<Property>();

            properties.Add(new BooleanProperty("Binary", true));
            properties.Add(new Int32Property("Wavelength", 8, 2, 100));
            properties.Add(new DoubleProperty("Orientation", 0, -180, 180));
            properties.Add(new Int32Property("RenderQuality", 3, 1, 5));

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

            info.SetPropertyControlValue("Binary", ControlInfoPropertyNames.DisplayName, "Input");
            info.SetPropertyControlValue("Binary", ControlInfoPropertyNames.Description, "Binary");

            info.SetPropertyControlValue("Wavelength", ControlInfoPropertyNames.DisplayName, "Wavelength");

            info.SetPropertyControlValue("Orientation", ControlInfoPropertyNames.DisplayName, "Angle");
            info.SetPropertyControlType("Orientation", PropertyControlType.AngleChooser);

            info.SetPropertyControlValue("RenderQuality", ControlInfoPropertyNames.DisplayName, "Render quality");

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
            double orientation = ((double)parameters.GetProperty<DoubleProperty>("Orientation").Value * Math.PI) / 180.0;

            _convolutionPool = new GaborConvolutionPool((double)parameters.GetProperty<Int32Property>("Wavelength").Value, orientation, parameters.GetProperty<Int32Property>("RenderQuality").Value + 1, parameters.GetProperty<BooleanProperty>("Binary").Value);
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
