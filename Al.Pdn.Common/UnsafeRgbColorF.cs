namespace Al.Pdn.Common
{
    using System;
    using System.Runtime.CompilerServices;
    using PaintDotNet;

    public struct UnsafeRgbColorF
    {
        public double Red { get; }

        public double Green { get; }

        public double Blue { get; }

        public bool IsZero => Red == 0.0 && Green == 0.0 && Blue == 0.0;

        public static readonly UnsafeRgbColorF One = new UnsafeRgbColorF(1, 1, 1);
        public static readonly UnsafeRgbColorF Zero = new UnsafeRgbColorF(0, 0, 0);
        
        public UnsafeRgbColorF(double red, double green, double blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public override string ToString()
        {
            return $"(R={Red}, G={Green}, B={Blue})";
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ColorBgra ToColorBgra()
        {
            return ColorBgra.FromBgr(
                this.Red.ConvertToByte(),
                this.Green.ConvertToByte(),
                this.Blue.ConvertToByte());
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnsafeRgbColorF Apply(Func<double, double> function)
        {
            var red = function(Red);
            var green = function(Green);
            var blue = function(Blue);
            var result = new UnsafeRgbColorF(red, green, blue);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnsafeRgbColorF Add(UnsafeRgbColorF right)
        {
            var red = Red + right.Red;
            var green = Green + right.Green;
            var blue = Blue + right.Blue;
            var result = new UnsafeRgbColorF(red, green, blue);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  UnsafeRgbColorF Subtract(UnsafeRgbColorF right)
        {
            var red = Red - right.Red;
            var green = Green - right.Green;
            var blue = Blue - right.Blue;
            var result = new UnsafeRgbColorF(red, green, blue);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnsafeRgbColorF MultiplyBy(UnsafeRgbColorF right)
        {
            var red = Red * right.Red;
            var green = Green * right.Green;
            var blue = Blue * right.Blue;
            var result = new UnsafeRgbColorF(red, green, blue);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnsafeRgbColorF MultiplyBy(double factor)
        {
            var red = Red / factor;
            var green = Green / factor;
            var blue = Blue / factor;
            var result = new UnsafeRgbColorF(red, green, blue);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnsafeRgbColorF DivideBy(int denominator)
        {
            var red = Red / denominator;
            var green = Green / denominator;
            var blue = Blue / denominator;
            var result = new UnsafeRgbColorF(red, green, blue);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  UnsafeRgbColorF DivideBy(UnsafeRgbColorF denominator)
        {
            var red = Red / denominator.Red;
            var green = Green / denominator.Green;
            var blue = Blue / denominator.Blue;
            var result = new UnsafeRgbColorF(red, green, blue);

            return result;
        }
    }
}
