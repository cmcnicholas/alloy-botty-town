
using System;
using System.Linq;
using System.Collections.Generic;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.Densify;
using NetTopologySuite.Triangulate;
using UnityEngine;

namespace Skeleton
{
    public static class Finder
    {
        public static IList<Vector3> getSkeleton(IList<Vector3> poly)
        {
            IGeometryFactory gfactory = GeometryFactory.Fixed;
            WKTReader greader = new WKTReader(gfactory);
            //Create a new polygon instance
            //"POLYGON((0 0, 0 10, 20 10, 20 20, 25 20, 25 10, 40 10, 40 0, 0 0))"
            var polyEnlarged = poly.Select(x => new Vector3(x.x * 100f, x.y * 100f, x.z * 100f)).ToList();
            polyEnlarged.Add(polyEnlarged.First());
            //string rd = "POLYGON((";
            //foreach (Vector3 v in polyEnlarged)
            //{
            //    rd += v.x.ToString() + " " + v.y.ToString() + " " + v.z.ToString() + ",";
            //}
            ////rd += polyEnlarged[0].x.ToString() + " " + polyEnlarged[0].y.ToString() + " " + polyEnlarged[0].z.ToString();
            //rd = rd.Substring(0, rd.Length - 1);
            //rd += "))";
            //IGeometry polygon = greader.Read(rd);
            //var gf = new GeometryFactory();
            //IGeometry polygon = gf.CreatePolygon(polyEnlarged.Select(x => {
            //    var c = new GeoAPI.Geometries.Coordinate((double)Mathf.Floor(x.x), (double)Mathf.Floor(x.z), 0.0);
            //    gf.CreatePoint(c);
            //    return c;
            //}).ToArray());
            //Densify polygon and store vertices in point array

            //int dd = 0;
            var cols = polyEnlarged.Select(x => new GeoAPI.Geometries.Coordinate((double)Mathf.Floor(x.x), (double)Mathf.Floor(x.y)) /*(double)Mathf.Floor(x.z),*/ ).ToArray();

            var polygon = gfactory.CreatePolygon(cols);

            var pointsL = new List<IGeometry>();
            IGeometry densification = Densifier.Densify(polygon, 5);
            IGeometry[] points = new IGeometry[densification.NumPoints - 1];// new IGeometry[polyEnlarged.Count - 1];

            for (int i = 0; i <= points.GetUpperBound(0); i++)
            {
                points[i] = gfactory.CreatePoint(densification.Coordinates[i]);
            }

            //foreach (GeoAPI.Geometries.Coordinate c in cols)
            //{
            //    pointsL.Add(gfactory.CreatePoint(c));
            //}

            //points = pointsL.ToArray();
            //Create a new point collection
            IGeometryCollection pointsCollection = gfactory.CreateGeometryCollection(points);
            //Build Voronoi diagram from point collection
            VoronoiDiagramBuilder voronoiBuilder = new VoronoiDiagramBuilder();
            voronoiBuilder.SetSites(pointsCollection.Coordinates);
            IGeometryCollection diagram = voronoiBuilder.GetDiagram(gfactory);
            //Intersect Voronoi diagram with target polygon to get crust
            IGeometry crust = diagram.Intersection(polygon);
            //Create an empty geometry collection for the skeleton 
            IList<IGeometry> skeleton = new List<IGeometry>();
            //Populate skeleton from edges in crust
            for (int i = 0; i < crust.NumGeometries; i++)
            {
                IGeometry g = crust.GetGeometryN(i);
                for (int j = 1; j < g.Coordinates.Length; j++)
                {
                    //For each polygon in crust get each edge 
                    IGeometry edge = gfactory.CreateLineString(new Coordinate[] { g.Coordinates[j - 1], g.Coordinates[j] });
                    if (!edge.Touches(polygon.Boundary) && edge.Within(polygon))
                    {
                        skeleton.Add(edge); //If this edge does not touch polygon perimeter add to skeleton
                    }
                }
            }
            return skeleton.Select(x => new Vector3((float)x.Coordinate.X/100f, (float)x.Coordinate.Y / 100f, 0.0f)).ToList();
        }
    }
}