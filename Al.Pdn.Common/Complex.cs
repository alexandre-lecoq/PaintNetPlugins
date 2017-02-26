namespace Al.Pdn.Common
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Implements a double-precision Complex Number
    /// </summary>
    [Serializable]
    public struct Complex
    {
        /// <summary>
        /// Gets or sets the real part of the complex number.
        /// </summary>
        public double Real;

        /// <summary>
        /// Gets or sets the imaginary part of the complex number.
        /// </summary>
        public double Imaginary;

        /// <summary>
        /// Represents a zero-valued complex number.
        /// </summary>
        public static readonly Complex Zero = new Complex(0, 0);

        /// <summary>
        /// Initializes a new instance of a <see cref="Complex"/>.
        /// </summary>
        /// <param name="real">The real part.</param>
        /// <param name="imaginary">The imaginary part.</param>
        public Complex(double real, double imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }

        /// <summary>
        /// Casts a complex to a double.
        /// </summary>
        /// <param name="complex">The complex number.</param>
        /// <returns>The double.</returns>
        public static implicit operator double(Complex complex)
        {
            return (complex.Real);
        }

        /// <summary>
        /// Casts double to complex.
        /// </summary>
        /// <param name="number">The double.</param>
        /// <returns>The complex number.</returns>
        public static explicit operator Complex(double number)
        {
            return (new Complex(number, 0));
        }

        /// <summary>
        /// Plus.
        /// </summary>
        /// <param name="complex">The complex number.</param>
        /// <returns>The complex number.</returns>
        public static Complex operator +(Complex complex)
        {
            return (complex);
        }

        /// <summary>
        /// Negates a complex number.
        /// </summary>
        /// <param name="complex">The complex number to negate.</param>
        /// <returns>The negated complex number.</returns>
        public static Complex operator -(Complex complex)
        {
            return (new Complex(-complex.Real, -complex.Imaginary));
        }

        /// <summary>
        /// Adds to complex numbers.
        /// </summary>
        /// <param name="leftComplex">The first complex number.</param>
        /// <param name="rightComplex">The second complex number.</param>
        /// <returns>The sum.</returns>
        public static Complex operator +(Complex leftComplex, Complex rightComplex)
        {
            return (new Complex(leftComplex.Real + rightComplex.Real, leftComplex.Imaginary + rightComplex.Imaginary));
        }

        /// <summary>
        /// Adds a complex number and a double.
        /// </summary>
        /// <param name="complex">The complex number.</param>
        /// <param name="number">The double.</param>
        /// <returns>The sum.</returns>
        public static Complex operator +(Complex complex, double number)
        {
            return (new Complex(complex.Real + number, complex.Imaginary));
        }

        /// <summary>
        /// Adds a double and a complex number.
        /// </summary>
        /// <param name="number">The double.</param>
        /// <param name="complex">The complex number.</param>
        /// <returns>The sum.</returns>
        public static Complex operator +(double number, Complex complex)
        {
            return (new Complex(complex.Real + number, complex.Imaginary));
        }

        /// <summary>
        /// Substracts a complex number and a double.
        /// </summary>
        /// <param name="complex">The complex number.</param>
        /// <param name="number">The double.</param>
        /// <returns>The difference.</returns>
        public static Complex operator -(Complex complex, double number)
        {
            return (new Complex(complex.Real - number, complex.Imaginary));
        }

        /// <summary>
        /// Substracts a double and a complex number.
        /// </summary>
        /// <param name="number">The double.</param>
        /// <param name="complex">The complex number.</param>
        /// <returns>The difference.</returns>
        public static Complex operator -(double number, Complex complex)
        {
            return (new Complex(number - complex.Real, -complex.Imaginary));
        }

        /// <summary>
        /// Substracts 2 complex numbers.
        /// </summary>
        /// <param name="leftComplex">The first complex number.</param>
        /// <param name="rightComplex">The second complex number.</param>
        /// <returns>The difference.</returns>
        public static Complex operator -(Complex leftComplex, Complex rightComplex)
        {
            return (new Complex(leftComplex.Real - rightComplex.Real, leftComplex.Imaginary - rightComplex.Imaginary));
        }

        /// <summary>
        /// Multiplies 2 complex numbers.
        /// </summary>
        /// <param name="leftComplex">The first complex number.</param>
        /// <param name="rightComplex">The second complex number.</param>
        /// <returns>The product.</returns>
        public static Complex operator *(Complex leftComplex, Complex rightComplex)
        {
            double real = (leftComplex.Real * rightComplex.Real) - (leftComplex.Imaginary * rightComplex.Imaginary);
            double imaginary = (leftComplex.Real * rightComplex.Imaginary) + (leftComplex.Imaginary * rightComplex.Real);

            return (new Complex(real, imaginary));
        }

        /// <summary>
        /// Multiplies a complex number and a double.
        /// </summary>
        /// <param name="complex">The complex number.</param>
        /// <param name="number">The number.</param>
        /// <returns>The product.</returns>
        public static Complex operator *(Complex complex, double number)
        {
            return (new Complex(complex.Real * number, complex.Imaginary * number));
        }

        /// <summary>
        /// Multiplies a double and a complex number.
        /// </summary>
        /// <param name="number">The double.</param>
        /// <param name="complex">The complex number.</param>
        /// <returns>The product.</returns>
        public static Complex operator *(double number, Complex complex)
        {
            return (new Complex(complex.Real * number, complex.Imaginary * number));
        }

        /// <summary>
        /// Divides 2 complex numbers.
        /// </summary>
        /// <param name="numerator">The first complex number.</param>
        /// <param name="denominator">The second complex number.</param>
        /// <returns>The quotient.</returns>
        public static Complex operator /(Complex numerator, Complex denominator)
        {
            double div = denominator.Real * denominator.Real + denominator.Imaginary * denominator.Imaginary;

            if (div == 0)
                throw new DivideByZeroException("The result is undefined when the denominator is 0.");

            double real = (numerator.Real * denominator.Real + numerator.Imaginary * denominator.Imaginary) / div;
            double imaginary = (numerator.Imaginary * denominator.Real - numerator.Real * denominator.Imaginary) / div;

            return (new Complex(real, imaginary));
        }

        /// <summary>
        /// Determines whether two complex numbers are equal.
        /// </summary>
        /// <param name="leftComplex">The first complex number.</param>
        /// <param name="rightComplex">The second complex number.</param>
        /// <returns>true if the specified Complex numbers are equal; otherwise, false.</returns>
        public static bool operator ==(Complex leftComplex, Complex rightComplex)
        {
            return ((leftComplex.Real == rightComplex.Real) && (leftComplex.Imaginary == rightComplex.Imaginary));
        }

        /// <summary>
        /// Determines whether two complex numbers are different.
        /// </summary>
        /// <param name="leftComplex">The first complex number.</param>
        /// <param name="rightComplex">The second complex number.</param>
        /// <returns>true if the specified Complex numbers are not equal; otherwise, false.</returns>
        public static bool operator !=(Complex leftComplex, Complex rightComplex)
        {
            return ((leftComplex.Real != rightComplex.Real) || (leftComplex.Imaginary != rightComplex.Imaginary));
        }

        /// <summary>
        /// Gets a hash code for the instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return (Real.GetHashCode() ^ Imaginary.GetHashCode());
        }

        /// <summary>
        /// Determines whether two complex numbers instances are equal.
        /// </summary>
        /// <param name="obj">The instance to compare.</param>
        /// <returns>true if the specified complex number is equal to the current complex number; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return ((obj is Complex) ? (this == (Complex)obj) : false);
        }

        /// <summary>
        /// Gets a string representing the Complex Number in natural form.
        /// </summary>
        /// <returns>A string representing the complex number.</returns>
        public override string ToString()
        {
            return (String.Format(CultureInfo.CurrentCulture, "{0} + {1}i", Real.ToString(), Imaginary.ToString()));
        }

        /// <summary>
        /// Gets the magnitude of the complex number.
        /// </summary>
        /// <remarks>This is the L2-norm. aka Euclidean norm.</remarks>
        public double Magnitude
        {
            get { return (Math.Sqrt(Real * Real + Imaginary * Imaginary)); }
        }

        /// <summary>
        /// Gets the squared magnitude of the complex number.
        /// </summary>
        /// <remarks>This is the squared L2-norm. aka squared Euclidean norm.</remarks>
		public double SquaredMagnitude
        {
            get { return (Real * Real + Imaginary * Imaginary); }
        }

        /// <summary>
        /// Gets the Manhattan norm of the complex number.
        /// </summary>
        /// <remarks>This is the L1-norm. aka Taxicab norm or Manhattan norm.</remarks>
        public double ManhattanNorm
        {
            get { return Math.Abs(Real) + Math.Abs(Imaginary); }
        }

        /// <summary>
        /// Gets the Maximum norm of the complex number.
        /// </summary>
        /// <remarks>This is the Linf-norm. aka Maximum norm.</remarks>
        public double MaximumNorm
        {
            get { return Math.Max(Math.Abs(Real), Math.Abs(Imaginary)); }
        }
    }
}
