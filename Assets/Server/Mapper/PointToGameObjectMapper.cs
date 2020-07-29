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
        private IDictionary<string, GameObject> _prefabs;

        public PointToGameObjectMapper(GameObject stage, StageCoordProjection stageCoordProjector, IDictionary<string, GameObject> prefabs): base(stage, stageCoordProjector)
        {
            _prefabs = prefabs;
        }

        public override GameObject CreateGameObjectForAsset(AssetModel asset)
        {
            // find the prefab model we have for this item
            string prefabName = PrefabNameForItem(asset);
            if (!_prefabs.ContainsKey(prefabName))
            {
                return null;
            }

            // get the game object we use as a template
            GameObject template = _prefabs[prefabName];
            
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

            // make the instance
            var go = UnityEngine.Object.Instantiate(template, new Vector3(itemStageCoords[0], 0, itemStageCoords[1]), new Quaternion(), Stage.transform);

            // set the item id of the asset
            go.GetComponent<AssetController>().ItemId = asset.ItemId;

            return go;
        }

        private string PrefabNameForItem(AssetModel item)
        {
            // no design? use default
            if (!ApplicationGlobals.PrefabReverseMapping.ContainsKey(item.DesignCode))
            {
                return ApplicationGlobals.PrefabDefault;
            }

            // find any suitable prefabs
            var suitablePrefabs = ApplicationGlobals.PrefabReverseMapping[item.DesignCode];
            int prefabsCount = suitablePrefabs.Count;

            // no suitable? use default
            if (prefabsCount == 0)
            {
                return ApplicationGlobals.PrefabDefault;
            }

            // only 1? use it
            if (prefabsCount == 1)
            {
                return suitablePrefabs[0];
            }

            // otherwise use a random prefab
            return suitablePrefabs[UnityEngine.Random.Range(0, prefabsCount - 1)];
        }
    }
}
