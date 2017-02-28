namespace Al.Pdn.Colorization.Inpainting
{
    /// <summary>
    /// Represents a blend.
    /// </summary>
    public struct Blend
    {
        /// <summary>
        /// The chrominance of the blend.
        /// </summary>
        public Chrominance Chrominance;

        /// <summary>
        /// The distance of the blend.
        /// </summary>
        public int Distance;

        /// <summary>
        /// Initializes a new instance of a <see cref="Blend"/>.
        /// </summary>
        /// <param name="chrominance">The chrominance of the blend.</param>
        /// <param name="distance">The distance of the blend.</param>
        public Blend(Chrominance chrominance, int distance)
        {
            Chrominance = chrominance;
            Distance = distance;
        }

        /// <summary>
        /// Determines whether two <see cref="Blend"/> instances are equal.
        /// </summary>
        /// <param name="obj">The instance to compare.</param>
        /// <returns>true if the specified <see cref="Blend"/> is equal to the current <see cref="Blend"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Blend))
                return false;

            return Equals((Blend)obj);
        }

        /// <summary>
        /// Determines whether two <see cref="Blend"/> instances are equal.
        /// </summary>
        /// <param name="other">The instance to compare.</param>
        /// <returns>true if the specified <see cref="Blend"/> is equal to the current <see cref="Blend"/>; otherwise, false.</returns>
        public bool Equals(Blend other)
        {
            if (Distance != other.Distance)
                return false;

            return Chrominance == other.Chrominance;
        }

        /// <summary>
        /// Determines whether two blends are equal.
        /// </summary>
        /// <param name="blend1">The first blend.</param>
        /// <param name="blend2">The second blend.</param>
        /// <returns>true if the specified blends are equal; otherwise, false.</returns>
        public static bool operator ==(Blend blend1, Blend blend2)
        {
            return blend1.Equals(blend2);
        }

        /// <summary>
        /// Determines whether two blends are different.
        /// </summary>
        /// <param name="blend1">The first blend.</param>
        /// <param name="blend2">The second blend.</param>
        /// <returns>true if the specified blends are not equal; otherwise, false.</returns>
        public static bool operator !=(Blend blend1, Blend blend2)
        {
            return !blend1.Equals(blend2);
        }

        /// <summary>
        /// Gets a hash code for the instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Chrominance.GetHashCode() ^ Distance.GetHashCode();
        }

        /// <summary>
        /// Gets a string representing the blend.
        /// </summary>
        /// <returns>A string representing the blend.</returns>
        public override string ToString()
        {
            return "Chrominance = {" + Chrominance.ToString() + "} ; Distance = " + Distance.ToString();
        }
    }
}
