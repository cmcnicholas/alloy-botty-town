using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// controller for the hud counters, only handles positive numbers
/// </summary>
public class HudCounterController : MonoBehaviour
{
    public GameObject CupIcon;
    public GameObject TimerIcon;
    public GameObject HudNumberPos1;
    public GameObject HudNumberPos2;
    public GameObject HudNumberPos3;
    public GameObject HudNumberPos4;
    public GameObject HudNumberPos5;
    public GameObject HudNumberPos6;
    public GameObject HudNumberPos7;
    public GameObject HudNumberPos8;
    public float ColourTilingOffsetY = 0.9f;
    public float PunchScale;
    public int Number;
    private IList<GameObject> _numberGameObjects;
    private IList<HudNumberController> _numberGameControllers;
    private int? _lastNumber = null;
    private float? _lastColourTilingOffsetY = null;
    private bool _firstUpdate = true;

    // Start is called before the first frame update
    void Start()
    {
        // populate the game objects
        _numberGameObjects = new List<GameObject>
        {
            HudNumberPos1,
            HudNumberPos2,
            HudNumberPos3,
            HudNumberPos4,
            HudNumberPos5,
            HudNumberPos6,
            HudNumberPos7,
            HudNumberPos8,
        };

        // populate the controllers
        _numberGameControllers = new List<HudNumberController>();
        foreach (var go in _numberGameObjects)
        {
            var numberController = go.GetComponent<HudNumberController>();
            _numberGameControllers.Add(numberController);

            // also turn off all the positions
            go.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // update colours if we have any
        if (_lastColourTilingOffsetY != ColourTilingOffsetY)
        {
            CupIcon.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(1f, ColourTilingOffsetY));
            TimerIcon.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(1f, ColourTilingOffsetY));
            foreach (var controller in _numberGameControllers)
            {
                controller.ColourTilingOffsetY = ColourTilingOffsetY;
            }
            _lastColourTilingOffsetY = ColourTilingOffsetY;
        }

        // check if we have changed numbers
        if (_lastNumber != Number)
        {
            // if the value is negative, turn it all off
            if (Number < 0)
            {
                foreach (var go in _numberGameObjects)
                {
                    go.SetActive(false);
                }
            }
            else
            {
                // clean the number incase too big
                string stringified = Number.ToString();
                stringified = stringified.Substring(Math.Max(stringified.Length - _numberGameObjects.Count, 0), Math.Min(_numberGameObjects.Count, Number.ToString().Length));
                int stringifiedLength = stringified.Length;

                // iterate through and update
                for (int i = 0, s = _numberGameObjects.Count; i < s; i++)
                {
                    if (i < stringifiedLength)
                    {
                        // show the number position and update number value
                        _numberGameObjects[i].SetActive(true);
                        _numberGameControllers[i].Number = int.Parse(stringified.Substring(i, 1));
                    }
                    else
                    {
                        // hide the number position
                        _numberGameObjects[i].SetActive(false);
                    }
                }

                // punch the UI to make them pop
                if (!_firstUpdate)
                {
                    iTween.PunchScale(gameObject, new Vector3(PunchScale, PunchScale, PunchScale), 0.9f);
                }
                _firstUpdate = false;
            }

            // update the last number
            _lastNumber = Number;
        }
    }
}
