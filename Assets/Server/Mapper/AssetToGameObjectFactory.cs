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
        public static AssetToGameObjectFactory Create(GameObject stage, StageCoordProjection stageCoordProjector)
        {
            // load the road material
            var roadMaterial = Resources.Load("Materials/Road") as Material;

            var prefabManager = new PrefabManager();

            // create the mappers
            var mappers = new Dictionary<GeoJSONObjectType, AssetToGameObjectMapperBase>();
            mappers[GeoJSONObjectType.Point] = new PointToGameObjectMapper(stage, stageCoordProjector, prefabManager);
            mappers[GeoJSONObjectType.MultiPoint] = new MultiPointToGameObjectMapper(stage, stageCoordProjector, prefabManager);
            mappers[GeoJSONObjectType.LineString] = new LineStringToGameObjectMapper(stage, stageCoordProjector, roadMaterial);
            mappers[GeoJSONObjectType.MultiLineString] = new MultiLineStringToGameObjectMapper(stage, stageCoordProjector, roadMaterial);
            mappers[GeoJSONObjectType.Polygon] = new PolygonToGameObjectMapper(stage, stageCoordProjector, prefabManager, roadMaterial);
            mappers[GeoJSONObjectType.MultiPolygon] = new MultiPolygonToGameObjectMapper(stage, stageCoordProjector, prefabManager, roadMaterial);
            mappers[GeoJSONObjectType.GeometryCollection] = new GeometryCollectionToGameObjectMapper(stage, stageCoordProjector, prefabManager, roadMaterial, roadMaterial);

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
