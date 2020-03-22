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
        
        private static Image StaticImage => Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Al.Pdn.EdgeDetection.Gabor.GaborIcon.png"));

        private static string StaticName => "Gabor filter";

        private static string StaticSubMenuName => "Stylize";

        /// <summary>
        /// Gets the effect's property collection.
        /// </summary>
        /// <returns>Returns the <see cref="PropertyCollection"/>.</returns>
        protected override PropertyCollection OnCreatePropertyCollection()
        {
            var properties = new List<Property>
            {
                new BooleanProperty("Binary", true),
                new Int32Property("Wavelength", 8, 2, 100),
                new DoubleProperty("Orientation", 0, -180, 180),
                new Int32Property("RenderQuality", 3, 1, 5)
            };
            
            return new PropertyCollection(properties);
        }

        /// <summary>
        /// Sets the parameter controls.
        /// </summary>
        /// <param name="props">The property collection.</param>
        /// <returns>The control information.</returns>
        protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
        {
            var info = PropertyBasedEffect.CreateDefaultConfigUI(props);

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
            var wavelength = parameters.GetProperty<Int32Property>("Wavelength").Value;
            var orientation = parameters.GetProperty<DoubleProperty>("Orientation").Value * Math.PI / 180.0;
            var renderQuality = parameters.GetProperty<Int32Property>("RenderQuality").Value + 1;
            var binary = parameters.GetProperty<BooleanProperty>("Binary").Value;

            _convolutionPool = new GaborConvolutionPool(wavelength, orientation, renderQuality, binary);
        }

        /// <summary>
        /// Renders part of the picture.
        /// </summary>
        /// <param name="renderRects">The regions to render.</param>
        /// <param name="startIndex">The index of the first region to render.</param>
        /// <param name="length">The number of regions to render.</param>
        protected override void OnRender(Rectangle[] renderRects, int startIndex, int length)
        {
            if (length == 0)
                return;

            if (IsCancelRequested)
                return;

            var convolution = _convolutionPool.GetConvolution();

            for (var i = startIndex; i < (startIndex + length); i++)
            {
                convolution.Render(base.DstArgs.Surface, base.SrcArgs.Surface, renderRects[i]);
            }

            _convolutionPool.Release(convolution);
        }
    }
}
