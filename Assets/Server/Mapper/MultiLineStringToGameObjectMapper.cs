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
    public class MultiLineStringToGameObjectMapper : AssetToGameObjectMapperBase
    {
        private Material _roadMaterial;

        public MultiLineStringToGameObjectMapper(GameObject stage, StageCoordProjection stageCoordProjector, Material roadMaterial) : base(stage, stageCoordProjector)
        {
            _roadMaterial = roadMaterial;
        }

        public override GameObject CreateGameObjectForAsset(AssetModel asset)
        {
            // get the geometry and expect it to be a multi linestring
            var itemMultiLineString = asset.Geometry as MultiLineString;
            if (itemMultiLineString == null)
            {
                throw new Exception("Expected item model geometry to be of type MultiLineString");
            }

            var assetGameObjects = new List<GameObject>();
            var allLineCoordinates = new List<List<Vector3>>();
            var masterGameObject = new GameObject();

            foreach (var itemLineString in itemMultiLineString.Coordinates)
            {
                // make the game object
                var go = LineStringToGameObjectMapper.CreateGameObjectForLineString(StageCoordProjector, _roadMaterial, asset, itemLineString, out GameObject assetGameObject, out List<Vector3> lineCoordinates);
                go.name = "MultiAsset";

                assetGameObjects.Add(assetGameObject);
                allLineCoordinates.Add(lineCoordinates);

                // finally add the multi asset to the master
                go.transform.parent = masterGameObject.transform;
            }

            // setup the asset controller
            var assetController = masterGameObject.AddComponent<AssetController>();
            assetController.Outlines = assetGameObjects;
            assetController.LineCoordinates = allLineCoordinates;
            assetController.ItemId = asset.ItemId;

            // add the master to the stage
            masterGameObject.transform.parent = Stage.transform;

            return masterGameObject;
        }
    }
}
