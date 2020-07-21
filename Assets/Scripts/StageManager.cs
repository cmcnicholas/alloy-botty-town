using Assets.Server.ApiModels;
using Assets.Server.Models;
using Assets.Server.Projection;
using GeoJSON.Net;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class StageManager : MonoBehaviour
{
    public GameObject ItemPrefab;
    public float CentreOfWorldLat;
    public float CentreOfWorldLon;
    public float MapSize;
    private IDictionary<string, GameObject> _itemPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        // enumerate the prefab game objects
        _itemPrefabs = new Dictionary<string, GameObject>();
        foreach (Transform child in ItemPrefab.transform)
        {
            _itemPrefabs.Add(child.name, child.gameObject);
        }
        
        StartCoroutine(CoroutineDiff());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator CoroutineDiff()
    {
        var stageCentroidMetres = WebMercatorProjection.LatLonToMeters(CentreOfWorldLat, CentreOfWorldLon);
        var stageCoordinateProjector = new StageCoordProjection(MapSize, stageCentroidMetres);

        var itemIdsLoaded = new HashSet<string>();

        while (true)
        {
            using (var request = new UnityWebRequest("https://api.labs.alloyapp.io/api/aqs/query?pageSize=100&token=e2c58fef-c0ce-4e15-883d-31fd6edba206", UnityWebRequest.kHttpVerbPOST))
            {
                string requestJson = "{ \"aqs\": { \"type\": \"Query\", \"properties\": { \"dodiCode\": \"designInterfaces_assetHeads\", \"attributes\": [\"attributes_itemsGeometry\", \"attributes_itemsTitle\", \"attributes_itemsSubtitle\"] } } }";
                var uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestJson));
                uploadHandler.contentType = "application/json";
                request.uploadHandler = uploadHandler;

                var downloadHandler = new DownloadHandlerBuffer();
                request.downloadHandler = downloadHandler;

                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log("Failed to get AQS response, waiting 5 seconds...");
                    yield return new WaitForSeconds(5.0f);
                }
                else
                {
                    var json = JsonConvert.DeserializeObject<AqsResponseWebModel>(request.downloadHandler.text);

                    foreach (var jsonItem in json.Results)
                    {
                        if (itemIdsLoaded.Contains(jsonItem.ItemId))
                        {
                            Debug.Log("Skipping item, already loaded " + jsonItem.ItemId);
                            continue;
                        }

                        var geometry = jsonItem.Attributes.First(a => a.AttributeCode == "attributes_itemsGeometry").ValueAsGeoJson();
                        if (geometry == null)
                        {
                            continue;
                        }

                        if (geometry.Type == GeoJSONObjectType.Point)
                        {
                            var itemLocation = (geometry as Point).Coordinates;
                            var itemMetres = WebMercatorProjection.LatLonToMeters(itemLocation.Latitude, itemLocation.Longitude);
                            var itemStageCoords = stageCoordinateProjector.MetresToStageCoordinate(itemMetres);

                            if (itemStageCoords == null)
                            {
                                Debug.Log("Item outside of map " + jsonItem.ItemId);
                                continue;
                            }

                            var item = new ItemModel(jsonItem.ItemId, itemStageCoords[0], itemStageCoords[1]);
                            NewItem(new ItemChangeModel(item, ItemChangeType.New));
                            itemIdsLoaded.Add(item.ItemId); // indicate we loaded
                            yield return null;
                        }
                    }

                    Debug.Log("Finished loading assets, waiting 5 seconds...");
                    yield return new WaitForSeconds(5.0f);
                }
            }
        }
    }

    private void NewItem(ItemChangeModel change)
    {
        var component = _itemPrefabs.ContainsKey("LampPrefab") ? _itemPrefabs["LampPrefab"] : null;

        // no component defined for this item
        if (component == null)
        {
            return;
        }
        
        var newComponent = Instantiate(component, new Vector3(change.Item.WorldX, 0, change.Item.WorldZ), 
            new Quaternion(), gameObject.transform);
        newComponent.name = "gameObjectItems_" + change.Item.ItemId;
    }
}
