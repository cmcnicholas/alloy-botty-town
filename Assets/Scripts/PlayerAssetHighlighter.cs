using cakeslice;
using System.Collections;
using UnityEngine;

public class PlayerAssetHighlighter : MonoBehaviour
{
    public GameObject MenuCanvas;
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
            _assetMenuController.OpenMenu();
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
