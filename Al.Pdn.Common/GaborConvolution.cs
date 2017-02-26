namespace Al.Pdn.Common
{
    using PaintDotNet;
    using System.Drawing;

    /// <summary>
    /// Represents a gabor convolution.
    /// </summary>
    public class GaborConvolution : Convolution
    {
        private Complex _complexR;
        private Complex _complexG;
        private Complex _complexB;
        private GaborFilterKernelWithEnvelope _kernel;
        private bool _binary;
        private ColorBgra _resultColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="GaborConvolution"/> class.
        /// </summary>
        public GaborConvolution(double wavelength, double orientation, int renderQuality, bool binary)
        {
            _complexR = Complex.Zero;
            _complexG = Complex.Zero;
            _complexB = Complex.Zero;
            _kernel = new GaborFilterKernelWithEnvelope(wavelength, orientation, renderQuality);
            _binary = binary;

            _resultColor = new ColorBgra();
            _resultColor.A = 0xff;
        }

        /// <inheritdoc />
        public override void Clear()
        {
            _complexR = Complex.Zero;
            _complexG = Complex.Zero;
            _complexB = Complex.Zero;
        }

        /// <inheritdoc />
        public override void Add(ColorBgra bgra, int m, int n)
        {
            if (_binary)
            {
                if (bgra.R == 0xFF)
                    _complexR += _kernel[m, n];
            }
            else
            {
                var kernelValue = _kernel[m, n];
                _complexR += bgra.R * kernelValue;
                _complexG += bgra.G * kernelValue;
                _complexB += bgra.B * kernelValue;
            }
        }

        /// <inheritdoc />
        public override ColorBgra Color
        {
            get
            {
                var factor = _kernel.Factor;

                if (_binary)
                {
                    byte scaledMagnitude = (byte)(_complexR.Magnitude * factor).Clamp(0,255);
                    
                    _resultColor.R = scaledMagnitude;
                    _resultColor.G = scaledMagnitude;
                    _resultColor.B = scaledMagnitude;
                }
                else
                {
                    _resultColor.R = (byte)(_complexR.Magnitude * factor).Clamp(0, 255);
                    _resultColor.G = (byte)(_complexG.Magnitude * factor).Clamp(0, 255);
                    _resultColor.B = (byte)(_complexB.Magnitude * factor).Clamp(0, 255);
                }

                return _resultColor;
            }
        }

        /// <inheritdoc />
        public override Rectangle DefinitionRange
        {
            get { return _kernel.DefinitionRange; }
        }
    }
}
