using cakeslice;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PersonAssetHighlighter : MonoBehaviour
{
    public GameObject AssetRadialMenu;
    private Outline _lastHitOutline;
    private FirstPersonController _firstPersonController;

    // Start is called before the first frame update
    void Start()
    {
        // turn off the menu to begin
        AssetRadialMenu.SetActive(false);

        // get the first person controller
        _firstPersonController = gameObject.GetComponent<FirstPersonController>();

        // start checking for hits
        StartCoroutine(CheckHitCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (_lastHitOutline != null && Input.GetMouseButtonDown(0))
        {
            AssetRadialMenu.SetActive(true);
            _firstPersonController.SetLocked(true);
        }
        if (Input.GetMouseButtonDown(1))
        {
            AssetRadialMenu.SetActive(false);
            _firstPersonController.SetLocked(false);
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
