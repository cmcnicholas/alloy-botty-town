using System.Collections;
using UnityEngine;

public class PlayerAssetHighlighter : MonoBehaviour
{
    public GameObject MenuCanvas;
    public GameObject Stage;
    public GameObject PlayerCamera;
    private AssetMenuController _assetMenuController;
    private Camera _playerCamera;
    private AssetController _lastHitAssetController;
    private float _lastClick;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        _playerCamera = PlayerCamera.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_lastHitAssetController != null && Input.GetMouseButtonDown(0) && _lastClick + 0.5f < Time.time)
        {
            // debounce
            _lastClick = Time.time;
            _assetMenuController.OpenMenu(_lastHitAssetController.ItemId);
        }
    }

    // Called each time game object is enabled
    void OnEnable()
    {
        // get the menu controller
        _assetMenuController = MenuCanvas.GetComponent<AssetMenuController>();

        // weird bug where the outline script selects everything on screen during transition from 
        // main menu camera to player camera, this goes and nukes the enabled state on all outlines
        // so they don't get a chance to render :)
        foreach (var assetController in Stage.GetComponentsInChildren<AssetController>())
        {
            assetController.SetOutline(false);
        }

        // start checking for hits, we HAVE to restart this each time enabled changes
        // unity will stop the coroutines for a disabled game object
        StartCoroutine(CheckHitCoroutine());
    }

    IEnumerator CheckHitCoroutine()
    {
        while (true)
        {
            // remove hit
            if (_lastHitAssetController != null)
            {
                _lastHitAssetController.SetOutline(false);
                _lastHitAssetController = null;
            }

            // find any new hit
            if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit hit, 5f))
            {
                if (hit.transform.parent != null)
                {
                    if (hit.transform.parent.TryGetComponent<AssetController>(out AssetController assetController))
                    {
                        _lastHitAssetController = assetController;
                        _lastHitAssetController.SetOutline(true);
                    }
                    else if (hit.transform.parent.parent != null && hit.transform.parent.parent.TryGetComponent<AssetController>(out AssetController parentAssetController))
                    {
                        _lastHitAssetController = parentAssetController;
                        _lastHitAssetController.SetOutline(true);
                    }
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}
