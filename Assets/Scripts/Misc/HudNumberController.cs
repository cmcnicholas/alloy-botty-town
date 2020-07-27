using System.Collections.Generic;
using UnityEngine;

public class HudNumberController : MonoBehaviour
{
    public GameObject Number0;
    public GameObject Number1;
    public GameObject Number2;
    public GameObject Number3;
    public GameObject Number4;
    public GameObject Number5;
    public GameObject Number6;
    public GameObject Number7;
    public GameObject Number8;
    public GameObject Number9;
    public float ColourTilingOffsetY = 0.9f;
    public int Number;
    private IDictionary<int, GameObject> _numbers;
    private int? _lastNumber = null;
    private float? _lastColourTilingOffsetY = null;

    // Start is called before the first frame update
    void Start()
    {
        // enumerate
        _numbers = new Dictionary<int, GameObject>
        {
            { 0, Number0 },
            { 1, Number1 },
            { 2, Number2 },
            { 3, Number3 },
            { 4, Number4 },
            { 5, Number5 },
            { 6, Number6 },
            { 7, Number7 },
            { 8, Number8 },
            { 9, Number9 },
        };

        // disable all to start
        foreach (var gameObject in _numbers.Values)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // check if we need to update colour
        if (_lastColourTilingOffsetY != ColourTilingOffsetY)
        {
            foreach (var keyPair in _numbers)
            {
                keyPair.Value.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(1f, ColourTilingOffsetY));
            }

            _lastColourTilingOffsetY = ColourTilingOffsetY;
        }

        // check if we need to number
        if (_lastNumber != Number)
        {
            // update visible number
            foreach (var keyPair in _numbers)
            {
                keyPair.Value.SetActive(keyPair.Key == Number);
            }

            _lastNumber = Number;
        }
    }

}
