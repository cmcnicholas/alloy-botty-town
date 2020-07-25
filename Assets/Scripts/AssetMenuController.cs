using System.Collections;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class AssetMenuController : MonoBehaviour
{
    public GameObject AssetMenu;
    public GameObject PlayerController;
    public GameObject AlloyMobile;
    private FirstPersonController _firstPersonController;
    private AlloyMobileController _alloyMobileController;
    private string _itemId;

    // Start is called before the first frame update
    void Start()
    {
        // turn off the menu to begin
        AssetMenu.SetActive(false);

        // get the first person controller
        _firstPersonController = PlayerController.GetComponent<FirstPersonController>();

        // get the alloy mobile controller
        _alloyMobileController = AlloyMobile.GetComponent<AlloyMobileController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenMenu(string itemId)
    {
        _itemId = itemId;
        _firstPersonController.SetLocked(true);
        AssetMenu.SetActive(true);
    }

    public void OnCompleteJobsPressed()
    {
        AssetMenu.SetActive(false);
        StartCoroutine(CompleteJobsForAsset());
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

    private IEnumerator CompleteJobsForAsset()
    {
        // display mobile
        _alloyMobileController.ShowMobile();

        // do web request
        yield return new WaitForSeconds(3.0f);

        // unlock the person
        _firstPersonController.SetLocked(false);
        _alloyMobileController.HideMobile();
        yield return null;
    }
}
