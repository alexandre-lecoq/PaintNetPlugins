namespace Al.Pdn.Colorization.Inpainting
{
    using Al.Pdn.Common;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Represents a class to compute a blend map.
    /// </summary>
    public static class Colorizer
    {
        /// <summary>
        /// Computes the blend map from a given picture.
        /// </summary>
        /// <param name="picture">The picture to compute the blend map from.</param>
        /// <param name="cancelPollFunction">A cancellation function.</param>
        /// <returns>The blend map.</returns>
        public static List<Blend>[][] ComputeBlendMap(YCbCrColor[][] picture, Func<bool> cancelPollFunction)
        {
            bool[][] grayMap = ToGrayMap(picture);

            PriorityQueue linkList = new PriorityQueue();
            AddColorsToList(linkList, grayMap, picture);

            List<Blend>[][]  blendMap = CreateBlendMap(picture.Length, picture[0].Length);

            if (FillBlendMap(blendMap, linkList, grayMap, picture, cancelPollFunction) == false)
                return null;

            return blendMap;
        }

        private static bool[][] ToGrayMap(YCbCrColor[][] picture)
        {
            bool[][] gray = new bool[picture.Length][];

            for (int x = 0; x < picture.Length; x++)
                gray[x] = new bool[picture[x].Length];

            for (int x = 0; x < picture.Length; x++)
                for (int y = 0; y < picture[x].Length; y++)
                    gray[x][y] = ((picture[x][y].Cb == 128) && (picture[x][y].Cr == 128)) ? true : false;

            return gray;
        }

        private static void AddColorsToList(PriorityQueue linkList, bool[][] grayMap, YCbCrColor[][] picture)
        {
            for (int x = 0; x < picture.Length; x++)
                for (int y = 0; y < picture[x].Length; y++)
                    if (grayMap[x][y] == false)
                        linkList.Add(0, new Vertex(new Point(x, y), new Blend(new Chrominance(picture[x][y].Cb, picture[x][y].Cr), 0)));
        }

        private static List<Blend>[][] CreateBlendMap(int width, int height)
        {
            List<Blend>[][] blendMatrix = new List<Blend>[width][];

            for (int x = 0; x < width; x++)
                blendMatrix[x] = new List<Blend>[height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    blendMatrix[x][y] = new List<Blend>();

            return blendMatrix;
        }

        private static bool FillBlendMap(List<Blend>[][] blendMap, PriorityQueue linkList, bool[][] grayMap, YCbCrColor[][] picture, Func<bool> cancelPollFunction)
        {
            while (linkList.IsEmpty == false)
            {
                if (cancelPollFunction())
                    return false;

                Vertex smallestLink = linkList.PopMin();

                List<Blend> blendList = blendMap[smallestLink.Point.X][smallestLink.Point.Y];

                if (blendList.Count < 3) // Relaxation 1 : no need for more than X blends
                    if (IsCloseChrominanceInBlendList(blendList, smallestLink) == false) // Relaxation 2 : no need to included 2 twice the same chrominance (or a close one)
                    {
                        blendList.Add(smallestLink.Blend);

                        // TODO: Relaxation 3 :  no need to include distance > 3 * distance(closestBlend)
                        AddNeighbours(picture, grayMap, linkList, smallestLink);
                    }
            }

            return true;
        }

        private static bool IsCloseChrominanceInBlendList(List<Blend> blendList, Vertex smallestLink)
        {
            foreach (Blend blend in blendList)
            {
                if (blend.Chrominance.IsClose(smallestLink.Blend.Chrominance))
                    return true;
            }

            return false;
        }

        private static void AddNeighbours(YCbCrColor[][] picture, bool[][] grayMap, PriorityQueue linkList, Vertex smallestLink)
        {
            int x = smallestLink.Point.X;
            int y = smallestLink.Point.Y;

            if ((x - 1) >= 0 && grayMap[x - 1][y] == true)
            {
                int distance = smallestLink.Blend.Distance + Math.Abs((int)picture[x][y].Y - (int)picture[x - 1][y].Y);
                linkList.Add(distance, new Vertex(new Point(x - 1, y), new Blend(smallestLink.Blend.Chrominance, distance)));
            }

            if ((x + 1) < picture.Length && grayMap[x + 1][y] == true)
            {
                int distance = smallestLink.Blend.Distance + Math.Abs((int)picture[x][y].Y - (int)picture[x + 1][y].Y);
                linkList.Add(distance, new Vertex(new Point(x + 1, y), new Blend(smallestLink.Blend.Chrominance, distance)));
            }

            if ((y - 1) >= 0 && grayMap[x][y - 1] == true)
            {
                int distance = smallestLink.Blend.Distance + Math.Abs((int)picture[x][y].Y - (int)picture[x][y - 1].Y);
                linkList.Add(distance, new Vertex(new Point(x, y - 1), new Blend(smallestLink.Blend.Chrominance, distance)));
            }

            if ((y + 1) < picture[x].Length && grayMap[x][y + 1] == true)
            {
                int distance = smallestLink.Blend.Distance + Math.Abs((int)picture[x][y].Y - (int)picture[x][y + 1].Y);
                linkList.Add(distance, new Vertex(new Point(x, y + 1), new Blend(smallestLink.Blend.Chrominance, distance)));
            }
        }
    }
}
