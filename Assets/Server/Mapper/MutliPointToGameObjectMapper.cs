using Assets.Server.Game;
using Assets.Server.Projection;
using GeoJSON.Net.Geometry;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Server.Mapper
{
    /// <summary>
    /// implementation to create game objects for point items
    /// </summary>
    public class MultiPointToGameObjectMapper : AssetToGameObjectMapperBase
    {
        private PrefabManager _prefabManager;

        public MultiPointToGameObjectMapper(GameObject stage, StageCoordProjection stageCoordProjector, PrefabManager prefabManager) : base(stage, stageCoordProjector)
        {
            _prefabManager = prefabManager;
        }

        public override GameObject CreateGameObjectForAsset(AssetModel asset)
        {
            // get the geometry and expect it to be a point
            var itemMultiPoint = asset.Geometry as MultiPoint;
            if (itemMultiPoint == null)
            {
                throw new Exception("Expected item model geometry to be of type MultiPoint");
            }

            var assetGameObjects = new List<GameObject>();
            var masterGameObject = new GameObject();

            foreach (var itemPoint in itemMultiPoint.Coordinates)
            {
                // make the game object
                var go = PointToGameObjectMapper.CreateGameObjectForPoint(_prefabManager, StageCoordProjector, asset, itemPoint, out GameObject assetGameObject);
                go.name = "MultiAsset";

                assetGameObjects.Add(assetGameObject);

                // finally add the multi asset to the master
                go.transform.parent = masterGameObject.transform;
            }

            // setup the asset controller
            var assetController = masterGameObject.AddComponent<AssetController>();
            assetController.Outlines = assetGameObjects;
            assetController.Points = assetGameObjects;
            assetController.ItemId = asset.ItemId;

            // add the master to the stage
            masterGameObject.transform.parent = Stage.transform;

            return masterGameObject;
        }
    }
}
