using System.Collections.Generic;
using UnityEngine;

public class AssetJobController : MonoBehaviour
{
    public bool Visible;
    public AssetController AssetController;
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
        if (AssetController.IsPolygon)
        {
            InitialisePolygon();
        }
        else if (AssetController.IsLineString)
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
        foreach (var asset in AssetController.Assets)
        {
            // get the renderer and a prefab to create an instance of
            var mr = asset.GetComponent<MeshRenderer>();
            var templatePrefab = GetRandomPrefab();

            // expand the bounds of the asset
            var expandedBounds = new Bounds(mr.bounds.center, mr.bounds.size);
            expandedBounds.Expand(2f);

            // add 1 around the asset
            var jobStuff = Instantiate(templatePrefab, gameObject.transform);

            // rotate anywhere
            jobStuff.transform.Rotate(0f, Random.Range(0f, 360f), 0f);

            // now move the asset away by the extent
            jobStuff.transform.Translate(expandedBounds.extents.x, JobPrefabOffsetY, 0f, Space.Self);

            _gameObjects.Add(jobStuff);
        }
    }

    private void InitialiseLineString()
    {
        if (AssetController.LineStringCoordinates == null)
        {
            Debug.Log("cannot initialise asset job controller, missing line string coordinates");
            return;
        }

        foreach (var line in AssetController.LineStringCoordinates)
        {
            foreach (var position in line)
            {
                var jobStuff = Instantiate(GetRandomPrefab(), gameObject.transform);
                jobStuff.transform.localPosition = new Vector3(position.x, JobPrefabOffsetY, position.z);
                jobStuff.transform.Rotate(0f, Random.Range(0f, 360f), 0f); // rotate anywhere
                _gameObjects.Add(jobStuff);
            }
        }
    }

    private void InitialisePolygon()
    {
        if (AssetController.PolygonsToTaskDefectAgainst == null)
        {
            Debug.Log("cannot initialise asset job controller, missing polygons to task/defect against");
            return;
        }

        foreach (var asset in AssetController.PolygonsToTaskDefectAgainst)
        {
            // get the renderer and a prefab to create an instance of
            var mr = asset.GetComponent<MeshRenderer>();
            var templatePrefab = GetRandomPrefab();

            // expand the bounds of the asset
            var expandedBounds = new Bounds(mr.bounds.center, mr.bounds.size);
            expandedBounds.Expand(2f);

            // add 1 around the asset
            var jobStuff = Instantiate(templatePrefab, gameObject.transform);

            var centre = new Vector3(mr.bounds.center.x, JobPrefabOffsetY, mr.bounds.center.z);
            jobStuff.transform.position = centre;
            jobStuff.transform.RotateAround(centre, new Vector3(0f, 1f, 0f), Random.Range(0f, 360f));
            jobStuff.transform.Translate(expandedBounds.extents.x, 0f, 0f);

            _gameObjects.Add(jobStuff);
        }
    }

    private GameObject GetRandomPrefab()
    {
        return Resources.Load(_jobPrefabs[Random.Range(0, _jobPrefabs.Length - 1)]) as GameObject;
    }
}