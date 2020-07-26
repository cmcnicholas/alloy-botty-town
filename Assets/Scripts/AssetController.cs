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
    private IList<GameObject> _fires;
    private IList<ParticleSystem> _fireParticleSystems;
    private bool _lastFire = false;
    private bool _lastFireClear = false;
    private bool _fire = false;
    private bool _fireClear = true; // clear on start
    private bool _lastInspect = false;
    private bool _lastInspectClear = false;
    private bool _inspect = false;
    private bool _inspectClear = true; // clear on start

    private const string FirePrefab = "Prefabs/Fire";

    // Start is called before the first frame update
    void Start()
    {
        // add the outline component
        Asset.AddComponent<Outline>().enabled = false;

        if (IsPolygon)
        {
            InitialiseFirePolygon();
        }
        else if (IsLineString)
        {
            InitialiseFireLineString();
        }
        else
        {
            InitialiseFirePoint();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // update the fire particle system
        if (_lastFire != _fire || _lastFireClear != _fireClear)
        {
            if (_fire)
            {
                foreach (var particleSystem in _fireParticleSystems)
                {
                    particleSystem.Play(true);
                }
            }
            else
            {
                foreach (var particleSystem in _fireParticleSystems)
                {
                    particleSystem.Stop(true, _fireClear ? ParticleSystemStopBehavior.StopEmittingAndClear : ParticleSystemStopBehavior.StopEmitting);
                }
            }

            _lastFire = _fire;
            _lastFireClear = _fireClear;
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

    public void SetFire(bool value, bool clear)
    {
        // set variables, updated in event loop because we do initialisation using this function and there are no particle
        // systems created at this point in time (directly aftrer construction)
        _fire = value;
        _fireClear = clear;
    }

    public void SetInspect(bool value, bool clear)
    {
        // set variables, updated in event loop because we do initialisation using this function and there are no particle
        // systems created at this point in time (directly aftrer construction)
        _inspect = value;
        _inspectClear = clear;
    }

    private void InitialiseFirePoint()
    {
        // make a fire object on the game object
        _fires = new List<GameObject>
        {
            Instantiate(Resources.Load(FirePrefab) as GameObject, gameObject.transform)
        };
        _fireParticleSystems = new List<ParticleSystem>
        {
            _fires[0].GetComponent<ParticleSystem>(),
        };

        // make the fire bigger
        var mr = Asset.GetComponent<MeshRenderer>();
        var particleSystemShape = _fireParticleSystems[0].shape;
        particleSystemShape.scale = new Vector3(mr.bounds.size.x, mr.bounds.size.y, mr.bounds.size.z);

        // put the fire on top of the thing
        _fires[0].transform.localPosition = new Vector3(0.0f, mr.bounds.size.y, 0.0f);
    }

    private void InitialiseFireLineString()
    {
        // get all the positions in the rendered line
        var renderer = Asset.GetComponent<LineRenderer>();
        var positions = new Vector3[renderer.positionCount];
        renderer.GetPositions(positions);

        _fires = new List<GameObject>();
        _fireParticleSystems = new List<ParticleSystem>();
        foreach (var position in positions)
        {
            var fire = Instantiate(Resources.Load(FirePrefab) as GameObject, gameObject.transform);
            var particleSystem = fire.GetComponent<ParticleSystem>();
            
            fire.transform.localPosition = position;

            _fires.Add(fire);
            _fireParticleSystems.Add(particleSystem);
        }
    }

    private void InitialiseFirePolygon()
    {
        _fires = new List<GameObject>();
        _fireParticleSystems = new List<ParticleSystem>();

        // TODO
    }
}
