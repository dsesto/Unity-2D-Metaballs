/// Author: David Sesto (GitHub: @dsesto)
/// Digging Dinosaurs Games (Twitter @digging_dinos)

using UnityEngine;

public class MeshGrid {

    /*
     * Each of the samples (vertices) into which the grid is divided. The number of samples that our experiment
     * will have is determined by the resolution defined in the MetaballManager.
     */
    public class Sample {
        public int col;
        public int row;

        public Vector2 position;
        public float sampleValue;

        public Sample(int row, int col, float x, float y) {
            this.row = row;
            this.col = col;
            this.position = new Vector2(x, y);
        }

        /// Update the sample value of this GridSample by considering the contribution of each of the
        /// metaballs present in the experiment, like:
        ///    f(x, y) = ∑(r_i^2/((x−xi)^2 + (y−yi)^2))
        /// Additionally, if a rectangle was added to the simulation, compute its contribution too.
        public void UpdateSampleValue(Metaball[] metaballs) {
            float newSample = 0;

            // Add metaballs contribution
            foreach (Metaball metaball in metaballs) {
                float sampleToBallXDist = position.x - metaball.position.x;
                float sampleToBallYDist = position.y - metaball.position.y;
                newSample += Mathf.Pow(metaball.radius, 2) / (Mathf.Pow(sampleToBallXDist, 2) + Mathf.Pow(sampleToBallYDist, 2));
            }

            // Add rectangle contribution
            if (MetaballManager.instance.includeRectangle) {
                newSample += GetRectangleSampleContribution();
            }

            this.sampleValue = newSample;
        }

        /// Computes the contribution of the rectangle to this sample, as the ratio between the distance
        /// from the rectangle to its border, and the distance between the rectangle and the sample point
        private float GetRectangleSampleContribution() {
            Vector2 rectangleScale = MetaballManager.instance.rectangle.transform.localScale;
            Vector2 rectanglePosition = MetaballManager.instance.rectangle.transform.position;
            Vector2 pointToRectangleDist = position - rectanglePosition;

            // The angle at which we need to switch between using sine and cosine
            float thresholdAngle = Mathf.Atan(rectangleScale.y / rectangleScale.x);
            float angle = Mathf.Atan(Mathf.Abs(pointToRectangleDist.y) / Mathf.Abs(pointToRectangleDist.x));

            float distanceToPoint = Mathf.Sqrt(Mathf.Pow(pointToRectangleDist.x, 2) + Mathf.Pow(pointToRectangleDist.y, 2));
            float distanceToBorder;
            if (angle <= thresholdAngle) {
                distanceToBorder = Mathf.Abs(rectangleScale.x / 2 / Mathf.Cos(angle));
            } else {
                distanceToBorder = Mathf.Abs(rectangleScale.y / 2 / Mathf.Sin(angle));
            }

            // Points inside the rectangle will always be painted (as per "Cell.GetCellConfiguration()")
            // because their contribution is greater than 1.
            // The scalingFactor can be used to control how far will the metaballs start merging with the
            // rectangle. Smaller factors increase the "merging distance" because the contribution of the
            // rectangle is not as penalized as with bigger factors.
            float scalingFactor = 5;
            return Mathf.Pow(distanceToBorder / distanceToPoint, scalingFactor);
        }
    }

    /*
     * Each of the cells that form the samples of the grid. They are quads (squares) of 4 samples (vertices)
     * which, following the documentation naming, are structured like:
     *    A ·———· B
     *      |   |
     *    C ·———· D
     */
    public class Cell {
        public Sample sampleA;
        public Sample sampleB;
        public Sample sampleC;
        public Sample sampleD;

        // Each cell is built starting on the lower-left (C) sample
        public Cell(Sample startingSample) {
            int nextRow = Mathf.Min(MetaballManager.instance.numRows - 1, startingSample.row + 1);
            int nextCol = Mathf.Min(MetaballManager.instance.numCols - 1, startingSample.col + 1);

            sampleA = MetaballManager.instance.gridSamples[nextRow, startingSample.col];
            sampleB = MetaballManager.instance.gridSamples[nextRow, nextCol];
            sampleC = startingSample;
            sampleD = MetaballManager.instance.gridSamples[startingSample.row, nextCol];
        }

        /// Compute which of the 16 [0-15] configurations corresponds to this cell, on each frame
        public int GetCellConfiguration() {
            int configuration = 0;

            float cellConfigurationThreshold = 1f;
            if (sampleC.sampleValue >= cellConfigurationThreshold)
                configuration |= 1;
            if (sampleD.sampleValue >= cellConfigurationThreshold)
                configuration |= 2;
            if (sampleB.sampleValue >= cellConfigurationThreshold)
                configuration |= 4;
            if (sampleA.sampleValue >= cellConfigurationThreshold)
                configuration |= 8;

            return configuration;
        }

        /// Interpolate points located over one of the edges A-B, D-B, C-D, C-A. Vertex order matters
        /// because starting points (Lerp a=-1f) are on the left (or bottom) and ending points (Lerp b=1f)
        /// are on the right (or top).
        /// Points that are located over a cell vertex do not need to be interpolated.
        public Vector3 ApplyLinearInterpolation(Vector3 point) {
            // Point is not on an edge
            if (point.x * point.y != 0)
                return point;

            // A-B
            if (point.x == 0 && point.y == 1)
                point.x = Mathf.Lerp(-1f, 1f, (1f - sampleA.sampleValue) / (sampleB.sampleValue - sampleA.sampleValue));
            // D-B
            else if (point.x == 1 && point.y == 0)
                point.y = Mathf.Lerp(-1f, 1f, (1f - sampleD.sampleValue) / (sampleB.sampleValue - sampleD.sampleValue));
            // C-D
            else if (point.x == 0 && point.y == -1)
                point.x = Mathf.Lerp(-1f, 1f, (1f - sampleC.sampleValue) / (sampleD.sampleValue - sampleC.sampleValue));
            // C-A
            else if (point.x == -1 && point.y == 0)
                point.y = Mathf.Lerp(-1f, 1f, (1f - sampleC.sampleValue) / (sampleA.sampleValue - sampleC.sampleValue));

            return point;
        }
    }
}
