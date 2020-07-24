using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class AssetMenuController : MonoBehaviour
{
    public GameObject AssetMenu;
    public GameObject PlayerController;
    private FirstPersonController _firstPersonController;

    // Start is called before the first frame update
    void Start()
    {
        // turn off the menu to begin
        AssetMenu.SetActive(false);

        // get the first person controller
        _firstPersonController = PlayerController.GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenMenu()
    {
        _firstPersonController.SetLocked(true);
        AssetMenu.SetActive(true);
    }

    public void OnCompleteJobsPressed()
    {

    }

    public void OnCompleteInspectionsPressed()
    {

    }

    public void OnRegisterDefectPressed()
    {

    }

    public void OnClosePressed()
    {
        AssetMenu.SetActive(false);
        _firstPersonController.SetLocked(false);
    }
}
