using Assets.Server.Drawing;
using Assets.Server.Game;
using Assets.Server.Projection;
using GeoAPI.Geometries;
using GeoJSON.Net.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Server.Mapper
{
    /// <summary>
    /// implementation to create game objects for linestring items
    /// </summary>
    public class PolygonToGameObjectMapper : AssetToGameObjectMapperBase
    {
        private PrefabManager _prefabManager;
        private Material _groundMaterial;

        public PolygonToGameObjectMapper(GameObject stage, StageCoordProjection stageCoordProjector, PrefabManager prefabManager, 
            Material groundMaterial) : base(stage, stageCoordProjector)
        {
            _prefabManager = prefabManager;
            _groundMaterial = groundMaterial;
        }

        public override GameObject CreateGameObjectForAsset(AssetModel asset)
        {
            // get the geometry and expect it to be a polygon
            var itemPolygon = asset.Geometry as Polygon;
            if (itemPolygon == null)
            {
                throw new Exception("Expected item model geometry to be of type Polygon");
            }

            // make the game object
            var go = CreateGameObjectForPolygon(_prefabManager, StageCoordProjector, _groundMaterial, asset, itemPolygon, 
                out GameObject assetGameObject, out GameObject assetModelGameObject);
            
            // add the asset controller
            var assetController = go.AddComponent<AssetController>();
            assetController.Outlines = new List<GameObject>
            {
                assetGameObject,
                assetModelGameObject,
            };
            assetController.Polys = new List<GameObject> { assetModelGameObject }; // only the model to add defects etc. to
            assetController.ItemId = asset.ItemId;

            // finally add the new object to the stage
            go.transform.parent = Stage.transform;

            return go;
        }

        // used for multi geom too
        public static GameObject CreateGameObjectForPolygon(PrefabManager prefabManager, StageCoordProjection stageCoordProjector,
            Material groundMaterial, AssetModel asset, Polygon itemPolygon, out GameObject assetGameObject, out GameObject assetModelGameObject)
        {
            assetGameObject = null;
            assetModelGameObject = null;

            // if no polygons then return
            if (itemPolygon.Coordinates.Count == 0)
            {
                return null;
            }

            // we only draw the outer, we don' care about holes (yet?)
            var outerPolygon = itemPolygon.Coordinates[0];

            // check we have enough coordinates
            int coordinateCount = outerPolygon.Coordinates.Count;
            if (coordinateCount <= 3)
            {
                return null;
            }

            // if the polygon from geojson is closed (should be to follow spec) unclose it? what unity?
            // we do this by reducing the coordinate count because, tight loading loops and allocating
            // extra resource is for dummies...
            if (outerPolygon.Coordinates.Last().Equals(outerPolygon.Coordinates.First()))
            {
                coordinateCount -= 1;
            }

            // get the vertices of the polygon in the game world
            var vertices = new Vector3[coordinateCount];
            for (int i = 0; i < coordinateCount; i++)
            {
                // calculate the world coordinates (metres) for the point in the linestring from the polygon
                var coordinate = outerPolygon.Coordinates[i];
                var coordinateMetres = WebMercatorProjection.LatLonToMeters(coordinate.Latitude, coordinate.Longitude);

                // project the metres to the stage
                var itemStageCoords = stageCoordProjector.MetresToStageCoordinate(coordinateMetres);

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
            assetGameObject = new GameObject("AssetPoly");

            // make the filter including the mesh we made
            var filter = assetGameObject.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            // the way we render the poly
            var renderer = assetGameObject.AddComponent<MeshRenderer>();
            renderer.material = groundMaterial;
            renderer.material.color = colour;

            // use the above mesh for the mesh collider so we can interact with it e.g. look at
            var collider = assetGameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;

            // wrap the drawn game object "Asset" in an empty container, we will attach the asset controller
            // to this just like the item prefabs
            var go = new GameObject();

            // add the asset to the container
            assetGameObject.transform.parent = go.transform;

            // now we start making the model to put on top of our poly

            // generate a polygon in world coordinates
            var polygonCoords = new Coordinate[coordinateCount + 1];
            for (var i = 0; i < coordinateCount; i++)
            {
                var vertex = vertices[i];
                polygonCoords[i] = new Coordinate(vertex.x, vertex.z);
            }
            polygonCoords[coordinateCount] = new Coordinate(vertices[0].x, vertices[0].z); // close the polygon

            // calculate the centroid
            var polygon = new NetTopologySuite.Geometries.Polygon(new NetTopologySuite.Geometries.LinearRing(polygonCoords));

            // move the asset to the correct position and add to the parent
            assetModelGameObject = prefabManager.CreateForAsset(asset);
            assetModelGameObject.transform.position = new Vector3((float)polygon.Centroid.X, 0.01f, (float)polygon.Centroid.Y);
            assetModelGameObject.transform.parent = go.transform;

            return go;
        }
    }
}
