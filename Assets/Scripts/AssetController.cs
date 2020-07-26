using cakeslice;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this should be added to all "asset" game objects so they can be managed,
/// the structure of the asset object should be:
/// GameObject (where the controller lives)
/// -- Asset (the game object that can be scaled/positioned relatively)
/// -- [Space to add effects e.g. Fire]
/// </summary>
public class AssetController : MonoBehaviour
{
    public GameObject Asset;
    public bool IsLineString;
    public bool IsPolygon;
    public string ItemId;
    private IList<GameObject> _jobFxGameObjects;
    private IList<ParticleSystem> _jobFxParticleSystems;
    private bool _lastJob = false;
    private bool _lastJobClear = false;
    private bool _job = false;
    private bool _jobClear = true; // clear on start
    private bool _lastInspect = false;
    private bool _lastInspectClear = false;
    private bool _inspect = false;
    private bool _inspectClear = true; // clear on start

    private string[] JobEffectPrefabs = new string[] { "Prefabs/FX/Fire", "Prefabs/FX/Smoke", "Prefabs/FX/Steam" };

    // Start is called before the first frame update
    void Start()
    {
        // add the outline component
        Asset.AddComponent<Outline>().enabled = false;

        if (IsPolygon)
        {
            InitialiseJobPolygon();
        }
        else if (IsLineString)
        {
            InitialiseJobLineString();
        }
        else
        {
            InitialiseJobPoint();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // update the job particle system
        if (_lastJob != _job || _lastJobClear != _jobClear)
        {
            if (_job)
            {
                foreach (var particleSystem in _jobFxParticleSystems)
                {
                    particleSystem.Play(true);
                }
            }
            else
            {
                foreach (var particleSystem in _jobFxParticleSystems)
                {
                    particleSystem.Stop(true, _jobClear ? ParticleSystemStopBehavior.StopEmittingAndClear : ParticleSystemStopBehavior.StopEmitting);
                }
            }

            _lastJob = _job;
            _lastJobClear = _jobClear;
        }

        // update the inspect particle system
        if (_lastInspect != _inspect || _lastInspectClear != _inspectClear)
        {
            if (_inspect)
            {
                // add effect
            }
            else
            {
                // remove effect
            }

            _lastInspect = _inspect;
            _lastInspectClear = _inspectClear;
        }
    }

    public void SetJob(bool value, bool clear)
    {
        // set variables, updated in event loop because we do initialisation using this function and there are no particle
        // systems created at this point in time (directly aftrer construction)
        _job = value;
        _jobClear = clear;
    }

    public void SetInspect(bool value, bool clear)
    {
        // set variables, updated in event loop because we do initialisation using this function and there are no particle
        // systems created at this point in time (directly aftrer construction)
        _inspect = value;
        _inspectClear = clear;
    }

    private void InitialiseJobPoint()
    {
        // make an fx object on the game object
        _jobFxGameObjects = new List<GameObject>
        {
            Instantiate(GetRandomJobEffectPrefab(), gameObject.transform)
        };
        _jobFxParticleSystems = new List<ParticleSystem>
        {
            _jobFxGameObjects[0].GetComponent<ParticleSystem>(),
        };

        // make the fx bigger
        var mr = Asset.GetComponent<MeshRenderer>();
        var particleSystemShape = _jobFxParticleSystems[0].shape;
        particleSystemShape.scale = new Vector3(mr.bounds.size.x, mr.bounds.size.y, mr.bounds.size.z);

        // put the fx on top of the thing
        _jobFxGameObjects[0].transform.localPosition = new Vector3(0.0f, mr.bounds.size.y, 0.0f);
    }

    private void InitialiseJobLineString()
    {
        // get all the positions in the rendered line
        var renderer = Asset.GetComponent<LineRenderer>();
        var positions = new Vector3[renderer.positionCount];
        renderer.GetPositions(positions);

        _jobFxGameObjects = new List<GameObject>();
        _jobFxParticleSystems = new List<ParticleSystem>();
        foreach (var position in positions)
        {
            var fx = Instantiate(GetRandomJobEffectPrefab(), gameObject.transform);
            var particleSystem = fx.GetComponent<ParticleSystem>();
            
            fx.transform.localPosition = position;

            _jobFxGameObjects.Add(fx);
            _jobFxParticleSystems.Add(particleSystem);
        }
    }

    private void InitialiseJobPolygon()
    {
        _jobFxGameObjects = new List<GameObject>();
        _jobFxParticleSystems = new List<ParticleSystem>();

        // TODO
    }

    private GameObject GetRandomJobEffectPrefab()
    {
        return Resources.Load(JobEffectPrefabs[Random.Range(0, JobEffectPrefabs.Length - 1)]) as GameObject;
    }
}
