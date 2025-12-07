using System.Collections.Generic;
using System.Numerics;

namespace SeaLegs
{
    public static class TriangleCutter
    {
        // Cut a triangle based on vertex heights above water (inverse submergence)
        // Returns 0-2 submerged triangles
        public static void Cut(Vector3 v0, float h0, Vector3 v1, float h1, Vector3 v2, float h2,
            List<HullTriangle> output)
        {
            var submergedCount = 0;
            if (h0 < 0) submergedCount += 1;
            if (h1 < 0) submergedCount += 1;
            if (h2 < 0) submergedCount += 1;

            // How many vertices are submerged?
            switch (submergedCount)
            {
                case 0:
                    // No vertices of triangle submerged, nothing to do
                    return;
                case 3:
                    // All vertices are submerged, no slicing needed
                    output.Add(new HullTriangle(v0, v1, v2));
                    return;
                case 1:
                    CutOneSubmerged(v0, h0, v1, h1, v2, h2, output);
                    return;
                case 2:
                    CutTwoSubmerged(v0, h0, v1, h1, v2, h2, output);
                    return;
            }
        }

        private static void CutOneSubmerged(Vector3 v0, float h0, Vector3 v1, float h1, Vector3 v2, float h2,
            List<HullTriangle> output)
        {
            Vector3 below; // submerged
            Vector3 aboveLower; // above surface but below higher
            Vector3 aboveHigher; // highest vertex
            float belowHeight;
            float aboveLowerHeight;
            float aboveHigherHeight;

            // Identify which vertex is below, the others are above
            if (h0 < 0)
            {
                below = v0;
                belowHeight = h0;
                if (h1 > h2)
                {
                    aboveHigher = v1;
                    aboveHigherHeight = h1;
                    aboveLower = v2;
                    aboveLowerHeight = h2;
                }
                else
                {
                    aboveHigher = v2;
                    aboveHigherHeight = h2;
                    aboveLower = v1;
                    aboveLowerHeight = h1;
                }
            }
            else if (h1 < 0)
            {
                below = v1;
                belowHeight = h1;
                if (h0 > h2)
                {
                    aboveHigher = v0;
                    aboveHigherHeight = h0;
                    aboveLower = v2;
                    aboveLowerHeight = h2;
                }
                else
                {
                    aboveHigher = v2;
                    aboveHigherHeight = h2;
                    aboveLower = v0;
                    aboveLowerHeight = h0;
                }
            }
            else
            {
                below = v2;
                belowHeight = h2;
                if (h0 > h1)
                {
                    aboveHigher = v0;
                    aboveHigherHeight = h0;
                    aboveLower = v1;
                    aboveLowerHeight = h1;
                }
                else
                {
                    aboveHigher = v1;
                    aboveHigherHeight = h1;
                    aboveLower = v0;
                    aboveLowerHeight = h0;
                }
            }

            // Find intersection with water via interpolation
            // t = -h2 / (h1 - h2)
            var tHigher = -belowHeight / (aboveHigherHeight - belowHeight);
            var tLower = -belowHeight / (aboveLowerHeight - belowHeight);

            // crossingPoint = lerp(vertex2, vertex1, t)
            var intersectionHigher = Vector3.Lerp(below, aboveHigher, tHigher);
            var intersectionLower = Vector3.Lerp(below, aboveLower, tLower);

            output.Add(new HullTriangle(below, intersectionLower, intersectionHigher));
        }

        private static void CutTwoSubmerged(
            Vector3 v0, float h0,
            Vector3 v1, float h1,
            Vector3 v2, float h2,
            List<HullTriangle> output)
        {
            Vector3 belowLower; // submerged
            Vector3 belowHigher; // submerged
            Vector3 above; // above
            float belowLowerHeight;
            float belowHeigherHeight;
            float aboveHeight;

            if (h0 >= 0)
            {
                above = v0;
                aboveHeight = h0;
                if (h1 < h2)
                {
                    belowLower = v1;
                    belowLowerHeight = h1;
                    belowHigher = v2;
                    belowHeigherHeight = h2;
                }
                else
                {
                    belowLower = v2;
                    belowLowerHeight = h2;
                    belowHigher = v1;
                    belowHeigherHeight = h1;
                }
            }
            else if (h1 >= 0)
            {
                above = v1;
                aboveHeight = h1;
                if (h0 < h2)
                {
                    belowLower = v0;
                    belowLowerHeight = h0;
                    belowHigher = v2;
                    belowHeigherHeight = h2;
                }
                else
                {
                    belowLower = v2;
                    belowLowerHeight = h2;
                    belowHigher = v0;
                    belowHeigherHeight = h0;
                }
            }
            else
            {
                above = v2;
                aboveHeight = h2;
                if (h0 < h1)
                {
                    belowLower = v0;
                    belowLowerHeight = h0;
                    belowHigher = v1;
                    belowHeigherHeight = h1;
                }
                else
                {
                    belowLower = v1;
                    belowLowerHeight = h1;
                    belowHigher = v0;
                    belowHeigherHeight = h0;
                }
            }

            // Find intersection with water via interpolation
            // t = -h2 / (h1 - h2)
            var tLower = -belowLowerHeight / (aboveHeight - belowLowerHeight);
            var tHigher = -belowHeigherHeight / (aboveHeight - belowHeigherHeight);

            // crossingPoint = lerp(vertex2, vertex1, t)
            var intersectLower = Vector3.Lerp(belowLower, above, tLower);
            var intsersectHigher = Vector3.Lerp(belowHigher, above, tHigher);
            
            output.Add(new HullTriangle(belowLower, belowHigher, intsersectHigher));
            output.Add(new HullTriangle(belowLower, intsersectHigher, intersectLower));
        }
    }
}