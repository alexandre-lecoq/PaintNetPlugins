namespace Al.Pdn.Common
{
    using PaintDotNet;
    using System;
    using System.Drawing;

    /// <summary>
    /// Represents a sobel convolution.
    /// </summary>
    public class SobelConvolution : Convolution
    {
        private double sumHR;
        private double sumHG;
        private double sumHB;

        private double sumVR;
        private double sumVG;
        private double sumVB;

        private SobelHorizontalKernel _hKernel;
        private SobelVerticalKernel _vKernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SobelConvolution"/> class.
        /// </summary>
        public SobelConvolution()
        {
            _hKernel = new SobelHorizontalKernel();
            _vKernel = new SobelVerticalKernel();
        }

        /// <inheritdoc />
        protected override void Clear()
        {
            sumHR = 0;
            sumHG = 0;
            sumHB = 0;

            sumVR = 0;
            sumVG = 0;
            sumVB = 0;
        }

        /// <inheritdoc />
        protected override void Add(ColorBgra bgra, int m, int n)
        {
            var hKernelValue = _hKernel[m, n];
            sumHR += bgra.R * hKernelValue;
            sumHG += bgra.G * hKernelValue;
            sumHB += bgra.B * hKernelValue;

            var vKernalValue = _vKernel[m, n];
            sumVR += bgra.R * vKernalValue;
            sumVG += bgra.G * vKernalValue;
            sumVB += bgra.B * vKernalValue;
        }

        /// <inheritdoc />
        protected override ColorBgra Color
        {
            get
            {
                ColorBgra resultColor = new ColorBgra();
                resultColor.A = 0xff;

                double normR = Math.Abs(sumHR) + Math.Abs(sumVR);
                double normG = Math.Abs(sumHG) + Math.Abs(sumVG);
                double normB = Math.Abs(sumHB) + Math.Abs(sumVB);

                resultColor.R = (byte)normR.Clamp(0, 255);
                resultColor.G = (byte)normG.Clamp(0, 255);
                resultColor.B = (byte)normB.Clamp(0, 255);

                return resultColor;
            }
        }

        /// <inheritdoc />
        protected override Rectangle DefinitionRange
        {
            get { return _hKernel.DefinitionRange; }
        }
    }
}
