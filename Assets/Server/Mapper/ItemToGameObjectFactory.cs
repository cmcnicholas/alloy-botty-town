using Assets.Server.Models;
using Assets.Server.Projection;
using GeoJSON.Net;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Server.Mapper
{
    public class ItemToGameObjectFactory
    {
        /// <summary>
        /// creates the factory, performing initialisation for the mappers
        /// </summary>
        public static ItemToGameObjectFactory Create(GameObject stage, StageCoordProjection stageCoordProjector, GameObject itemPrefabs, Material roadMaterial)
        {
            // enumerate the prefab game objects, we use an empty game object prefab to house several game objects
            // the name is the key we use to look them up and assign them to items e.g. street lights
            var prefabs = new Dictionary<string, GameObject>();
            foreach (Transform child in itemPrefabs.transform)
            {
                prefabs.Add(child.name, child.gameObject);
            }

            // create the mappers
            var mappers = new Dictionary<GeoJSONObjectType, ItemToGameObjectMapperBase>();
            mappers[GeoJSONObjectType.Point] = new PointToGameObjectMapper(stage, stageCoordProjector, prefabs);
            mappers[GeoJSONObjectType.LineString] = new LineStringToGameObjectMapper(stage, stageCoordProjector, roadMaterial);
            mappers[GeoJSONObjectType.Polygon] = new PolygonToGameObjectMapper(stage, stageCoordProjector, roadMaterial);

            return new ItemToGameObjectFactory(mappers);
        }

        private IDictionary<GeoJSONObjectType, ItemToGameObjectMapperBase> _mappers;

        private ItemToGameObjectFactory(IDictionary<GeoJSONObjectType, ItemToGameObjectMapperBase> mappers)
        {
            _mappers = mappers;
        }
        
        /// <summary>
        /// gets a game object for an item model
        /// </summary>
        public GameObject CreateGameObjectForItem(ItemModel item)
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
            var go = _mappers[item.Geometry.Type].CreateGameObjectForItem(item);
            if (go != null)
            {
                go.name = "itemGameObjects_" + item.ItemId;
            }
            return go;
        }
    }
}
