using GeoJSON.Net.Geometry;
using System;

namespace Assets.Server.Projection
{
    /// <summary>
    /// Conversion routines for Google, TMS, and Microsoft Quadtree tile representations, derived from
    /// http://www.maptiler.org/google-maps-coordinates-tile-bounds-projection/ 
    /// </summary>
    public class WebMercatorProjection
    {
        private const double EarthRadius = 6378137.0;
        private const double OriginShift = 2.0 * Math.PI * EarthRadius / 2.0;

        /// <summary>
        /// Converts given lat/lon in WGS84 Datum to XY in Spherical Mercator EPSG:900913
        /// </summary>
        public static float[] LatLonToMeters(double lat, double lon)
        {
            float x = (float)lon * (float)OriginShift / 180.0f;
            float y = (float)Math.Log(Math.Tan((90.0f + lat) * Math.PI / 360.0f)) / (float)(Math.PI / 180.0f);
            y = y * (float)OriginShift / 180.0f;
            return new float[] { x, y };
        }

        /// <summary>
        /// Converts XY point from (Spherical) Web Mercator EPSG:3785 (unofficially EPSG:900913) to lat/lon in WGS84 Datum
        /// </summary>
        public static Position MetersToLatLon(float x, float y)
        {
            double lon = (x / OriginShift) * 180.0;
            double lat = (y / OriginShift) * 180.0;
            lat = 180.0 / Math.PI * (2.0 * Math.Atan(Math.Exp(y * Math.PI / 180.0)) - Math.PI / 2.0);
            return new Position(lat, lon);
        }
    }
}
