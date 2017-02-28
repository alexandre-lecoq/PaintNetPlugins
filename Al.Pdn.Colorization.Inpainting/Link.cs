namespace Al.Pdn.Colorization.Inpainting
{
    using System.Drawing;

    /// <summary>
    /// Represents a vertex in Dijkstra's algorithm.
    /// </summary>
    public struct Vertex
    {
        /// <summary>
        /// The point coordinate of the vertex.
        /// </summary>
        public Point Point;

        /// <summary>
        /// The Blend of the vertex.
        /// </summary>
        public Blend Blend;

        /// <summary>
        /// Initializes a new instance of a <see cref="Vertex"/>.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="blend"></param>
        public Vertex(Point point, Blend blend)
        {
            Point = point;
            Blend = blend;
        }

        /// <summary>
        /// Determines whether two <see cref="Vertex"/> instances are equal.
        /// </summary>
        /// <param name="obj">The instance to compare.</param>
        /// <returns>true if the specified <see cref="Vertex"/> is equal to the current <see cref="Vertex"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Vertex))
                return false;

            return Equals((Vertex)obj);
        }

        /// <summary>
        /// Determines whether two <see cref="Vertex"/> instances are equal.
        /// </summary>
        /// <param name="other">The instance to compare.</param>
        /// <returns>true if the specified <see cref="Vertex"/> is equal to the current <see cref="Vertex"/>; otherwise, false.</returns>
        public bool Equals(Vertex other)
        {
            if (Point != other.Point)
                return false;

            return Blend == other.Blend;
        }

        /// <summary>
        /// Determines whether two Vertex are equal.
        /// </summary>
        /// <param name="vertex1">The first Vertex.</param>
        /// <param name="vertex2">The second Vertex.</param>
        /// <returns>true if the specified vertices are equal; otherwise, false.</returns>
        public static bool operator ==(Vertex vertex1, Vertex vertex2)
        {
            return vertex1.Equals(vertex2);
        }

        /// <summary>
        /// Determines whether two Vertex are different.
        /// </summary>
        /// <param name="vertex1">The first Vertex.</param>
        /// <param name="vertex2">The second Vertex.</param>
        /// <returns>true if the specified vertices are not equal; otherwise, false.</returns>
        public static bool operator !=(Vertex vertex1, Vertex vertex2)
        {
            return !vertex1.Equals(vertex2);
        }

        /// <summary>
        /// Gets a hash code for the instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Point.GetHashCode() ^ Blend.GetHashCode();
        }

        /// <summary>
        /// Gets a string representing the Vertex.
        /// </summary>
        /// <returns>A string representing the Vertex.</returns>
        public override string ToString()
        {
            return "Point = {" + Point.ToString() + "} ; Blend = {" + Blend.ToString() + "}";
        }
    }
}
