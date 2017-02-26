namespace Al.Pdn.Common
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a synchronized pool of <see cref="GaborConvolutionPool"/> instances.
    /// </summary>
    public struct GaborConvolutionPool
    {
        private double _wavelength;
        private double _orientation;
        private int _renderQuality;
        private bool _binary;
        private LinkedList<GaborConvolution> _availableConvolutionList;

        /// <summary>
        /// Initializes a new instance of the <see cref="GaborConvolutionPool"/> class.
        /// </summary>
        public GaborConvolutionPool(double wavelength, double orientation, int renderQuality, bool binary)
        {
            _wavelength = wavelength;
            _orientation = orientation;
            _renderQuality = renderQuality;
            _binary = binary;

            _availableConvolutionList = new LinkedList<GaborConvolution>();
        }

        /// <summary>
        /// Gets an instance of <see cref="GaborConvolution"/>.
        /// </summary>
        /// <returns>The instance of <see cref="GaborConvolution"/>.</returns>
        public GaborConvolution GetConvolution()
        {
            lock (_availableConvolutionList)
            {
                if (_availableConvolutionList.Count == 0)
                {
                    return new GaborConvolution(_wavelength, _orientation, _renderQuality, _binary);
                }
                else
                {
                    // TODO : move lock here. In the worst we'll create more instance than needed. Is it a problem ?
                    var firstConvolution = _availableConvolutionList.First.Value;
                    _availableConvolutionList.RemoveFirst();
                    return firstConvolution;
                }
            }
        }

        /// <summary>
        /// Releases an instance of <see cref="GaborConvolution"/>.
        /// </summary>
        /// <param name="convolution">The instance of <see cref="GaborConvolution"/> to release.</param>
        public void Release(GaborConvolution convolution)
        {
            lock (_availableConvolutionList)
            {
                _availableConvolutionList.AddFirst(convolution);
            }
        }
    }
}
