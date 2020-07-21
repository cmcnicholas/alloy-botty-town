using Assets.Server.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Server.Mapper
{
    public class ItemPrefabMapper
    {
        private GameObject _stage;
        private IDictionary<string, GameObject> _itemPrefabs = new Dictionary<string, GameObject>();

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

        public void Initialise(GameObject stage, GameObject itemPrefabs)
        {
            _stage = stage;

            // enumerate the prefab game objects
            foreach (Transform child in itemPrefabs.transform)
            {
                _itemPrefabs.Add(child.name, child.gameObject);
            }
        }

        public GameObject ItemToPrefab(ItemModel item)
        {
            string prefabName = PrefabNameForItem(item);
            if (!_itemPrefabs.ContainsKey(prefabName))
            {
                return null;
            }

            GameObject template = _itemPrefabs[prefabName];
            
            var instanced = Object.Instantiate(template, new Vector3(item.WorldX, 0, item.WorldZ), new Quaternion(), _stage.transform);
            instanced.name = "gameObjectItems_" + item.ItemId;
            return instanced;
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
                    return "Trash01Prefab";
                default:
                    return "DefaultPrefab"; // no model
            }
        }
    }
}
