using Assets.Server.Game;
using Assets.Server.Projection;
using GeoJSON.Net;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Server.Mapper
{
    public class AssetToGameObjectFactory
    {
        /// <summary>
        /// creates the factory, performing initialisation for the mappers
        /// </summary>
        public static AssetToGameObjectFactory Create(GameObject stage, StageCoordProjection stageCoordProjector, Camera playerCamera)
        {
            // load the road material
            var roadMaterial = Resources.Load("Materials/Road") as Material;
            var itemPrefabs = Resources.Load("Prefabs/ItemPrefab") as GameObject;

            // enumerate the prefab game objects, we use an empty game object prefab to house several game objects
            // the name is the key we use to look them up and assign them to items e.g. street lights
            var prefabs = new Dictionary<string, GameObject>();
            foreach (Transform child in itemPrefabs.transform)
            {
                prefabs.Add(child.name, child.gameObject);
            }

            // create the mappers
            var mappers = new Dictionary<GeoJSONObjectType, AssetToGameObjectMapperBase>();
            mappers[GeoJSONObjectType.Point] = new PointToGameObjectMapper(stage, stageCoordProjector, prefabs);
            mappers[GeoJSONObjectType.LineString] = new LineStringToGameObjectMapper(stage, stageCoordProjector, roadMaterial, playerCamera);
            mappers[GeoJSONObjectType.Polygon] = new PolygonToGameObjectMapper(stage, stageCoordProjector, roadMaterial);

            return new AssetToGameObjectFactory(mappers);
        }

        private IDictionary<GeoJSONObjectType, AssetToGameObjectMapperBase> _mappers;

        private AssetToGameObjectFactory(IDictionary<GeoJSONObjectType, AssetToGameObjectMapperBase> mappers)
        {
            _mappers = mappers;
        }
        
        /// <summary>
        /// gets a game object for an asset model
        /// </summary>
        public GameObject CreateGameObjectForAsset(AssetModel asset)
        {
            // do we have geom?
            if (asset.Geometry == null)
            {
                return null;
            }

            // do we support the item geom?
            if (!_mappers.ContainsKey(asset.Geometry.Type))
            {
                return null;
            }

            // go ahead and make some game object
            var go = _mappers[asset.Geometry.Type].CreateGameObjectForAsset(asset);
            if (go != null)
            {
                go.name = "itemGameObjects_" + asset.ItemId;
            }
            return go;
        }
    }
}
