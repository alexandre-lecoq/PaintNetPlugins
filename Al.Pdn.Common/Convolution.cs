namespace Al.Pdn.Common
{
    using PaintDotNet;
    using System;
    using System.Drawing;

    /// <summary>
    /// Represents an abstract convolution.
    /// </summary>
    public abstract class Convolution
    {
        /// <summary>
        /// Accumulates color multiplied by kernel value at (m,n).
        /// </summary>
        /// <param name="bgra">The color.</param>
        /// <param name="m">The m coordinate in the convolution kernel.</param>
        /// <param name="n">The n coordinate in the convolution kernel.</param>
        protected abstract void Add(ColorBgra bgra, int m, int n);

        /// <summary>
        /// Resets the accumulator.
        /// </summary>
        protected abstract void Clear();

        /// <summary>
        /// Gets the accumulated color.
        /// </summary>
        protected abstract ColorBgra Color { get; }

        /// <summary>
        /// The convolution kernel definition range.
        /// </summary>
        protected abstract Rectangle DefinitionRange { get; }

        /// <summary>
        /// Applies the convolution to the picture.
        /// </summary>
        /// <param name="destination">The destination picture.</param>
        /// <param name="source">The source picture.</param>
        /// <param name="rectangle">The rectangle defining the region to process.</param>
        public void Render(Surface destination, Surface source, Rectangle rectangle)
        {
            for (var y0 = rectangle.Top; y0 < rectangle.Bottom; y0++)
            {
                var top = Math.Max(-y0, DefinitionRange.Left);
                var bottom = Math.Min((source.Height - 1 - y0), DefinitionRange.Right - 1);

                for (var x0 = rectangle.Left; x0 < rectangle.Right; x0++)
                {
                    Clear();

                    var left = Math.Max(-x0, DefinitionRange.Left);
                    var right = Math.Min((source.Width - 1 - x0), DefinitionRange.Right - 1);

                    for (var x = left; x <= right; x++)
                    {
                        var x0x = x0 + x;

                        for (var y = top; y <= bottom; y++)
                        {
                            var y0y = y0 + y;
                            var color = source[x0x, y0y];
                            Add(color, x, y);
                        }
                    }

                    destination[x0, y0] = Color;
                }
            }
        }
    }
}
