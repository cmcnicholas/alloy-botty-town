using Assets.Server;
using UnityEngine;

public class DayNightManager : MonoBehaviour
{
    public GameObject SkyDome;
    public GameObject Sun;
    public GameObject Moon;
    public GameObject Stars;
    public GameObject Light;

    private Renderer _skyDomeRenderer;
    private float _skyOffsetX;
    private MeshRenderer _starsRenderer;
    private float _daysPassed = 0.5f; // starts at mid day

    // Start is called before the first frame update
    void Start()
    {
        // get renderer and initial x position
        _skyDomeRenderer = SkyDome.GetComponent<Renderer>();
        _skyOffsetX = _skyDomeRenderer.material.GetTextureOffset("_MainTex").x;

        // star renderer
        _starsRenderer = Stars.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // difference normalised 0-1 from last frame
        float difference = Time.deltaTime / ApplicationGlobals.DayNightCycleSeconds;

        // the skybox we use has offset x between 0-1 which is nice, we just increment by delta each frame
        _skyOffsetX = _skyOffsetX + difference;
        
        // rotate back round if we exceed 1
        if (_skyOffsetX > 1.0f)
        {
            _skyOffsetX = 1.0f - _skyOffsetX;
        }

        // rotate the texture map (the night/day texture)
        _skyDomeRenderer.material.SetTextureOffset("_MainTex", new Vector2(_skyOffsetX, 0.0f));

        // rotate the sun and the moon
        float orbitRotation = 360.0f * difference;
        Moon.transform.Rotate(new Vector3(orbitRotation, 0.0f, 0.0f));
        Sun.transform.Rotate(new Vector3(orbitRotation, 0.0f, 0.0f));
        
        // manage when the stars come out
        _daysPassed += Time.deltaTime / ApplicationGlobals.DayNightCycleSeconds;
        float timeOfDay = _daysPassed - Mathf.Floor(_daysPassed);
        bool isDay = timeOfDay >= 0.25 && timeOfDay < 0.75;
        _starsRenderer.enabled = !isDay;
    }
}
