namespace Al.Pdn.Colorization.Manual
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
    /// Represents an effect colorizing the picture with a choosen color.
    /// </summary>
    public class ManualColorFilter : PropertyBasedEffect
    {
        private YCbCrColor _selectedColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualColorFilter"/> class.
        /// </summary>
        public ManualColorFilter()
            : base(StaticName, StaticImage, StaticSubMenuName, EffectFlags.Configurable)
        {
        }

        private static Image StaticImage
        {
            get { return Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Al.Pdn.Colorization.Manual.ColorPencil.png")); }
        }

        private static string StaticName
        {
            get { return "Manual color"; }
        }

        private static string StaticSubMenuName
        {
            get { return "Color"; }
        }

        /// <summary>
        /// Gets the effect's property collection.
        /// </summary>
        /// <returns>Returns the <see cref="PropertyCollection"/>.</returns>
        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> properties = new List<Property>();

            int defaultColor = 0;
            defaultColor |= this.EnvironmentParameters.PrimaryColor.R << 16;
            defaultColor |= this.EnvironmentParameters.PrimaryColor.G << 8;
            defaultColor |= this.EnvironmentParameters.PrimaryColor.B << 0;

            properties.Add(new Int32Property("Color", defaultColor, 0, 16777215));

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

            info.SetPropertyControlType("Color", PropertyControlType.ColorWheel);

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
            int color = parameters.GetProperty<Int32Property>("Color").Value;

            _selectedColor = YCbCrColor.FromRgb(ColorBgra.FromUInt32((UInt32)color));
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
                        YCbCrColor color = YCbCrColor.FromRgb(src[x, y]);

                        color.Cb = _selectedColor.Cb;
                        color.Cr = _selectedColor.Cr;

                        dst[x, y] = color.ToRgb();
                    }
                }
            }
        }
    }
}
