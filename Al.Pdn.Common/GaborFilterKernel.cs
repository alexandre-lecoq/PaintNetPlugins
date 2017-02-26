namespace Al.Pdn.Common
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Represents the Gabor convolution kernel in an infinite definition range.
    /// </summary>
    public class GaborFilterKernel : ConvolutionKernel<Complex>
    {
        private Rectangle _definitionRange;

        private double _expMinusSigmaSquareOverTwo;
        private double _kappa;
        private double _kappaSquare;
        private double _kappaSquareOverSigmaSquare;
        private double _sigma;
        private double _sigmaSquare;
        private double _twiceSigmaSquare;
        private double _kappaTimesCosVk;
        private double _kappaTimesSinVk;
        private double _kappaSquareOverTwiceSigmaSquare;

        /// <summary>
        /// Initializes a new instance of the <see cref="GaborFilterKernel"/> class.
        /// </summary>
        /// <param name="wavelength">The wavelength of the Gabor kernel.</param>
        /// <param name="orientation">The angle of the Gabor kernel.</param>
        public GaborFilterKernel(double wavelength, double orientation)
        {
            _definitionRange = new Rectangle(Int32.MinValue, Int32.MinValue, Int32.MaxValue, Int32.MaxValue);

            InitializeGaborKernel(wavelength, orientation);
        }

        private void InitializeGaborKernel(double iota, double Vk)
        {
            // constants
            _sigma = Math.PI;
            _sigmaSquare = _sigma * _sigma;
            _twiceSigmaSquare = 2.0 * _sigmaSquare;
            _expMinusSigmaSquareOverTwo = Math.Exp(-(_sigmaSquare / 2.0));

            // variables
            _kappa = 2 * Math.PI / iota;
            _kappaSquare = _kappa * _kappa;
            _kappaSquareOverSigmaSquare = _kappaSquare / _sigmaSquare;
            _kappaTimesCosVk = _kappa * Math.Cos(Vk);
            _kappaTimesSinVk = _kappa * Math.Sin(Vk);
            _kappaSquareOverTwiceSigmaSquare = _kappaSquare / _twiceSigmaSquare;
        }

        /// <inheritdoc />
        public override Complex this[int x, int y]
        {
            get { return Kernel(x, y); }
        }

        private Complex Kernel(int x, int y)
        {
            double xSquare = x * x;
            double ySquare = y * y;

            double d = (_kappaTimesCosVk * x) + (_kappaTimesSinVk * y);
            double num = _kappaSquareOverSigmaSquare * Math.Exp(-(_kappaSquareOverTwiceSigmaSquare * (xSquare + ySquare)));

            double real = num * (Math.Cos(d) - _expMinusSigmaSquareOverTwo);
            double imaginary = num * Math.Sin(d);

            return new Complex(real, imaginary);
        }

        /// <inheritdoc />
        public override Rectangle DefinitionRange
        {
            get { return _definitionRange; }
        }
    }
}
