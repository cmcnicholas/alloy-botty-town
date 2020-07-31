using System;
using System.Linq;
using Assets.Server.Drawing;
using Assets.Server.Game;
using Assets.Server.Projection;
using GeoAPI.Geometries;
using GeoJSON.Net.Geometry;
using UnityEngine;

namespace Assets.Server.Mapper
{
    /// <summary>
    /// implementation to create game objects for linestring items
    /// </summary>
    public class LineStringToGameObjectMapper : AssetToGameObjectMapperBase
    {
        private Material _roadMaterial;
        private Camera _camera;

        public LineStringToGameObjectMapper(GameObject stage, StageCoordProjection stageCoordProjector, Material roadMaterial, Camera camera) : base(stage, stageCoordProjector)
        {
            _roadMaterial = roadMaterial;
            _camera = camera;
        }

        public override GameObject CreateGameObjectForAsset(AssetModel asset)
        {
            // get the geometry and expect it to be a linestring
            var itemLineString = asset.Geometry as LineString;
            if (itemLineString == null)
            {
                throw new Exception("Expected item model geometry to be of type LineString");
            }

            // check we have enough coordinates
            int coordinateCount = itemLineString.Coordinates.Count;
            if (coordinateCount <= 1)
            {
                return null;
            }

            // get the coordinates in the game world of the line string, we use 2d vector as all our lines are flat
            // alloy api never gives us 3d coords
            var lineCoordinates = new Coordinate[coordinateCount];
            for (int i = 0; i < coordinateCount; i++)
            {
                // calculate the world coordinates (metres) for the point in the linestring
                var coordinate = itemLineString.Coordinates[i];
                var coordinateMetres = WebMercatorProjection.LatLonToMeters(coordinate.Latitude, coordinate.Longitude);

                // project the metres to the stage
                var itemStageCoords = StageCoordProjector.MetresToStageCoordinate(coordinateMetres);

                // outside of map?
                if (itemStageCoords == null)
                {
                    return null;
                }

                lineCoordinates[i] = new Coordinate(itemStageCoords[0], itemStageCoords[1]);
            }

            // using net topology suite to buffer the linestring by game units (create a poly from the line)
            var gameCoordLineString = new NetTopologySuite.Geometries.LineString(lineCoordinates);
            var bufferedPolygon = gameCoordLineString.Buffer(5.0, GeoAPI.Operations.Buffer.BufferStyle.CapSquare);

            // calculate the 3d vertices, these are lifted by a set value off the game floor plane
            var vertices = bufferedPolygon.Coordinates.
                // geojson polys are always closed, unity doesn't want this so drop last point
                Take(bufferedPolygon.Coordinates.Length - 1).
                Select(c => new Vector3((float)c.X, 0.01f /* off the floor */, (float)c.Y /* 2d y becomes 3d z */)).
                ToArray();

            // update coordinate count, we're using the polygon now
            coordinateCount = vertices.Length;

            // calculate the texture coordinates for the mesh (I don't really care, everything is flat)
            var uvs = new Vector2[coordinateCount];
            for (int i = 0; i < coordinateCount; i++)
            {
                uvs[i] = (i % 2) == 0 ? new Vector2(0.0f, 0.0f) : new Vector2(1.0f, 1.0f);
            }

            // calculate triangles for the mesh (caveat: stolen from some horrendous unity post)
            var triangulator = new Triangulator(Array.ConvertAll<Vector3, Vector2>(vertices, v => new Vector2(v.x, v.z)));

            // get the per design colour or the default
            var colour = ApplicationGlobals.MeshColourMapping.ContainsKey(asset.DesignCode) ?
                ApplicationGlobals.MeshColourMapping[asset.DesignCode] :
                ApplicationGlobals.MeshColourDefault;

            // Generate a color for each vertex
            var colors = Enumerable.Range(0, coordinateCount)
                .Select(i => colour)
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
            var assetGameObject = new GameObject("Asset");

            // make the filter including the mesh we made
            var filter = assetGameObject.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            // the way we render the poly
            var renderer = assetGameObject.AddComponent<MeshRenderer>();
            renderer.material = _roadMaterial;
            renderer.material.color = colour;

            // use the above mesh for the mesh collider so we can interact with it e.g. look at
            var collider = assetGameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;
            collider.enabled = true;
            
            // wrap the drawn game object "Asset" in an empty container, we will attach the asset controller
            // to this just like the item prefabs
            var go = new GameObject();

            // add the asset to the container
            assetGameObject.transform.parent = go.transform;

            // add the asset controller
            var assetController = go.AddComponent<AssetController>();
            assetController.Asset = assetGameObject;
            assetController.IsLineString = true;
            assetController.LineStringCoordinates = vertices;
            assetController.ItemId = asset.ItemId;

            // finally add the new object to the stage
            go.transform.parent = Stage.transform;

            return go;
        }
    }
}
