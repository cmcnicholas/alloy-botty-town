using Assets.Server.Models;
using Assets.Server.Projection;
using GeoJSON.Net;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Server.Mapper
{
    public class ItemToGameObjectFactory
    {
        private IDictionary<GeoJSONObjectType, ItemToGameObjectMapperBase> _mappers = new Dictionary<GeoJSONObjectType, ItemToGameObjectMapperBase>();

        public void Initialise(GameObject stage, StageCoordProjection stageCoordProjector, GameObject itemPrefabs)
        {
            // enumerate the prefab game objects, we use an empty game object prefab to house several game objects
            // the name is the key we use to look them up and assign them to items e.g. street lights
            var prefabs = new Dictionary<string, GameObject>();
            foreach (Transform child in itemPrefabs.transform)
            {
                prefabs.Add(child.name, child.gameObject);
            }

            // set the mappers
            _mappers[GeoJSONObjectType.Point] = new PointToGameObjectMapper(stage, stageCoordProjector, prefabs);
            _mappers[GeoJSONObjectType.LineString] = new LineStringToGameObjectMapper(stage, stageCoordProjector);
        }

        public GameObject GetGameObjectForItem(ItemModel item)
        {
            // do we have geom?
            if (item.Geometry == null)
            {
                return null;
            }

            // do we support the item geom?
            if (!_mappers.ContainsKey(item.Geometry.Type))
            {
                return null;
            }

            // go ahead and make some game object
            var go = _mappers[item.Geometry.Type].GetGameObjectForItem(item);
            if (go != null)
            {
                go.name = "itemGameObjects_" + item.ItemId;
            }
            return go;
        }
    }
}
