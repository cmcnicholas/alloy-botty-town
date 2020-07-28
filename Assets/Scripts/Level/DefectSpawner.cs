﻿using Assets.Server;
using Assets.Server.Game;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DefectSpawner : MonoBehaviour
{
    private LevelController _levelController;
    private float _nextSpawn;

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
    }

    // Update is called once per frame
    void Update()
    {
        // check we allow spawning
        if (ApplicationGlobals.DefectSpawn && _nextSpawn < Time.time)
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
            
            // make a temp defect (we may create it later)
            _levelController.GameStore.AddTempDefect(new TempDefectModel(hit.ItemId, Guid.NewGuid().ToString()));
            Debug.Log("A wild defect was spawned on asset: " + hit.ItemId);

            // decide when next defect spawn will be
            _nextSpawn = Time.time + Random.Range(ApplicationGlobals.DefectSpawnMinTime, ApplicationGlobals.DefectSpawnMaxTime);
        }
    }
}
