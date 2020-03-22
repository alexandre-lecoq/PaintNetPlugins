using System.Runtime.CompilerServices;

namespace Al.Pdn.Common
{
    using System.Drawing;

    /// <summary>
    /// Represents the Gabor convolution kernel in a finite definition range.
    /// </summary>
    public class GaborFilterKernelWithEnvelope : ConvolutionKernel<Complex>
    {
        private readonly Complex[][] _kernelMatrix;
        private Rectangle _definitionRange;
        private readonly int _kernelArrayXOffset;
        private readonly int _kernelArrayYOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="GaborFilterKernelWithEnvelope"/> class.
        /// </summary>
        /// <param name="wavelength">The wavelength of the Gabor kernel.</param>
        /// <param name="orientation">The angle of the Gabor kernel.</param>
        /// <param name="envelopeWavelengthFactor">The side size of the definition range of the kernel.</param>
        public GaborFilterKernelWithEnvelope(double wavelength, double orientation, int envelopeWavelengthFactor)
        {
            var fullKernel = new GaborFilterKernel(wavelength, orientation);
            var sideSize = (int)(wavelength * envelopeWavelengthFactor);

            // this is a quick fix dealing with even sized kernels.
            if (sideSize % 2 == 0)
                sideSize += 1;

            var left = -(sideSize / 2);
            
            _definitionRange = new Rectangle(left, left, sideSize, sideSize);
            _kernelArrayXOffset = _definitionRange.Right - 1;
            _kernelArrayYOffset = _definitionRange.Bottom - 1;
            _kernelMatrix = CreateSquareMatrix(sideSize);
            Fill2DArray(fullKernel);
            Factor = GetFactor();
        }

        private void Fill2DArray(GaborFilterKernel fullKernel)
        {
            for (var i = 0; i < _kernelMatrix.Length; i++)
            {
                for (var j = 0; j < _kernelMatrix.Length; j++)
                {
                    _kernelMatrix[i][j] = fullKernel[i - _definitionRange.Right, j - _definitionRange.Bottom];
                }
            }
        }

        private double GetFactor()
        {
            var factor = Complex.Zero;

            for (var i = 0; i < _kernelMatrix.Length; i++)
            {
                for (var j = 0; j < _kernelMatrix.Length; j++)
                {
                    factor += _kernelMatrix[i][j];
                }
            }

            return 1.0 / factor.Magnitude;
        }

        /// <inheritdoc />
        public override Complex this[int x, int y]
        {
            get
            {
                var realX = x + _kernelArrayXOffset;
                var realY = y + _kernelArrayYOffset;

                return _kernelMatrix[realX][realY];
            }
        }

        /// <inheritdoc />
        public override Rectangle DefinitionRange => _definitionRange;

        /// <inheritdoc />
        public override double Factor { get; }
    }
}
