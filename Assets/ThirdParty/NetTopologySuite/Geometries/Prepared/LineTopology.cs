using GeoAPI.Geometries;
using NetTopologySuite.Noding;

namespace NetTopologySuite.Geometries.Prepared
{
    public class LineTopology
    {
        private readonly IGeometryFactory _geomFact;
        private readonly NodedSegmentString _segStr;

        public LineTopology(ICoordinate[] pts, IGeometryFactory geomFact)
        {
            _segStr = new NodedSegmentString((Coordinate[])pts, this);
            _geomFact = geomFact;
        }

        public void AddIntersection(ICoordinate intPt, int segmentIndex)
        {
            _segStr.AddIntersection((Coordinate)intPt, segmentIndex);
        }

        public IGeometry Result
        {
            get
            {
                ICoordinate[] resultPts = new Coordinate[0];
                return _geomFact.CreateLineString((Coordinate[])resultPts);
            }
        }

    }
}