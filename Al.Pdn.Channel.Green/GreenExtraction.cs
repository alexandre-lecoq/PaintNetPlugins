namespace Al.Pdn.Channel.Green
{
    using PaintDotNet;
    using PaintDotNet.Effects;
    using PaintDotNet.PropertySystem;
    using System.Drawing;

    /// <summary>
    /// Represents an effect extracting the green channel of the picture.
    /// </summary>
    public class GreenExtraction : PropertyBasedEffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GreenExtraction"/> class.
        /// </summary>
        public GreenExtraction()
            : base(StaticName, StaticIcon, StaticSubMenuName, EffectFlags.None)
        {
        }
        
        private static Bitmap StaticIcon
        {
            get { return null; }
        }

        private static string StaticName
        {
            get { return "Extract green"; }
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
            return PropertyCollection.CreateEmpty();
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
                        dst[x, y] = ColorBgra.FromBgra(sourceColor.G, sourceColor.G, sourceColor.G, sourceColor.A);
                    }
                }
            }
        }
    }
}
