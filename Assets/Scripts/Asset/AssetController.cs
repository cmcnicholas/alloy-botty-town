using cakeslice;
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
    public Vector3[] LineStringCoordinates;
    public bool IsPolygon;
    public string ItemId;
    private AssetJobController _jobController;
    private AssetInspectionController _inspectionController;
    private AssetDefectController _defectController;
    private bool _job = false;
    private bool _inspect = false;
    private bool _defect = false;
    private bool _defectClear = true; // clear on start

    // Start is called before the first frame update
    void Start()
    {
        // add the outline component
        Asset.AddComponent<Outline>().enabled = false;

        // inspection controller, invisible by default
        _inspectionController = gameObject.AddComponent<AssetInspectionController>();
        _inspectionController.Asset = Asset;
        _inspectionController.IsPolygon = IsPolygon;
        _inspectionController.IsLineString = IsLineString;
        _inspectionController.LineStringCoordinates = LineStringCoordinates;
        _inspectionController.Visible = false; // non-visible assets to start
        _inspectionController.enabled = false; // also don't bother running

        // defect controller, invisible by default
        _defectController = gameObject.AddComponent<AssetDefectController>();
        _defectController.Asset = Asset;
        _defectController.IsPolygon = IsPolygon;
        _defectController.IsLineString = IsLineString;
        _defectController.LineStringCoordinates = LineStringCoordinates;
        _defectController.Visible = false; // non-visible assets to start
        _defectController.Clear = false;
        _defectController.enabled = false; // also don't bother running
        
        // job controller, invisible by default
        _jobController = gameObject.AddComponent<AssetJobController>();
        _jobController.Asset = Asset;
        _jobController.IsPolygon = IsPolygon;
        _jobController.IsLineString = IsLineString;
        _jobController.LineStringCoordinates = LineStringCoordinates;
        _jobController.Visible = false; // non-visible assets to start
        _jobController.enabled = false; // also don't bother running
    }

    // Update is called once per frame
    void Update()
    {
        // update the defect controller
        _jobController.Visible = _job;
        if (_job)
        {
            // first time we go visible turn on the controller
            _jobController.enabled = true;
        }

        // update the inspection controller
        _inspectionController.Visible = _inspect;
        if (_inspect)
        {
            // first time we go visible turn on the controller
            _inspectionController.enabled = true;
        }

        // update the defect controller
        _defectController.Visible = _defect;
        _defectController.Clear = _defectClear;
        if (_defect)
        {
            // first time we go visible turn on the controller
            _defectController.enabled = true;
        }
    }

    public void SetJob(bool value, bool clear)
    {
        // set variables, updated in event loop because we do initialisation using this function and there are no particle
        // systems created at this point in time (directly aftrer construction)
        _job = value;
    }

    public void SetInspect(bool value)
    {
        // set variables, updated in event loop because we do initialisation using Start()
        _inspect = value;
    }

    public void SetDefect(bool value, bool clear)
    {
        // set variables, updated in event loop because we do initialisation using Start()
        _defect = value;
        _defectClear = clear;
    }
}
