namespace Al.Pdn.Colorization.Inpainting
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a priority queue.
    /// </summary>
    public class PriorityQueue
    {
        private LinkedList<Vertex>[] _priorities;
        private int _minPriority = 0;
        private int _minPriorityIndex = 0;
        private int _count = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue"/> class.
        /// </summary>
        public PriorityQueue()
        {
            _priorities = new LinkedList<Vertex>[256];

            for (int i = 0; i < 256; i++)
                _priorities[i] = new LinkedList<Vertex>();
        }

        /// <summary>
        /// Adds a Vertex to the priority queue.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="vertex">The Vertex to add.</param>
        public void Add(int priority, Vertex vertex)
        {
            int i = priority - _minPriority + _minPriorityIndex;

            if (i > 255)
                i -= 256;

            _priorities[i].AddLast(vertex);
            _count++;
        }

        /// <summary>
        /// Gets and remove the Vertex with the minimum priority.
        /// </summary>
        /// <returns>The Vertex with the minimum priority.</returns>
        public Vertex PopMin()
        {
            int i = 0;

            while (_priorities[_minPriorityIndex].Count == 0)
            {
                _minPriorityIndex += 1;

                if (_minPriorityIndex == 256)
                    _minPriorityIndex = 0;

                ++i;

                if (i == 256)
                    throw new InvalidOperationException("Priority queue is empty.");
            }

            _minPriority += i;

            Vertex vertex = _priorities[_minPriorityIndex].First.Value;
            _priorities[_minPriorityIndex].RemoveFirst();
            _count--;

            return vertex;
        }

        /// <summary>
        /// Gets a boolean indicating whether the priority queue is empty or not.
        /// </summary>
        public bool IsEmpty
        {
            get { return _count == 0; }
        }
        
        /// <summary>
        /// Gets a string representing the blend.
        /// </summary>
        /// <returns>A string representing the blend.</returns>
        public override string ToString()
        {
            return "Count = " + _count.ToString() + " ; MinPriority = " + _minPriority.ToString();
        }
    }
}
