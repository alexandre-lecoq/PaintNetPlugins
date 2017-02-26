namespace Al.Pdn.Common
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a synchronized pool of <see cref="SobelConvolution"/> instances.
    /// </summary>
    public class SobelConvolutionPool
    {
        private LinkedList<SobelConvolution> _availableConvolutionList;

        /// <summary>
        /// Initializes a new instance of the <see cref="SobelConvolutionPool"/> class.
        /// </summary>
        public SobelConvolutionPool()
        {
            _availableConvolutionList = new LinkedList<SobelConvolution>();
        }

        /// <summary>
        /// Gets an instance of <see cref="SobelConvolution"/>.
        /// </summary>
        /// <returns>The instance of <see cref="SobelConvolution"/>.</returns>
        public SobelConvolution GetConvolution()
        {
            lock (_availableConvolutionList)
            {
                if (_availableConvolutionList.Count == 0)
                {
                    return new SobelConvolution();
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
        /// Releases an instance of <see cref="SobelConvolution"/>.
        /// </summary>
        /// <param name="convolution">The instance of <see cref="SobelConvolution"/> to release.</param>
        public void Release(SobelConvolution convolution)
        {
            lock (_availableConvolutionList)
            {
                _availableConvolutionList.AddFirst(convolution);
            }
        }
    }
}
