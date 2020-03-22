namespace Al.Pdn.Common
{
    using PaintDotNet;
    using System;
    using System.Runtime.CompilerServices;

    public static class RgbColorExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnsafeRgbColorF ToUnsafeRgbColorF(this ColorBgra color)
        {
            return new UnsafeRgbColorF(
                color.R / (double) byte.MaxValue,
                color.G / (double) byte.MaxValue,
                color.B / (double) byte.MaxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ConvertToByte(this double d)
        {
            return Int32Util.ClampToByte((int)Math.Round(d * byte.MaxValue));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnsafeRgbColorF Subtract(this double left, UnsafeRgbColorF right)
        {
            var red = left - right.Red;
            var green = left - right.Green;
            var blue = left - right.Blue;
            var result = new UnsafeRgbColorF(red, green, blue);

            return result;
        }
    }
}
