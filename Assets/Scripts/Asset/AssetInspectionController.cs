using System.Collections.Generic;
using UnityEngine;

public class AssetInspectionController : MonoBehaviour
{
    public bool Visible;
    public AssetController AssetController;
    private bool? _lastVisible = null;
    private IList<GameObject> _gameObjects = new List<GameObject>();
    private string[] _inspectionPrefabs = new string[]
    {
        "Prefabs/Inspection/Barrier01",
        "Prefabs/Inspection/Barrier02",
        "Prefabs/Inspection/Barrier03",
        "Prefabs/Inspection/Barrier04",
        "Prefabs/Inspection/Barrier05"
    };

    // Start is called before the first frame update
    void Start()
    {
        if (AssetController.Polys != null)
        {
            InitialisePolygon();
        }
        if (AssetController.LineCoordinates != null)
        {
            InitialiseLineString();
        }
        if (AssetController.Points != null)
        {
            InitialisePoint();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_lastVisible != Visible)
        {
            foreach (var go in _gameObjects)
            {
                go.SetActive(Visible);
            }

            _lastVisible = Visible;
        }
    }

    private void InitialisePoint()
    {
        foreach (var asset in AssetController.Points)
        {
            // get the renderer and a prefab to create an instance of
            var mr = asset.GetComponent<MeshRenderer>();
            var templatePrefab = GetRandomPrefab();

            // expand the bounds of the asset
            var expandedBounds = new Bounds(mr.bounds.center, mr.bounds.size);
            expandedBounds.Expand(3f);

            // add 4 around the asset
            _gameObjects.Add(CreateBarrierFromTemplate(templatePrefab,
                new Vector3(expandedBounds.min.x, Mathf.Min(mr.bounds.max.y, mr.bounds.min.y), expandedBounds.min.z), -135f));
            _gameObjects.Add(CreateBarrierFromTemplate(templatePrefab,
                new Vector3(expandedBounds.min.x, Mathf.Min(mr.bounds.max.y, mr.bounds.min.y), expandedBounds.max.z), -45f));
            _gameObjects.Add(CreateBarrierFromTemplate(templatePrefab,
                new Vector3(expandedBounds.max.x, Mathf.Min(mr.bounds.max.y, mr.bounds.min.y), expandedBounds.max.z), 45f));
            _gameObjects.Add(CreateBarrierFromTemplate(templatePrefab,
                new Vector3(expandedBounds.max.x, Mathf.Min(mr.bounds.max.y, mr.bounds.min.y), expandedBounds.min.z), 135f));
        }
    }

    private void InitialiseLineString()
    {
        foreach (var line in AssetController.LineCoordinates)
        {
            foreach (var position in line)
            {
                var barrier = Instantiate(GetRandomPrefab(), gameObject.transform);
                barrier.transform.localPosition = new Vector3(position.x, position.y, position.z);
                barrier.transform.Rotate(0f, Random.Range(0f, 360f), 0f); // rotate anywhere
                _gameObjects.Add(barrier);
            }
        }
    }

    private void InitialisePolygon()
    {
        if (AssetController.Polys == null)
        {
            Debug.Log("cannot initialise asset inspection controller, missing polygons to task/defect against");
            return;
        }

        foreach (var asset in AssetController.Polys)
        {
            // get the renderer and a prefab to create an instance of
            var mr = asset.GetComponent<MeshRenderer>();
            var templatePrefab = GetRandomPrefab();

            // expand the bounds of the asset
            var expandedBounds = new Bounds(mr.bounds.center, mr.bounds.size);
            expandedBounds.Expand(3f);
            
            // add 4 around the asset
            _gameObjects.Add(CreateBarrierFromTemplate(templatePrefab, 
                new Vector3(expandedBounds.min.x, Mathf.Min(mr.bounds.max.y, mr.bounds.min.y), expandedBounds.min.z), -135f));
            _gameObjects.Add(CreateBarrierFromTemplate(templatePrefab, 
                new Vector3(expandedBounds.min.x, Mathf.Min(mr.bounds.max.y, mr.bounds.min.y), expandedBounds.max.z), -45f));
            _gameObjects.Add(CreateBarrierFromTemplate(templatePrefab, 
                new Vector3(expandedBounds.max.x, Mathf.Min(mr.bounds.max.y, mr.bounds.min.y), expandedBounds.max.z), 45f));
            _gameObjects.Add(CreateBarrierFromTemplate(templatePrefab, 
                new Vector3(expandedBounds.max.x, Mathf.Min(mr.bounds.max.y, mr.bounds.min.y), expandedBounds.min.z), 135f));
        }
    }

    private GameObject CreateBarrierFromTemplate(GameObject templatePrefab, Vector3 position, float roughRotate)
    {
        var barrier = Instantiate(templatePrefab, gameObject.transform);
        barrier.transform.position = position;

        // rotate with some jitter to make it look more organic
        barrier.transform.RotateAround(position, new Vector3(0f, 1f, 0f), Random.Range(roughRotate - 10f, roughRotate + 10f));
        return barrier;
    }

    private GameObject GetRandomPrefab()
    {
        return Resources.Load(_inspectionPrefabs[Random.Range(0, _inspectionPrefabs.Length - 1)]) as GameObject;
    }
}