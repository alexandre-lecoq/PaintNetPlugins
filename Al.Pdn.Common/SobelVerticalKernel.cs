namespace Al.Pdn.Common
{
    using System.Drawing;

    /// <summary>
    /// Represents the Sobel vertical convolution kernel.
    /// </summary>
    public class SobelVerticalKernel : ConvolutionKernel<double>
    {
        private double[][] _kernelMatrix;
        private Rectangle _definitionRange;

        /// <summary>
        /// Initializes a new instance of the <see cref="SobelVerticalKernel"/> class.
        /// </summary>
        public SobelVerticalKernel()
        {
            _definitionRange = new Rectangle(-1, -1, 3, 3);
            _kernelMatrix =  CreateSquareMatrix(3);
            Fill2DArray();
        }

        private void Fill2DArray()
        {
            _kernelMatrix[0][0] = 1;
            _kernelMatrix[0][1] = 0;
            _kernelMatrix[0][2] = -1;
            _kernelMatrix[1][0] = 2;
            _kernelMatrix[1][1] = 0;
            _kernelMatrix[1][2] = -2;
            _kernelMatrix[2][0] = 1;
            _kernelMatrix[2][1] = 0;
            _kernelMatrix[2][2] = -1;
        }

        /// <inheritdoc />
        public override double this[int x, int y]
        {
            get
            {
                int realX = x + _definitionRange.Right - 1;
                int realY = y + _definitionRange.Bottom - 1;

                return _kernelMatrix[realX][realY];
            }
        }

        /// <inheritdoc />
        public override Rectangle DefinitionRange 
        {
            get { return _definitionRange; }
        }
    }
}
