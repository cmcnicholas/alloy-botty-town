using Assets.Server.Game;
using Assets.Server.Projection;
using GeoJSON.Net.Geometry;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Server.Mapper
{
    /// <summary>
    /// implementation to create game objects for polygon items
    /// </summary>
    public class MultiPolygonToGameObjectMapper : AssetToGameObjectMapperBase
    {
        private PrefabManager _prefabManager;
        private Material _groundMaterial;

        public MultiPolygonToGameObjectMapper(GameObject stage, StageCoordProjection stageCoordProjector, PrefabManager prefabManager,
            Material groundMaterial) : base(stage, stageCoordProjector)
        {
            _prefabManager = prefabManager;
            _groundMaterial = groundMaterial;
        }

        public override GameObject CreateGameObjectForAsset(AssetModel asset)
        {
            // get the geometry and expect it to be a point
            var itemMultiPolygon = asset.Geometry as MultiPolygon;
            if (itemMultiPolygon == null)
            {
                throw new Exception("Expected item model geometry to be of type MultiPolygon");
            }

            var outlineGameObjects = new List<GameObject>();
            var modelGameObjects = new List<GameObject>();
            var masterGameObject = new GameObject();

            foreach (var itemPolygon in itemMultiPolygon.Coordinates)
            {
                // make the game object
                var go = PolygonToGameObjectMapper.CreateGameObjectForPolygon(_prefabManager, StageCoordProjector, _groundMaterial, asset, itemPolygon, 
                    out GameObject assetGameObject, out GameObject assetModelGameObject);
                go.name = "MultiAsset";

                // want all models for outlining
                outlineGameObjects.Add(assetGameObject);
                outlineGameObjects.Add(assetModelGameObject);

                // only want the prefab for to base effects around (e.g. defects, inspections)
                modelGameObjects.Add(assetModelGameObject);

                // finally add the multi asset to the master
                go.transform.parent = masterGameObject.transform;
            }

            // setup the asset controller
            var assetController = masterGameObject.AddComponent<AssetController>();
            assetController.Outlines = outlineGameObjects;
            assetController.Polys = modelGameObjects;
            assetController.ItemId = asset.ItemId;

            // add the master to the stage
            masterGameObject.transform.parent = Stage.transform;

            return masterGameObject;
        }
    }
}
