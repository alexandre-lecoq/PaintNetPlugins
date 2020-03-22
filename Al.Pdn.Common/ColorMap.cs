namespace Al.Pdn.Common
{
    using System;

    public class ColorMap
    {
        public byte[] Red { get; }
        public byte[] Green { get; }
        public byte[] Blue { get; }

        public ColorMap(byte[] red, byte[] green, byte[] blue)
        {
            Red = red ?? throw new ArgumentNullException(nameof(red));
            Green = green ?? throw new ArgumentNullException(nameof(green));
            Blue = blue ?? throw new ArgumentNullException(nameof(blue));
        }
    }
}
