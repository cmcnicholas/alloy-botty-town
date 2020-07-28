using Assets.Server;
using Assets.Server.Api;
using Assets.Server.ApiModels;
using Assets.Server.Game;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefectSpawner : MonoBehaviour
{
    private LevelController _levelController;
    private float _nextSpawn;
    private bool _spawning = false;

    // Start is called before the first frame update
    void Start()
    {
        // get level controller
        _levelController = GetComponent<LevelController>();
    }

    void Awake()
    {
        // if spawning is disabled, then don't
        enabled = ApplicationGlobals.DefectSpawn;

        // decide when next defect spawn will be
        _nextSpawn = Time.time + Random.Range(ApplicationGlobals.DefectSpawnMinTime, ApplicationGlobals.DefectSpawnMaxTime);
        _spawning = false;
    }

    // Update is called once per frame
    void Update()
    {
        // check we allow spawning
        if (ApplicationGlobals.DefectSpawn && !_spawning && _nextSpawn < Time.time)
        {
            var hitColliders = Physics.OverlapSphere(transform.position, Random.Range(ApplicationGlobals.DefectSpawnMinRange, ApplicationGlobals.DefectSpawnMaxRange));
            var assetControllers = hitColliders.Select(h => h.transform.parent?.GetComponent<AssetController>()).Where(a => a != null).ToList();

            // if nothing to defect on then wait a small amount and retry
            if (assetControllers.Count == 0)
            {
                _nextSpawn = Time.time + 1f;
                return;
            }

            // pick a random hit
            var hit = assetControllers[Random.Range(0, assetControllers.Count)];

            // we're about to start spawing
            _spawning = true;
            StartCoroutine(SpawnDefectCoroutine(hit));
        }
    }

    private IEnumerator SpawnDefectCoroutine(AssetController assetController)
    {
        var itemCreate = new ItemCreateWebRequestModel(ApplicationGlobals.DefectCreateDesignCode, ApplicationGlobals.DefectCreateCollection);
        itemCreate.SetAttributes(ApplicationGlobals.DefectCreateAttributes);
        itemCreate.Parents.Add("attributes_defectsAssignableDefects", new List<string> { assetController.ItemId });
        var defectClient = new DefectCreateClient(ApplicationGlobals.ExtendedApiUrl, ApplicationGlobals.ApiToken, itemCreate);

        var startTime = Time.time;
        yield return defectClient.Send();

        // wait for a minimum of 3 seconds
        var minWait = 3f - (Time.time - startTime);
        if (minWait > 0)
        {
            yield return new WaitForSeconds(minWait);
        }

        if (defectClient.Error != null)
        {
            Debug.Log("Failed to create defect... Error: " + defectClient.Error.Message);
        }
        else if (defectClient.Response == null)
        {
            Debug.Log("Failed to create defect... Error: response was null");
        }
        else
        {
            var jsonItem = defectClient.Response.DefectItem;
            var title = jsonItem.Attributes.FirstOrDefault(a => a.AttributeCode == "attributes_itemsTitle")?.ValueAsString();
            var subtitle = jsonItem.Attributes.FirstOrDefault(a => a.AttributeCode == "attributes_itemsSubtitle")?.ValueAsString();
            _levelController.GameStore.AddDefect(new DefectModel(assetController.ItemId, jsonItem.ItemId, jsonItem.DesignCode, title, subtitle, jsonItem.Signature));
            Debug.Log("A wild defect was spawned: " + jsonItem.ItemId);
        }

        // decide when next defect spawn will be
        _nextSpawn = Time.time + Random.Range(ApplicationGlobals.DefectSpawnMinTime, ApplicationGlobals.DefectSpawnMaxTime);
        _spawning = false;
    }
}
