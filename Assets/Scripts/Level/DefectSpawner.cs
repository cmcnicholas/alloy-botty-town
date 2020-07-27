using Assets.Server.Game;
using System.Collections;
using System.Linq;
using UnityEngine;

public class DefectSpawner : MonoBehaviour
{
    public float MinSpawnTime;
    public float MaxSpawnTime;
    public float MinSpawnRange;
    public float MaxSpawnRange;
    private LevelController _levelController;
    private float _nextSpawn;

    // Start is called before the first frame update
    void Start()
    {
        // get level controller
        _levelController = GetComponent<LevelController>();

        // decide when next defect spawn will be
        _nextSpawn = Time.time + Random.Range(MinSpawnTime, MaxSpawnTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (_nextSpawn > Time.time)
        {
            var hitColliders = Physics.OverlapSphere(transform.position, Random.Range(MinSpawnRange, MaxSpawnRange));
            var assetControllers = hitColliders.Select(h => h.transform.parent?.GetComponent<AssetController>()).Where(a => a != null).ToList();

            // if nothing to defect on then wait a small amount and retry
            if (assetControllers.Count == 0)
            {
                _nextSpawn = Time.time + 1f;
                return;
            }

            // pick a random hit
            var hit = assetControllers[Random.Range(0, assetControllers.Count)];

            // set next spawn to never update (this will get reset at the end of coroutine)
            _nextSpawn = -1;
            StartCoroutine(SpawnDefectCoroutine(hit));
        }
    }

    private IEnumerator SpawnDefectCoroutine(AssetController assetController)
    {
        // TODO call the api and make a defect
        yield return new WaitForSeconds(3f);
        _levelController.GameStore.AddDefect(new DefectModel(assetController.ItemId, Time.time.ToString(), "som signature"));

        // decide when next defect spawn will be
        _nextSpawn = Time.time + Random.Range(MinSpawnTime, MaxSpawnTime);
    }
}
