using System.Collections.Generic;
using UnityEngine;

public class AssetJobController : MonoBehaviour
{
    public bool Visible;
    public GameObject Asset;
    public bool IsLineString;
    public bool IsPolygon;
    private bool? _lastVisible = null;
    private IList<GameObject> _gameObjects = new List<GameObject>();
    private string[] _jobPrefabs = new string[]
    {
        "Prefabs/Job/JobBundle1",
        "Prefabs/Job/JobBundle2",
        "Prefabs/Job/JobBundle3",
        "Prefabs/Job/JobBundle4",
        "Prefabs/Job/JobBundle5"
    };
    private const float JobPrefabOffsetY = 0.3f; // prefabs are not registered against 0 correctly

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
        expandedBounds.Expand(2f);

        // get the expanded min/max
        var expandedMin = expandedBounds.min - expandedBounds.center;
        var expandedMax = expandedBounds.max - expandedBounds.center;

        // add 1 around the asset
        var jobStuff = Instantiate(templatePrefab, gameObject.transform);

        // rotate anywhere
        jobStuff.transform.Rotate(0f, Random.Range(0f, 360f), 0f);

        // now move the asset away by the extent
        jobStuff.transform.Translate(expandedBounds.extents.x, JobPrefabOffsetY, 0f, Space.Self);
        
        _gameObjects.Add(jobStuff);
    }

    private void InitialiseLineString()
    {
        // get all the positions in the rendered line
        var renderer = Asset.GetComponent<LineRenderer>();
        var positions = new Vector3[renderer.positionCount];
        renderer.GetPositions(positions);

        foreach (var position in positions)
        {
            var jobStuff = Instantiate(GetRandomPrefab(), gameObject.transform);
            jobStuff.transform.localPosition = new Vector3(position.x, JobPrefabOffsetY, position.z);
            jobStuff.transform.Rotate(0f, Random.Range(0f, 360f), 0f); // rotate anywhere
            _gameObjects.Add(jobStuff);
        }
    }

    private void InitialisePolygon()
    {
        // TODO
    }

    private GameObject GetRandomPrefab()
    {
        return Resources.Load(_jobPrefabs[Random.Range(0, _jobPrefabs.Length - 1)]) as GameObject;
    }
}