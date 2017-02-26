namespace Al.Pdn.Common
{
    using System.Drawing;

    /// <summary>
    /// Represents an abstract convolution kernel.
    /// </summary>
    /// <typeparam name="T">The numeric type of the kernel.</typeparam>
    public abstract class ConvolutionKernel<T>
    {
        /// <summary>
        /// The definition range of the kernel.
        /// </summary>
        public abstract Rectangle DefinitionRange { get; }

        /// <summary>
        /// Gets the value of the kernel at position (x, y).
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>The value of the kernel at position (x,y).</returns>
        public abstract T this[int x, int y] { get; }

        /// <summary>
        /// Gets the multiplicative factor of the kernel.
        /// </summary>
        public virtual double Factor
        {
            get { return 1.0; }
        }

        /// <summary>
        /// Gets the additive biais of the kernel.
        /// </summary>
        public virtual double Biais
        {
            get { return 0.0; }
        }

        /// <summary>
        /// Creates a square matric to store the kernel values.
        /// </summary>
        /// <param name="sideSize">The side size of the matrix.</param>
        /// <returns>The created matrix.</returns>
        protected static T[][] CreateSquareMatrix(int sideSize)
        {
            T[][] matrix = new T[sideSize][];

            for (int i = 0; i < sideSize; i++)
            {
                matrix[i] = new T[sideSize];
            }

            return matrix;
        }
    }
}
