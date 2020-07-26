using System.Collections.Generic;
using UnityEngine;

public class AssetInspectionController : MonoBehaviour
{
    public bool Visible;
    public GameObject Asset;
    public bool IsLineString;
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
        var mr = Asset.GetComponent<MeshRenderer>();
        var min = mr.bounds.min - mr.bounds.center;
        var max = mr.bounds.max - mr.bounds.center;
        var templatePrefab = GetRandomPrefab();

        // add 4 around the asset
        var barrier1 = Instantiate(GetRandomPrefab(), gameObject.transform);
        var barrier2 = Instantiate(GetRandomPrefab(), gameObject.transform);
        var barrier3 = Instantiate(GetRandomPrefab(), gameObject.transform);
        var barrier4 = Instantiate(GetRandomPrefab(), gameObject.transform);
        barrier1.transform.localPosition = new Vector3(min.x, Mathf.Min(mr.bounds.max.y, mr.bounds.min.y), min.z);
        barrier2.transform.localPosition = new Vector3(min.x, Mathf.Min(mr.bounds.max.y, mr.bounds.min.y), max.z);
        barrier3.transform.localPosition = new Vector3(max.x, Mathf.Min(mr.bounds.max.y, mr.bounds.min.y), max.z);
        barrier4.transform.localPosition = new Vector3(max.x, Mathf.Min(mr.bounds.max.y, mr.bounds.min.y), min.z);
        _gameObjects.Add(barrier1);
        _gameObjects.Add(barrier2);
        _gameObjects.Add(barrier3);
        _gameObjects.Add(barrier4);
    }

    private void InitialiseLineString()
    {
        // get all the positions in the rendered line
        var renderer = Asset.GetComponent<LineRenderer>();
        var positions = new Vector3[renderer.positionCount];
        renderer.GetPositions(positions);
        
        foreach (var position in positions)
        {
            var barrier = Instantiate(GetRandomPrefab(), gameObject.transform);
            barrier.transform.localPosition = new Vector3(position.x, position.y, position.z);
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