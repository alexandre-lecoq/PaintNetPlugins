namespace Al.Pdn.Colorization.Inpainting
{
    /// <summary>
    /// Represents a chrominance.
    /// </summary>
    public struct Chrominance
    {
        /// <summary>
        /// The Cb component of the chrominance.
        /// </summary>
        public byte Cb;

        /// <summary>
        /// The Cr component of the chrominance.
        /// </summary>
        public byte Cr;

        /// <summary>
        /// Initializes a new instance of a <see cref="Chrominance"/>.
        /// </summary>
        /// <param name="cb">The Cb component of the chrominance.</param>
        /// <param name="cr">The Cr component of the chrominance.</param>
        public Chrominance(byte cb, byte cr)
        {
            Cb = cb;
            Cr = cr;
        }

        /// <summary>
        /// Determines whether two chrominances are close to each others.
        /// </summary>
        /// <param name="other">The instance to compare.</param>
        /// <returns>true if the specified <see cref="Chrominance"/> is close to the current <see cref="Chrominance"/>; otherwise, false.</returns>
        /// <remarks>
        /// The method considers the maximum squared euclidean distance to be close is 150.
        /// </remarks>
        public bool IsClose(Chrominance other)
        {
            return IsClose(other, 150);
        }

        /// <summary>
        /// Determines whether two chrominances are close to each others.
        /// </summary>
        /// <param name="other">The instance to compare.</param>
        /// <param name="maxSquaredDistance">The maximum squared euclidian distance to consider the two chrominances as being close.</param>
        /// <returns></returns>
        public bool IsClose(Chrominance other, int maxSquaredDistance)
        {
            int cbDistance = this.Cb - other.Cb;
            int crDsitance = this.Cr - other.Cr;
            int distance = cbDistance * cbDistance + crDsitance * crDsitance;

            if (distance <= maxSquaredDistance)
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether two <see cref="Chrominance"/> instances are equal.
        /// </summary>
        /// <param name="obj">The instance to compare.</param>
        /// <returns>true if the specified <see cref="Chrominance"/> is equal to the current <see cref="Chrominance"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Chrominance))
                return false;

            return Equals((Chrominance)obj);
        }

        /// <summary>
        /// Determines whether two <see cref="Chrominance"/> instances are equal.
        /// </summary>
        /// <param name="other">The instance to compare.</param>
        /// <returns>true if the specified <see cref="Chrominance"/> is equal to the current <see cref="Chrominance"/>; otherwise, false.</returns>
        public bool Equals(Chrominance other)
        {
            if (Cb != other.Cb)
                return false;

            return Cr == other.Cr;
        }

        /// <summary>
        /// Determines whether two Chrominance are equal.
        /// </summary>
        /// <param name="chrominance1">The first Chrominance.</param>
        /// <param name="chrominance2">The second Chrominance.</param>
        /// <returns>true if the specified Chrominances are equal; otherwise, false.</returns>
        public static bool operator ==(Chrominance chrominance1, Chrominance chrominance2)
        {
            return chrominance1.Equals(chrominance2);
        }

        /// <summary>
        /// Determines whether two Chrominance are different.
        /// </summary>
        /// <param name="chrominance1">The first Chrominance.</param>
        /// <param name="chrominance2">The second Chrominance.</param>
        /// <returns>true if the specified Chrominances are not equal; otherwise, false.</returns>
        public static bool operator !=(Chrominance chrominance1, Chrominance chrominance2)
        {
            return !chrominance1.Equals(chrominance2);
        }

        /// <summary>
        /// Gets a hash code for the instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Cb.GetHashCode() ^ Cr.GetHashCode();
        }

        /// <summary>
        /// Gets a string representing the Chrominance.
        /// </summary>
        /// <returns>A string representing the Chrominance.</returns>
        public override string ToString()
        {
            return "Cb = " + Cb.ToString() + " ; Cr = " + Cr.ToString();
        }
    }
}
