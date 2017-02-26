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
        public abstract void Add(ColorBgra bgra, int m, int n);

        /// <summary>
        /// Resets the accumulator.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Gets the acumulated color.
        /// </summary>
        public abstract ColorBgra Color { get; }

        /// <summary>
        /// The convolution kernel definition range.
        /// </summary>
        public abstract Rectangle DefinitionRange { get; }

        /// <summary>
        /// Applies the convolution to the picture.
        /// </summary>
        /// <param name="destination">The destination picture.</param>
        /// <param name="source">The source picture.</param>
        /// <param name="rectangle">The rectangle defining the region to process.</param>
        public void Render(Surface destination, Surface source, Rectangle rectangle)
        {
            for (int y0 = rectangle.Top; y0 < rectangle.Bottom; y0++)
            {
                int top = Math.Max(-y0, this.DefinitionRange.Left);
                int bottom = Math.Min((source.Height - 1 - y0), this.DefinitionRange.Right - 1);

                for (int x0 = rectangle.Left; x0 < rectangle.Right; x0++)
                {
                    this.Clear();

                    int left = Math.Max(-x0, this.DefinitionRange.Left);
                    int right = Math.Min((source.Width - 1 - x0), this.DefinitionRange.Right - 1);

                    for (int x = left; x <= right; x++)
                    {
                        for (int y = top; y <= bottom; y++)
                        {
                            this.Add(source[x0 + x, y0 + y], x, y);
                        }
                    }

                    destination[x0, y0] = this.Color;
                }
            }
        }
    }
}
