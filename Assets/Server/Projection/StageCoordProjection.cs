namespace Assets.Server.Projection
{
    public class StageCoordProjection
    {
        private float _stageSize;
        private float _halfStageSize;
        private float[] _centroidMetres;

        /// <summary>
        /// creates a new stage coord projection instance
        /// </summary>
        /// <param name="stageSize">the stage size (length of a side in metres)</param>
        /// <param name="centroidMetres">the world coordinate centroid of the stage (metres)</param>
        public StageCoordProjection(float stageSize, float[] centroidMetres)
        {
            _stageSize = stageSize;
            _halfStageSize = stageSize / 2.0f;
            _centroidMetres = centroidMetres;
        }

        /// <summary>
        /// takes world coordinates (metres) and converts to stage coordinates
        /// </summary>
        public float[] MetresToStageCoordinate(float[] metresCoordinate)
        {
            float itemXDiff = metresCoordinate[0] - _centroidMetres[0];
            float itemYDiff = metresCoordinate[1] - _centroidMetres[1];

            // check we're on the stage, if not then return null
            if (itemXDiff < -_halfStageSize || itemYDiff < -_halfStageSize || itemXDiff > _halfStageSize || itemYDiff > _halfStageSize)
            {
                return null;
            }

            return new float[] { itemXDiff, itemYDiff };
        }
    }
}
