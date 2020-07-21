using Assets.Server.Api;
using Assets.Server.Mapper;
using Assets.Server.Models;
using Assets.Server.Projection;
using GeoJSON.Net;
using GeoJSON.Net.Geometry;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject ItemPrefab;
    public float CentreOfWorldLat;
    public float CentreOfWorldLon;
    public float MapSize;
    private string ApiUrl = "https://api.labs.alloyapp.io";
    private string ApiKey = "e2c58fef-c0ce-4e15-883d-31fd6edba206";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CoroutineDiff());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator CoroutineDiff()
    {
        // setup the prefab mapper (maps items to models of stuff)
        var itemPrefabMapper = new ItemPrefabMapper();
        itemPrefabMapper.Initialise(gameObject, ItemPrefab);

        // calculate the centroid in metres and setup the projector (maps items to positions on the screen)
        var stageCentroidMetres = WebMercatorProjection.LatLonToMeters(CentreOfWorldLat, CentreOfWorldLon);
        var stageCoordinateProjector = new StageCoordProjection(MapSize, stageCentroidMetres);

        // keep track of what we have loaded
        var itemIdsLoaded = new HashSet<string>();

        while (true)
        {
            // recursive coroutines start here
            yield return StartCoroutine(CoroutineDiffPage(1, itemPrefabMapper, stageCoordinateProjector, itemIdsLoaded));

            Debug.Log("Finished loading assets, waiting 5 seconds...");
            yield return new WaitForSeconds(5.0f);
        }
    }

    private IEnumerator CoroutineDiffPage(int page, ItemPrefabMapper itemPrefabMapper, StageCoordProjection stageCoordinateProjector, ISet<string> itemIdsLoaded)
    {
        string aqs = "{ \"type\": \"Query\", \"properties\": { \"dodiCode\": \"designInterfaces_assetHeads\", \"attributes\": [\"attributes_itemsGeometry\", \"attributes_itemsTitle\", \"attributes_itemsSubtitle\"] } }";
        var aqsClient = new AqsClient(ApiUrl, ApiKey, aqs, page);
        yield return aqsClient.Send();

        if (aqsClient.Error != null)
        {
            Debug.Log("Failed to get AQS response, waiting 5 seconds... Error: " + aqsClient.Error.Message);
            yield return new WaitForSeconds(5.0f);
        }
        else if (aqsClient.Response == null)
        {
            Debug.Log("Failed to get AQS response, waiting 5 seconds... Error: response was null");
            yield return new WaitForSeconds(5.0f);
        }
        else
        {
            foreach (var jsonItem in aqsClient.Response.Results)
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

                    // create item to keep and manage
                    var item = new ItemModel(jsonItem.ItemId, jsonItem.DesignCode, itemStageCoords[0], itemStageCoords[1]);

                    // map the item to a prefab (will add to the screen)
                    itemPrefabMapper.ItemToPrefab(item);

                    // indicate we loaded
                    itemIdsLoaded.Add(item.ItemId);

                    // yield and await more work
                    yield return null;
                }
            }

            // if we have more pages, go get them
            if (aqsClient.Response.TotalPages > page)
            {
                yield return StartCoroutine(CoroutineDiffPage(++page, itemPrefabMapper, stageCoordinateProjector, itemIdsLoaded));
            }
        }
    }
}
