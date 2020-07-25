using Assets.Server.Api;
using Assets.Server.Game;
using Assets.Server.Mapper;
using Assets.Server.Projection;
using System.Collections;
using System.Linq;
using UnityEngine;

public class SyncManager : MonoBehaviour
{
    public GameObject PlayerController;
    public float CentreOfWorldLat;
    public float CentreOfWorldLon;
    public float MapSize;
    public string ApiUrl;
    public string ApiKey;
    private LevelController _levelController;

    // Start is called before the first frame update
    void Start()
    {
        // get the level controller (for the store)
        _levelController = PlayerController.GetComponent<LevelController>();

        // start loading stuff
        StartCoroutine(CoroutineDiff());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator CoroutineDiff()
    {
        // calculate the centroid in metres and setup the projector (maps items to positions on the screen)
        var stageCentroidMetres = WebMercatorProjection.LatLonToMeters(CentreOfWorldLat, CentreOfWorldLon);
        var stageCoordinateProjector = new StageCoordProjection(MapSize, stageCentroidMetres);

        // setup the prefab mapper (maps items to models of stuff)
        var itemToGameObjectFactory = AssetToGameObjectFactory.Create(gameObject, stageCoordinateProjector);

        while (true)
        {
            // recursive coroutines start here
            yield return StartCoroutine(CoroutineDiffPage(1, itemToGameObjectFactory));

            Debug.Log("Finished loading assets, waiting 5 seconds...");
            yield return new WaitForSeconds(5.0f);
        }
    }

    private IEnumerator CoroutineDiffPage(int page, AssetToGameObjectFactory itemToGameObjectFactory)
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
                if (_levelController.GameStore.GetAsset(jsonItem.ItemId) != null)
                {
                    Debug.Log("Skipping item, already loaded " + jsonItem.ItemId);
                    continue;
                }

                // create item to keep and manage
                var geometry = jsonItem.Attributes.First(a => a.AttributeCode == "attributes_itemsGeometry").ValueAsGeoJson();
                var asset = new AssetModel(jsonItem.ItemId, jsonItem.DesignCode, geometry);

                // make the game object for the asset
                asset.GameObject = itemToGameObjectFactory.CreateGameObjectForAsset(asset);

                // add to store
                _levelController.GameStore.AddAsset(asset);

                if (Random.value > 0.75f)
                {
                    var job = new JobModel(asset.ItemId, asset.ItemId + "XXX", "YYY");
                    _levelController.GameStore.AddJob(job);
                }
                if (Random.value > 0.75f)
                {
                    var inspection = new InspectionModel(asset.ItemId, asset.ItemId + "YYY", "ZZZ");
                    _levelController.GameStore.AddInspection(inspection);
                }

                // yield and await more work
                yield return null;
            }

            // if we have more pages, go get them
            if (aqsClient.Response.TotalPages > page)
            {
                yield return StartCoroutine(CoroutineDiffPage(++page, itemToGameObjectFactory));
            }
        }
    }
}
