using Assets.Server.Game;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Server.Mapper
{
    public class PrefabManager
    {
        private IDictionary<string, GameObject> _cachedTemplates = new Dictionary<string, GameObject>();

        public GameObject CreateForAsset(AssetModel asset)
        {
            string prefabName = PrefabNameForItem(asset);

            // if we don't have the prefab, load it and cache the work once
            if (!_cachedTemplates.ContainsKey(prefabName))
            {
                GameObject template = Resources.Load(prefabName) as GameObject;

                // check if we need to apply any scaling etc. to the template once
                var config = ApplicationGlobals.PrefabMapping[prefabName];

                // modify scale if we need to
                if (config.Scale != 1.0f)
                {
                    template.transform.localScale = new Vector3(config.Scale, config.Scale, config.Scale);
                }

                // try to get the actual game object we want, for some of the prefabs this is at the root, others it's a child...
                // we need to add a collider
                GameObject assetGameObject = null;
                if (template.TryGetComponent<MeshFilter>(out MeshFilter filter))
                {
                    var collider = template.AddComponent<MeshCollider>();
                    collider.sharedMesh = filter.sharedMesh;
                    assetGameObject = template;
                }
                else
                {
                    // iterate through the child objects to find the mesh filter and apply collider
                    foreach (Transform child in template.transform)
                    {
                        if (child.TryGetComponent<MeshFilter>(out MeshFilter childFilter))
                        {
                            var collider = child.gameObject.AddComponent<MeshCollider>();
                            collider.sharedMesh = childFilter.sharedMesh;
                            assetGameObject = child.gameObject;
                            break;
                        }
                    }
                }

                if (assetGameObject is null)
                {
                    Debug.Log("could not find asset game object for prefab: " + prefabName);
                    return new GameObject(); // empty object
                }

                // get the renderer to set some props
                var renderer = assetGameObject.GetComponentInChildren<MeshRenderer>();
                if (renderer == null)
                {
                    Debug.Log("could not find mesh renderer object for prefab: " + prefabName);
                }
                else
                {
                    renderer.receiveShadows = false;
                    renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Simple;
                }

                // cache all the work to reuse
                _cachedTemplates.Add(prefabName, assetGameObject);
            }

            var instance = GameObject.Instantiate(_cachedTemplates[prefabName]);
            instance.name = "Asset";
            return instance;
        }

        public static string PrefabNameForItem(AssetModel asset)
        {
            // no design? use default
            if (!ApplicationGlobals.PrefabReverseMapping.ContainsKey(asset.DesignCode))
            {
                return ApplicationGlobals.PrefabDefault;
            }

            // find any suitable prefabs
            var suitablePrefabs = ApplicationGlobals.PrefabReverseMapping[asset.DesignCode];
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
            return suitablePrefabs[Random.Range(0, prefabsCount - 1)];
        }
    }
}
