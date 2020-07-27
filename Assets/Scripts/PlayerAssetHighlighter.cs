using cakeslice;
using System.Collections;
using UnityEngine;

public class PlayerAssetHighlighter : MonoBehaviour
{
    public GameObject MenuCanvas;
    public GameObject Stage;
    private AssetMenuController _assetMenuController;
    private Outline _lastHitOutline;

    // Start is called before the first frame update
    void Start()
    {
        // get the menu controller
        _assetMenuController = MenuCanvas.GetComponent<AssetMenuController>();

        // start checking for hits
        StartCoroutine(CheckHitCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (_lastHitOutline != null && Input.GetMouseButtonDown(0))
        {
            // weirdly hit appears on the child component but the outline is on the parent, whatever...
            var assetController = _lastHitOutline.transform.parent.GetComponent<AssetController>();
            _assetMenuController.OpenMenu(assetController.ItemId);
        }
    }

    private void OnEnable()
    {
        // weird bug where the outline script selects everything on screen during transition from 
        // main menu camera to player camera, this goes and nukes the enabled state on all outlines
        // so they don't get a chance to render :)
        foreach (var outline in Stage.GetComponentsInChildren<Outline>())
        {
            outline.enabled = false;
        }
    }

    IEnumerator CheckHitCoroutine()
    {
        while (true)
        {
            // remove hit
            if (_lastHitOutline != null)
            {
                _lastHitOutline.enabled = false;
                _lastHitOutline = null;
            }

            // find any new hit
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 5.0f))
            {
                if (hit.transform.TryGetComponent<Outline>(out Outline outlineScript))
                {
                    _lastHitOutline = outlineScript;
                    _lastHitOutline.enabled = true;
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}
