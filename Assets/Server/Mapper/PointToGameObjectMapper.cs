using System;
using System.Collections.Generic;
using Assets.Server.Game;
using Assets.Server.Projection;
using GeoJSON.Net.Geometry;
using UnityEngine;

namespace Assets.Server.Mapper
{
    /// <summary>
    /// implementation to create game objects for point items
    /// </summary>
    public class PointToGameObjectMapper : AssetToGameObjectMapperBase
    {
        private PrefabManager _prefabManager;

        public PointToGameObjectMapper(GameObject stage, StageCoordProjection stageCoordProjector, PrefabManager prefabManager): base(stage, stageCoordProjector)
        {
            _prefabManager = prefabManager;
        }

        public override GameObject CreateGameObjectForAsset(AssetModel asset)
        {
            // get the geometry and expect it to be a point
            var itemPoint = asset.Geometry as Point;
            if (itemPoint == null)
            {
                throw new Exception("Expected item model geometry to be of type Point");
            }

            // make the game object
            var go = CreateGameObjectForPoint(_prefabManager, StageCoordProjector, asset, itemPoint, out GameObject assetGameObject);
            
            // setup the asset controller
            var assetController = go.AddComponent<AssetController>();
            assetController.Outlines = new List<GameObject> { assetGameObject };
            assetController.Points = new List<GameObject> { assetGameObject };
            assetController.ItemId = asset.ItemId;

            // finally add the new object to the stage
            go.transform.parent = Stage.transform;

            return go;
        }

        // used for multi geom too
        public static GameObject CreateGameObjectForPoint(PrefabManager prefabManager, StageCoordProjection stageCoordProjector, 
            AssetModel asset, Point itemPoint, out GameObject assetGameObject)
        {
            // calculate the world coordinates (metres) for the point
            var itemMetres = WebMercatorProjection.LatLonToMeters(itemPoint.Coordinates.Latitude, itemPoint.Coordinates.Longitude);

            // project the metres to the stage
            var itemStageCoords = stageCoordProjector.MetresToStageCoordinate(itemMetres);

            // outside of map?
            if (itemStageCoords == null)
            {
                assetGameObject = null;
                return null;
            }

            // make the instance 
            var go = new GameObject();

            // make a game object of the model and add the asset to the parent
            assetGameObject = prefabManager.CreateForAsset(asset);
            assetGameObject.transform.parent = go.transform;
            assetGameObject.transform.localPosition = new Vector3(0, 0, 0);

            // move the outer game object to the right spot
            go.transform.position = new Vector3(itemStageCoords[0], 0, itemStageCoords[1]);

            return go;
        }
    }
}
