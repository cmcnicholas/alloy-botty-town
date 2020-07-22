using System;
using System.Collections.Generic;
using Assets.Server.Models;
using Assets.Server.Projection;
using GeoJSON.Net.Geometry;
using UnityEngine;

namespace Assets.Server.Mapper
{
    /// <summary>
    /// implementation to create game objects for point items
    /// </summary>
    public class PointToGameObjectMapper : ItemToGameObjectMapperBase
    {
        private const string Tree01Prefab = "Tree01Prefab";
        private const string Tree02Prefab = "Tree01Prefab";
        private const string Tree03Prefab = "Tree01Prefab";
        private const string Tree04Prefab = "Tree01Prefab";
        private const string Tree05Prefab = "Tree05Prefab";
        private const string Lamp01Prefab = "Lamp01Prefab";
        private const string TrafficLight01Prefab = "TrafficLight01Prefab";
        private const string Trash01Prefab = "Trash01Prefab";
        private const string Trash02Prefab = "Trash02Prefab";
        private const string Sign01Prefab = "Sign01Prefab";
        private const string House01Prefab = "House01Prefab";

        private IDictionary<string, GameObject> _prefabs;

        public PointToGameObjectMapper(GameObject stage, StageCoordProjection stageCoordProjector, IDictionary<string, GameObject> prefabs): base(stage, stageCoordProjector)
        {
            _prefabs = prefabs;
        }

        public override GameObject CreateGameObjectForItem(ItemModel item)
        {
            // find the prefab model we have for this item
            string prefabName = PrefabNameForItem(item);
            if (!_prefabs.ContainsKey(prefabName))
            {
                return null;
            }

            // get the game object we use as a template
            GameObject template = _prefabs[prefabName];
            
            // get the geometry and expect it to be a point
            var itemPoint = item.Geometry as Point;
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
            return UnityEngine.Object.Instantiate(template, new Vector3(itemStageCoords[0], 0, itemStageCoords[1]), new Quaternion(), Stage.transform);
        }

        private string PrefabNameForItem(ItemModel item)
        {
            switch (item.DesignCode)
            {
                case "designs_streetLights":
                case "designs_otherStreetLights":
                case "designs_signIlluminations":
                case "designs_subwayLights":
                    return Lamp01Prefab;
                case "designs_bollards":
                    return Sign01Prefab;
                case "designs_feederPillars":
                    return Trash01Prefab; // TODO something better here, more money $$$
                case "designs_wasteContainers":
                    return Trash01Prefab;
                case "designs_trees":
                    return Tree01Prefab; // TODO consistently randomize?
                case "designs_nlpgPremises":
                    return House01Prefab;
                default:
                    return "DefaultPrefab"; // no model
            }
        }
    }
}
