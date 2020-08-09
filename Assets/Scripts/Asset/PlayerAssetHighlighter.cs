using Assets.Server;
using System.Collections;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;

public class PlayerAssetHighlighter : MonoBehaviour
{
    public GameObject MenuCanvas;
    public GameObject Stage;
    public GameObject LeftController;
    public GameObject RightController;
    public SteamVR_Action_Boolean ClickAction;
    private SteamVR_LaserPointer leftLaserPointer;
    private SteamVR_LaserPointer rightLaserPointer;
    private AssetMenuController _assetMenuController;
    private AssetController _lastLeftHitAssetController;
    private AssetController _lastRightHitAssetController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        leftLaserPointer = LeftController.GetComponent<SteamVR_LaserPointer>();
        rightLaserPointer = RightController.GetComponent<SteamVR_LaserPointer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_lastRightHitAssetController != null)
        {
            if (ClickAction.GetState(SteamVR_Input_Sources.RightHand))
            {
                if ((ApplicationGlobals.LastVrControllerPress == null || ApplicationGlobals.LastVrControllerPress + 0.5f < Time.time))
                {
                    Debug.Log("GOT HERE BOYZ");
                }
            }
        }

        if (_lastLeftHitAssetController != null && ClickAction.GetState(SteamVR_Input_Sources.LeftHand) && 
            (ApplicationGlobals.LastVrControllerPress == null || ApplicationGlobals.LastVrControllerPress + 0.5f < Time.time))
        {
            // debounce
            Debug.Log("Asset highlighter, asset clicked");
            ApplicationGlobals.LastVrControllerPress = Time.time;
            _assetMenuController.OpenMenu(_lastLeftHitAssetController.ItemId);
        }
        else if (_lastRightHitAssetController != null && ClickAction.GetState(SteamVR_Input_Sources.RightHand) && 
            (ApplicationGlobals.LastVrControllerPress == null || ApplicationGlobals.LastVrControllerPress + 0.5f < Time.time))
        {
            // debounce
            Debug.Log("Asset highlighter, asset clicked");
            ApplicationGlobals.LastVrControllerPress = Time.time;
            _assetMenuController.OpenMenu(_lastRightHitAssetController.ItemId);
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
            if (_lastLeftHitAssetController != null)
            {
                _lastLeftHitAssetController.SetOutline(false);
                _lastLeftHitAssetController = null;
            }

            // find any new hit
            if (Physics.Raycast(leftLaserPointer.transform.position, leftLaserPointer.transform.forward, out RaycastHit hitLeft, 5f))
            {
                if (hitLeft.transform.parent != null)
                {
                    if (hitLeft.transform.parent.TryGetComponent<AssetController>(out AssetController assetController))
                    {
                        _lastLeftHitAssetController = assetController;
                        _lastLeftHitAssetController.SetOutline(true);
                    }
                    else if (hitLeft.transform.parent.parent != null && hitLeft.transform.parent.parent.TryGetComponent<AssetController>(out AssetController parentAssetController))
                    {
                        _lastLeftHitAssetController = parentAssetController;
                        _lastLeftHitAssetController.SetOutline(true);
                    }
                }
            }

            // remove hit
            if (_lastRightHitAssetController != null)
            {
                _lastRightHitAssetController.SetOutline(false);
                _lastRightHitAssetController = null;
            }

            // find any new hit
            if (Physics.Raycast(rightLaserPointer.transform.position, rightLaserPointer.transform.forward, out RaycastHit hitRight, 5f))
            {
                if (hitRight.transform.parent != null)
                {
                    if (hitRight.transform.parent.TryGetComponent<AssetController>(out AssetController assetController))
                    {
                        _lastRightHitAssetController = assetController;
                        _lastRightHitAssetController.SetOutline(true);
                    }
                    else if (hitRight.transform.parent.parent != null && hitRight.transform.parent.parent.TryGetComponent<AssetController>(out AssetController parentAssetController))
                    {
                        _lastRightHitAssetController = parentAssetController;
                        _lastRightHitAssetController.SetOutline(true);
                    }
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}
