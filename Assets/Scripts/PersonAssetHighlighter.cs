using cakeslice;
using System.Collections;
using UnityEngine;

public class PersonAssetHighlighter : MonoBehaviour
{
    private Outline _lastHitOutline;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckHitCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
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
