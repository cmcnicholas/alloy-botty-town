using UnityEngine;
using Valve.VR.Extras;
using Valve.VR;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(SteamVR_LaserPointer))]
public class MainMenuVrController : MonoBehaviour
{
    public SteamVR_Input_Sources HandType;
    public SteamVR_Action_Boolean ClickAction;
    private SteamVR_LaserPointer laserPointer;
    private float? lastClick = null;

    private void OnEnable()
    {
        laserPointer = GetComponent<SteamVR_LaserPointer>();
        laserPointer.PointerIn -= HandlePointerIn;
        laserPointer.PointerIn += HandlePointerIn;
        laserPointer.PointerOut -= HandlePointerOut;
        laserPointer.PointerOut += HandlePointerOut;
    }

    public void Update()
    {
        if (ClickAction.GetState(HandType) && EventSystem.current.currentSelectedGameObject != null)
        {
            // stop fast clicking
            if (lastClick == null || lastClick + 0.5f < Time.time)
            {
                ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
                lastClick = Time.time;
            }
        }
    }

    private void HandlePointerIn(object sender, PointerEventArgs e)
    {
        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            button.Select();
        }
        var input = e.target.GetComponent<InputField>();
        if (input != null)
        {
            input.Select();
        }
    }

    private void HandlePointerOut(object sender, PointerEventArgs e)
    {
        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
        var input = e.target.GetComponent<InputField>();
        if (input != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
