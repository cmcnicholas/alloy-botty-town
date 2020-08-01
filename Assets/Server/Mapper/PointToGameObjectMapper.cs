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
            
            // calculate the world coordinates (metres) for the point
            var itemMetres = WebMercatorProjection.LatLonToMeters(itemPoint.Coordinates.Latitude, itemPoint.Coordinates.Longitude);

            // project the metres to the stage
            var itemStageCoords = StageCoordProjector.MetresToStageCoordinate(itemMetres);

            // outside of map?
            if (itemStageCoords == null)
            {
                return null;
            }

            // make the instance and move it to a position
            var go = new GameObject();
            
            // make a game object of the model and add the asset to the parent
            GameObject assetGameObject = _prefabManager.CreateForAsset(asset);
            assetGameObject.transform.parent = go.transform;
            assetGameObject.transform.localPosition = new Vector3(0, 0, 0);

            // setup the asset controller
            var assetController = go.AddComponent<AssetController>();
            assetController.Assets = new List<GameObject> { assetGameObject };
            assetController.IsLineString = false;
            assetController.IsPolygon = false;
            assetController.ItemId = asset.ItemId;

            // move the outer game objec to the right spot
            go.transform.position = new Vector3(itemStageCoords[0], 0, itemStageCoords[1]);

            // finally add the new object to the stage
            go.transform.parent = Stage.transform;

            return go;
        }
    }
}
