﻿using System.Collections.Generic;
using UnityEngine;

public class AssetDefectController : MonoBehaviour
{
    public bool Visible;
    public bool Clear;
    public GameObject Asset;
    public bool IsLineString;
    public Vector3[] LineStringCoordinates;
    public bool IsPolygon;
    private bool? _lastVisible = null;
    private bool? _lastClear = null;
    private IList<ParticleSystem> _particleSystems = new List<ParticleSystem>();
    private string[] _defectPrefabs = new string[] {
        "Prefabs/Defect/Fire",
        "Prefabs/Defect/Smoke",
        "Prefabs/Defect/Steam"
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
        if (_lastVisible != Visible || _lastClear != Clear)
        {
            if (Visible)
            {
                foreach (var particleSystem in _particleSystems)
                {
                    particleSystem.Play(true);
                }
            }
            else
            {
                foreach (var particleSystem in _particleSystems)
                {
                    particleSystem.Stop(true, Clear ? ParticleSystemStopBehavior.StopEmittingAndClear : ParticleSystemStopBehavior.StopEmitting);
                }
            }

            _lastVisible = Visible;
            _lastClear = Clear;
        }
    }

    private void InitialisePoint()
    {
        // make an fx object on the game object template
        var go = Instantiate(GetRandomPrefab(), gameObject.transform);
        var particleSystem = go.GetComponent<ParticleSystem>();

        _particleSystems.Add(particleSystem);

        // make the fx bigger
        var mr = Asset.GetComponent<MeshRenderer>();
        var particleSystemShape = particleSystem.shape;
        particleSystemShape.scale = new Vector3(mr.bounds.size.x, mr.bounds.size.y, mr.bounds.size.z);

        // put the fx on top of the thing
        go.transform.localPosition = new Vector3(0.0f, mr.bounds.size.y, 0.0f);
    }

    private void InitialiseLineString()
    {
        if (LineStringCoordinates == null)
        {
            Debug.Log("cannot initialise asset defect controller, missing line string coordinates");
            return;
        }

        // get all the positions in the rendered line        
        foreach (var position in LineStringCoordinates)
        {
            var fx = Instantiate(GetRandomPrefab(), gameObject.transform);
            var particleSystem = fx.GetComponent<ParticleSystem>();

            fx.transform.localPosition = position;
            
            _particleSystems.Add(particleSystem);
        }
    }

    private void InitialisePolygon()
    {
        // TODO
    }

    private GameObject GetRandomPrefab()
    {
        return Resources.Load(_defectPrefabs[Random.Range(0, _defectPrefabs.Length - 1)]) as GameObject;
    }
}