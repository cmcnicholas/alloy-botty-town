using System.Collections.Generic;
using UnityEngine;

public class AssetInspectionController : MonoBehaviour
{
    public bool Visible;
    public GameObject Asset;
    public bool IsLineString;
    public Vector3[] LineStringCoordinates;
    public bool IsPolygon;
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
        if (IsPolygon)
        {
            InitialisePolygon();
        }
        else if (IsLineString)
        {
            InitialiseLineString();
        }
        else
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
        // get the renderer and a prefab to create an instance of
        var mr = Asset.GetComponent<MeshRenderer>();
        var templatePrefab = GetRandomPrefab();

        // expand the bounds of the asset
        var expandedBounds = new Bounds(mr.bounds.center, mr.bounds.size);
        expandedBounds.Expand(3f);

        // get the expanded min/max
        var expandedMin = expandedBounds.min - expandedBounds.center;
        var expandedMax = expandedBounds.max - expandedBounds.center;

        // add 4 around the asset
        _gameObjects.Add(CreateBarrierFromTemplate(templatePrefab, new Vector3(expandedMin.x, Mathf.Min(mr.bounds.max.y, mr.bounds.min.y), expandedMin.z), -135f));
        _gameObjects.Add(CreateBarrierFromTemplate(templatePrefab, new Vector3(expandedMin.x, Mathf.Min(mr.bounds.max.y, mr.bounds.min.y), expandedMax.z), -45f));
        _gameObjects.Add(CreateBarrierFromTemplate(templatePrefab, new Vector3(expandedMax.x, Mathf.Min(mr.bounds.max.y, mr.bounds.min.y), expandedMax.z), 45f));
        _gameObjects.Add(CreateBarrierFromTemplate(templatePrefab, new Vector3(expandedMax.x, Mathf.Min(mr.bounds.max.y, mr.bounds.min.y), expandedMin.z), 135f));
    }

    private GameObject CreateBarrierFromTemplate(GameObject templatePrefab, Vector3 position, float roughRotate)
    {
        var barrier = Instantiate(templatePrefab, gameObject.transform);
        barrier.transform.localPosition = position;

        // rotate with some jitter to make it look more organic
        barrier.transform.Rotate(0f, Random.Range(roughRotate - 10f, roughRotate + 10f), 0f);
        return barrier;
    }

    private void InitialiseLineString()
    {
        if (LineStringCoordinates == null)
        {
            Debug.Log("cannot initialise asset inspection controller, missing line string coordinates");
            return;
        }
        
        foreach (var position in LineStringCoordinates)
        {
            var barrier = Instantiate(GetRandomPrefab(), gameObject.transform);
            barrier.transform.localPosition = new Vector3(position.x, position.y, position.z);
            barrier.transform.Rotate(0f, Random.Range(0f, 360f), 0f); // rotate anywhere
            _gameObjects.Add(barrier);
        }
    }

    private void InitialisePolygon()
    {
        // TODO
    }

    private GameObject GetRandomPrefab()
    {
        return Resources.Load(_inspectionPrefabs[Random.Range(0, _inspectionPrefabs.Length - 1)]) as GameObject;
    }
}