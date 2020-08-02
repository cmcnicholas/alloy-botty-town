using Assets.Server.Game;
using Assets.Server.Projection;
using GeoJSON.Net.Geometry;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Server.Mapper
{
    /// <summary>
    /// implementation to create game objects for geometry collection items
    /// </summary>
    public class GeometryCollectionToGameObjectMapper : AssetToGameObjectMapperBase
    {
        private PrefabManager _prefabManager;
        private Material _groundMaterial;
        private Material _roadMaterial;

        public GeometryCollectionToGameObjectMapper(GameObject stage, StageCoordProjection stageCoordProjector, 
            PrefabManager prefabManager, Material groundMaterial, Material roadMaterial) : base(stage, stageCoordProjector)
        {
            _prefabManager = prefabManager;
            _groundMaterial = groundMaterial;
            _roadMaterial = roadMaterial;
        }

        public override GameObject CreateGameObjectForAsset(AssetModel asset)
        {
            // get the geometry and expect it to be geom collection...
            var itemGeometryCollection = asset.Geometry as GeometryCollection;
            if (itemGeometryCollection == null)
            {
                throw new Exception("Expected item model geometry to be of type GeometryCollection");
            }
            
            var outlineGameObjects = new List<GameObject>();
            var pointGameObjects = new List<GameObject>();
            var polyGameObjects = new List<GameObject>();
            var allLineCoordinates = new List<List<Vector3>>();
            var masterGameObject = new GameObject();

            foreach (var subGeometry in itemGeometryCollection.Geometries)
            {
                if (subGeometry is Point itemPoint)
                {
                    // make the game object
                    var go = PointToGameObjectMapper.CreateGameObjectForPoint(_prefabManager, StageCoordProjector, asset, itemPoint, out GameObject assetGameObject);
                    outlineGameObjects.Add(assetGameObject);
                    pointGameObjects.Add(assetGameObject);

                    // finally add the asset to the master
                    go.transform.parent = masterGameObject.transform;
                }
                else if (subGeometry is MultiPoint itemMultiPoint)
                {
                    foreach (var subItemPoint in itemMultiPoint.Coordinates)
                    {
                        // make the game object
                        var go = PointToGameObjectMapper.CreateGameObjectForPoint(_prefabManager, StageCoordProjector, asset, subItemPoint, out GameObject assetGameObject);
                        go.name = "MultiAsset";
                        outlineGameObjects.Add(assetGameObject);
                        pointGameObjects.Add(assetGameObject);

                        // finally add the multi asset to the master
                        go.transform.parent = masterGameObject.transform;
                    }
                }
                else if (subGeometry is LineString itemLineString)
                {
                    // make the game object
                    var go = LineStringToGameObjectMapper.CreateGameObjectForLineString(StageCoordProjector, _roadMaterial, asset, itemLineString, 
                        out GameObject assetGameObject, out List<Vector3> lineCoordinates);
                    outlineGameObjects.Add(assetGameObject);
                    allLineCoordinates.Add(lineCoordinates);

                    // finally add the multi asset to the master
                    go.transform.parent = masterGameObject.transform;
                }
                else if (subGeometry is MultiLineString itemMultiLineString)
                {
                    foreach (var subItemLineString in itemMultiLineString.Coordinates)
                    {
                        // make the game object
                        var go = LineStringToGameObjectMapper.CreateGameObjectForLineString(StageCoordProjector, _roadMaterial, asset, subItemLineString, 
                            out GameObject assetGameObject, out List<Vector3> lineCoordinates);
                        go.name = "MultiAsset";

                        outlineGameObjects.Add(assetGameObject);
                        allLineCoordinates.Add(lineCoordinates);

                        // finally add the multi asset to the master
                        go.transform.parent = masterGameObject.transform;
                    }
                }
                else if (subGeometry is Polygon itemPolygon)
                {
                    // make the game object
                    var go = PolygonToGameObjectMapper.CreateGameObjectForPolygon(_prefabManager, StageCoordProjector, _groundMaterial, asset, itemPolygon,
                        out GameObject assetGameObject, out GameObject assetModelGameObject);
                    outlineGameObjects.Add(assetGameObject);
                    outlineGameObjects.Add(assetModelGameObject);
                    polyGameObjects.Add(assetModelGameObject);

                    // finally add the multi asset to the master
                    go.transform.parent = masterGameObject.transform;
                }
                else if (subGeometry is MultiPolygon itemMultiPolygon)
                {
                    foreach (var subItemPolygon in itemMultiPolygon.Coordinates)
                    {
                        // make the game object
                        var go = PolygonToGameObjectMapper.CreateGameObjectForPolygon(_prefabManager, StageCoordProjector, _groundMaterial, asset, subItemPolygon,
                            out GameObject assetGameObject, out GameObject assetModelGameObject);
                        go.name = "MultiAsset";

                        // want all models for outlining
                        outlineGameObjects.Add(assetGameObject);
                        outlineGameObjects.Add(assetModelGameObject);

                        // only want the prefab for to base effects around (e.g. defects, inspections)
                        polyGameObjects.Add(assetModelGameObject);

                        // finally add the multi asset to the master
                        go.transform.parent = masterGameObject.transform;
                    }
                }
                else
                {
                    Debug.Log("Geometry collection has unhandled child geometry type: " + subGeometry.Type);
                }
            }
            
            // setup the asset controller
            var assetController = masterGameObject.AddComponent<AssetController>();
            assetController.Outlines = outlineGameObjects;
            assetController.Points = pointGameObjects.Count == 0 ? null : pointGameObjects;
            assetController.Polys = polyGameObjects.Count == 0 ? null : polyGameObjects;
            assetController.LineCoordinates = allLineCoordinates.Count == 0 ? null : allLineCoordinates;
            assetController.ItemId = asset.ItemId;

            // add the master to the stage
            masterGameObject.transform.parent = Stage.transform;

            return masterGameObject;
        }
    }
}
