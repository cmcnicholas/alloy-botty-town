using Assets.Server;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.Extras;

[RequireComponent(typeof(SteamVR_LaserPointer))]
public class AssetMenuVrController : MonoBehaviour
{
    public GameObject InspectionsButton;
    public GameObject JobsButton;
    public GameObject DefectsButton;
    public GameObject CloseButton;
    public GameObject Label;
    public SteamVR_Input_Sources HandType;
    public SteamVR_Action_Boolean ClickAction;
    private SteamVR_LaserPointer laserPointer;
    private Text _text;

    private void OnEnable()
    {
        laserPointer = GetComponent<SteamVR_LaserPointer>();
        laserPointer.PointerIn -= HandlePointerIn;
        laserPointer.PointerIn += HandlePointerIn;
        laserPointer.PointerOut -= HandlePointerOut;
        laserPointer.PointerOut += HandlePointerOut;

        _text = Label.GetComponent<Text>();
    }

    public void Update()
    {
        if (ClickAction.GetState(HandType) && EventSystem.current.currentSelectedGameObject != null)
        {
            // stop fast clicking
            if (ApplicationGlobals.LastVrControllerPress == null || ApplicationGlobals.LastVrControllerPress + 0.5f < Time.time)
            {
                ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
                Debug.Log("Asset menu vr clicked");
                ApplicationGlobals.LastVrControllerPress = Time.time;
            }
        }
    }

    private void HandlePointerIn(object sender, PointerEventArgs e)
    {
        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            if (Object.ReferenceEquals(e.target.gameObject, CloseButton))
            {
                _text.text = "CLOSE MENU";
            }
            else if (Object.ReferenceEquals(e.target.gameObject, InspectionsButton))
            {
                _text.text = "INSPECT ASSET";
            }
            else if (Object.ReferenceEquals(e.target.gameObject, JobsButton))
            {
                _text.text = "COMPLETE JOB";
            }
            else if (Object.ReferenceEquals(e.target.gameObject, DefectsButton))
            {
                _text.text = "REPORT DEFECT";
            }

            button.Select();
        }
    }

    private void HandlePointerOut(object sender, PointerEventArgs e)
    {
        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            _text.text = string.Empty;
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
