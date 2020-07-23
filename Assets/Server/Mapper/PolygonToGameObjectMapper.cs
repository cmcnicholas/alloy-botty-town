using Assets.Server.Drawing;
using Assets.Server.Models;
using Assets.Server.Projection;
using GeoJSON.Net.Geometry;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Server.Mapper
{
    /// <summary>
    /// implementation to create game objects for linestring items
    /// </summary>
    public class PolygonToGameObjectMapper : ItemToGameObjectMapperBase
    {
        private Material _groundMaterial;

        public PolygonToGameObjectMapper(GameObject stage, StageCoordProjection stageCoordProjector, Material groundMaterial) : base(stage, stageCoordProjector)
        {
            _groundMaterial = groundMaterial;
        }

        public override GameObject CreateGameObjectForItem(ItemModel item)
        {
            // get the geometry and expect it to be a polygon
            var itemPolygon = item.Geometry as Polygon;
            if (itemPolygon == null)
            {
                throw new Exception("Expected item model geometry to be of type Polygon");
            }

            // if no polygons then return
            if (itemPolygon.Coordinates.Count == 0)
            {
                return null;
            }

            // we only draw the outer, we don' care about holes (yet?)
            var outerPolygon = itemPolygon.Coordinates[0];

            // check we have enough coordinates
            int coordinateCount = outerPolygon.Coordinates.Count;
            if (coordinateCount <= 4)
            {
                return null;
            }

            // get the vertices of the polygon in the game world
            var vertices = new Vector3[coordinateCount];
            for (int i = 0; i < coordinateCount; i++)
            {
                // calculate the world coordinates (metres) for the point in the linestring from the polygon
                var coordinate = outerPolygon.Coordinates[i];
                var coordinateMetres = WebMercatorProjection.LatLonToMeters(coordinate.Latitude, coordinate.Longitude);

                // project the metres to the stage
                var itemStageCoords = StageCoordProjector.MetresToStageCoordinate(coordinateMetres);

                // outside of map?
                if (itemStageCoords == null)
                {
                    return null;
                }

                vertices[i] = new Vector3(itemStageCoords[0], 0.01f /* off the floor */, itemStageCoords[1]);
            }

            // calculate the texture coordinates for the mesh (I don't really care, everything is flat)
            var uvs = new Vector2[coordinateCount];
            for (int i = 0; i < coordinateCount; i++)
            {
                uvs[i] = (i % 2) == 0 ? new Vector2(0.0f, 0.0f) : new Vector2(1.0f, 1.0f);
            }

            // calculate triangles for the mesh (caveat: stolen from some horrendous unity post)
            var triangulator = new Triangulator(System.Array.ConvertAll<Vector3, Vector2>(vertices, v => new Vector2(v.x, v.z)));

            // Generate a color for each vertex
            var colors = Enumerable.Range(0, coordinateCount)
                .Select(i => UnityEngine.Random.ColorHSV())
                .ToArray();

            // create a mesh from the points
            var mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangulator.Triangulate();
            mesh.colors = colors;

            // make our mesh better for performance
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.Optimize();

            // make a new game object, we will draw programatically
            var go = new GameObject();
            go.transform.parent = Stage.transform;

            // make the filter including the mesh we made
            var filter = go.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            // the way we render the poly
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.material = _groundMaterial;

            // use the above mesh for the mesh collider so we can interact with it e.g. look at
            var collider = go.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;

            return go;
        }
    }
}
