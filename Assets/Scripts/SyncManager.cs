using Assets.Server;
using Assets.Server.Api;
using Assets.Server.Aqs;
using Assets.Server.Game;
using Assets.Server.Mapper;
using Assets.Server.Projection;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SyncManager : MonoBehaviour
{
    public GameObject PlayerController;
    public GameObject PlayerCamera;
    public float MapSize;
    private LevelController _levelController;
    private Camera _playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        // get the level controller (for the store)
        _levelController = PlayerController.GetComponent<LevelController>();

        _playerCamera = PlayerCamera.GetComponent<Camera>();

        // start loading stuff
        StartCoroutine(CoroutineDiff());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator CoroutineDiff()
    {
        // go sleep, helps startup
        yield return new WaitForSeconds(2.0f);

        // go check session to see if we can get in
        var sessionMeClient = new SessionMeClient(ApplicationGlobals.ApiUrl, ApplicationGlobals.ApiToken);
        yield return sessionMeClient.Send();

        if (sessionMeClient.Error != null)
        {
            Debug.Log("Failed to get session for api key '" + ApplicationGlobals.ApiToken + "' Error: " + sessionMeClient.Error.Message);
            ApplicationGlobals.FatalError = "FAILED TO VERIFY API KEY, CHECK YOUR .CONFIG FILE @ " + ApplicationGlobals.GetConfigFilePath();
            SceneManager.LoadScene("FatalErrorScene");
        }
        else if (sessionMeClient.Response == null)
        {
            Debug.Log("Failed to get session for api key '" + ApplicationGlobals.ApiToken + "' Error: response was null");
            ApplicationGlobals.FatalError = "FAILED TO VERIFY API KEY, (NO SERVER RESPONSE) CHECK YOUR .CONFIG FILE @ " + ApplicationGlobals.GetConfigFilePath();
            SceneManager.LoadScene("FatalErrorScene");
        }
        else
        {
            // token looks good
            ApplicationGlobals.ApiTokenVerified = true;

            // calculate the centroid in metres and setup the projector (maps items to positions on the screen)
            var stageCentroidMetres = WebMercatorProjection.LatLonToMeters(ApplicationGlobals.CentreLat, ApplicationGlobals.CentreLng);
            var stageCoordinateProjector = new StageCoordProjection(MapSize, stageCentroidMetres);

            // setup the prefab mapper (maps items to models of stuff)
            var itemToGameObjectFactory = AssetToGameObjectFactory.Create(gameObject, stageCoordinateProjector, _playerCamera);

            while (true)
            {
                // recursive coroutines start here
                yield return StartCoroutine(SyncAssetsCoroutine(1, itemToGameObjectFactory));
                yield return StartCoroutine(SyncJobsCoroutine(1));
                yield return StartCoroutine(SyncInspectionsCoroutine(1));

                Debug.Log($"Finished syncing @{Time.realtimeSinceStartup}s, sleeping...");
                yield return new WaitForSeconds(5.0f);
            }
        }
    }

    private IEnumerator SyncAssetsCoroutine(int page, AssetToGameObjectFactory itemToGameObjectFactory)
    {
        var aqs = new AqsNode("Query");
        aqs.SetPropertyString("dodiCode", "designInterfaces_assetHeads");
        aqs.SetPropertyObject("attributes", "[\"attributes_itemsGeometry\", \"attributes_itemsTitle\", \"attributes_itemsSubtitle\"]");

        int pageSize = 100;
        var aqsClient = new AqsClient(ApplicationGlobals.ApiUrl, ApplicationGlobals.ApiToken, aqs.Stringify(), false, page, pageSize);
        yield return aqsClient.Send();

        if (aqsClient.Error != null)
        {
            Debug.Log("Failed to get AQS response for asset sync, waiting 5 seconds... Error: " + aqsClient.Error.Message);
            yield return new WaitForSeconds(5.0f);
        }
        else if (aqsClient.Response == null)
        {
            Debug.Log("Failed to get AQS response for asset sync, waiting 5 seconds... Error: response was null");
            yield return new WaitForSeconds(5.0f);
        }
        else
        {
            foreach (var jsonItem in aqsClient.Response.Results)
            {
                // if blacklisted then skip or if we already have the asset then skip
                if (_levelController.GameStore.IsBlacklistedItemId(jsonItem.ItemId) || _levelController.GameStore.GetAsset(jsonItem.ItemId) != null)
                {
                    continue;
                }

                // create item to keep and manage
                var title = jsonItem.Attributes.FirstOrDefault(a => a.AttributeCode == "attributes_itemsTitle")?.ValueAsString();
                var subtitle = jsonItem.Attributes.FirstOrDefault(a => a.AttributeCode == "attributes_itemsSubtitle")?.ValueAsString();
                var geometry = jsonItem.Attributes.FirstOrDefault(a => a.AttributeCode == "attributes_itemsGeometry")?.ValueAsGeoJson();

                // if we don't have geom then be gone!
                if (geometry == null)
                {
                    continue;
                }

                var asset = new AssetModel(jsonItem.ItemId, jsonItem.DesignCode, title, subtitle, jsonItem.Signature, geometry);

                // make the game object for the asset
                var go = itemToGameObjectFactory.CreateGameObjectForAsset(asset);

                // if we can't make a game object? (maybe complex poly?)
                if (go == null)
                {
                    continue;
                }

                // otherwise set the assets game object to manage
                asset.GameObject = go;

                // add to store
                _levelController.GameStore.AddAsset(asset);

                // yield and await more work
                yield return null;
            }

            // if we have a full page, there might be more
            if (aqsClient.Response.Results.Count == pageSize)
            {
                // 1/2 second between pages
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(SyncAssetsCoroutine(++page, itemToGameObjectFactory));
            }
        }
    }

    private IEnumerator SyncJobsCoroutine(int page)
    {
        var aqs = new AqsNode("Join");
        aqs.SetPropertyString("dodiCode", "designInterfaces_jobs");
        aqs.SetPropertyObject("attributes", "[\"attributes_itemsTitle\", \"attributes_itemsSubtitle\"]");

        // join to parent asset, we have to get an attribute so just get title, we use this later to find the query (should only be one anyway)
        string joinAttributeToMatch = "root^attributes_tasksAssignableTasks.attributes_itemsTitle";
        aqs.SetPropertyObject("joinAttributes", "[\"" + joinAttributeToMatch + "\"]");

        int pageSize = 100;
        var aqsClient = new AqsClient(ApplicationGlobals.ApiUrl, ApplicationGlobals.ApiToken, aqs.Stringify(), true, page, pageSize);
        yield return aqsClient.Send();

        if (aqsClient.Error != null)
        {
            Debug.Log("Failed to get AQS response for jobs sync, waiting 5 seconds... Error: " + aqsClient.Error.Message);
            yield return new WaitForSeconds(5.0f);
        }
        else if (aqsClient.Response == null)
        {
            Debug.Log("Failed to get AQS response for jobs sync, waiting 5 seconds... Error: response was null");
            yield return new WaitForSeconds(5.0f);
        }
        else
        {
            // process the join results to key on item id
            var joinResultsByItemId = aqsClient.Response.JoinResults?.ToDictionary(r => r.ItemId, r => r.JoinQueries);

            foreach (var jsonItem in aqsClient.Response.Results)
            {
                // check if the join results has a value, if not, it isn't connected to an asset (shouldn't be possible based on query)
                if (joinResultsByItemId == null || !joinResultsByItemId.ContainsKey(jsonItem.ItemId))
                {
                    continue;
                }
                // if blacklisted then skip or if we already have the job then skip
                if (_levelController.GameStore.IsBlacklistedItemId(jsonItem.ItemId) || _levelController.GameStore.GetJob(jsonItem.ItemId) != null)
                {
                    continue;
                }

                // try to get the parent asset from the join results, if not then the job has no asset parent
                var parentAssetJson = joinResultsByItemId[jsonItem.ItemId].FirstOrDefault(q => q.JoinAttributes.Any(a => a == joinAttributeToMatch));
                if (parentAssetJson == null)
                {
                    continue;
                }
                // special case, job has multiple asset parents
                if (parentAssetJson.Item == null)
                {
                    Debug.Log("Job has multiple parent assets, skipping: " + jsonItem.ItemId);
                    continue;
                }
                // check the parent asset exists in our store
                if (_levelController.GameStore.GetAsset(parentAssetJson.Item.ItemId) == null)
                {
                    Debug.Log("Job parent asset has not been synced, skipping: " + jsonItem.ItemId);
                    continue;
                }

                // create item to keep and manage
                var title = jsonItem.Attributes.FirstOrDefault(a => a.AttributeCode == "attributes_itemsTitle")?.ValueAsString();
                var subtitle = jsonItem.Attributes.FirstOrDefault(a => a.AttributeCode == "attributes_itemsSubtitle")?.ValueAsString();
                var job = new JobModel(parentAssetJson.Item.ItemId, jsonItem.ItemId, jsonItem.DesignCode, title, subtitle, jsonItem.Signature);
                
                // add to store
                _levelController.GameStore.AddJob(job);
                
                // yield and await more work
                yield return null;
            }

            // if we have a full page, there might be more
            if (aqsClient.Response.Results.Count == pageSize)
            {
                // 1/2 second between pages
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(SyncJobsCoroutine(++page));
            }
        }
    }

    private IEnumerator SyncInspectionsCoroutine(int page)
    {
        var aqs = new AqsNode("Join");
        aqs.SetPropertyString("dodiCode", "designInterfaces_inspections");
        aqs.SetPropertyObject("attributes", "[\"attributes_itemsTitle\", \"attributes_itemsSubtitle\"]");

        // join to parent asset, we have to get an attribute so just get title, we use this later to find the query (should only be one anyway)
        string joinAttributeToMatch = "root^attributes_tasksAssignableTasks.attributes_itemsTitle";
        aqs.SetPropertyObject("joinAttributes", "[\"" + joinAttributeToMatch + "\"]");

        int pageSize = 100;
        var aqsClient = new AqsClient(ApplicationGlobals.ApiUrl, ApplicationGlobals.ApiToken, aqs.Stringify(), true, page, pageSize);
        yield return aqsClient.Send();

        if (aqsClient.Error != null)
        {
            Debug.Log("Failed to get AQS response for inspections sync, waiting 5 seconds... Error: " + aqsClient.Error.Message);
            yield return new WaitForSeconds(5.0f);
        }
        else if (aqsClient.Response == null)
        {
            Debug.Log("Failed to get AQS response for inspections sync, waiting 5 seconds... Error: response was null");
            yield return new WaitForSeconds(5.0f);
        }
        else
        {
            // process the join results to key on item id
            var joinResultsByItemId = aqsClient.Response.JoinResults?.ToDictionary(r => r.ItemId, r => r.JoinQueries);

            foreach (var jsonItem in aqsClient.Response.Results)
            {
                // check if the join results has a value, if not, it isn't connected to an asset (shouldn't be possible based on query)
                if (joinResultsByItemId == null || !joinResultsByItemId.ContainsKey(jsonItem.ItemId))
                {
                    continue;
                }
                // if blacklisted then skip or if we already have the inspection then skip
                if (_levelController.GameStore.IsBlacklistedItemId(jsonItem.ItemId) || _levelController.GameStore.GetInspection(jsonItem.ItemId) != null)
                {
                    continue;
                }

                // try to get the parent asset from the join results, if not then the job has no asset parent
                var parentAssetJson = joinResultsByItemId[jsonItem.ItemId].FirstOrDefault(q => q.JoinAttributes.Any(a => a == joinAttributeToMatch));
                if (parentAssetJson == null)
                {
                    continue;
                }
                // special case, job has multiple asset parents
                if (parentAssetJson.Item == null)
                {
                    Debug.Log("Inspection has multiple parent assets, skipping: " + jsonItem.ItemId);
                    continue;
                }
                // check the parent asset exists in our store
                if (_levelController.GameStore.GetAsset(parentAssetJson.Item.ItemId) == null)
                {
                    Debug.Log("Inspection parent asset has not been synced, skipping: " + jsonItem.ItemId);
                    continue;
                }

                // create item to keep and manage
                var title = jsonItem.Attributes.FirstOrDefault(a => a.AttributeCode == "attributes_itemsTitle")?.ValueAsString();
                var subtitle = jsonItem.Attributes.FirstOrDefault(a => a.AttributeCode == "attributes_itemsSubtitle")?.ValueAsString();
                var inspection = new InspectionModel(parentAssetJson.Item.ItemId, jsonItem.ItemId, jsonItem.DesignCode, title, subtitle, jsonItem.Signature);

                // add to store
                _levelController.GameStore.AddInspection(inspection);

                // yield and await more work
                yield return null;
            }

            // if we have a full page, there might be more
            if (aqsClient.Response.Results.Count == pageSize)
            {
                // 1/2 second between pages
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(SyncInspectionsCoroutine(++page));
            }
        }
    }
}
