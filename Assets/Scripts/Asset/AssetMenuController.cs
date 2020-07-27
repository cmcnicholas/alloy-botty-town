using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class AssetMenuController : MonoBehaviour
{
    public GameObject AssetMenu;
    public GameObject CloseJobsButton;
    public GameObject CloseInspectionsButton;
    public GameObject RegisterDefectButton;
    public GameObject PlayerController;
    public GameObject AlloyMobile;
    private FirstPersonController _firstPersonController;
    private AlloyMobileController _alloyMobileController;
    private LevelController _levelController;
    private LevelSoundEffectsController _levelSoundEffectsController;
    private string _assetItemId;
    private bool _working = false;

    // Start is called before the first frame update
    void Start()
    {
        // turn off the menu to begin
        AssetMenu.SetActive(false);

        // get the first person controller
        _firstPersonController = PlayerController.GetComponent<FirstPersonController>();

        // get the level controller
        _levelController = PlayerController.GetComponent<LevelController>();
        _levelSoundEffectsController = PlayerController.GetComponent<LevelSoundEffectsController>();

        // get the alloy mobile controller
        _alloyMobileController = AlloyMobile.GetComponent<AlloyMobileController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenMenu(string assetItemId)
    {
        // check if no-op
        if (_working || _assetItemId == assetItemId)
        {
            return;
        }

        // check we have an asset
        var asset = _levelController.GameStore.GetAsset(assetItemId);
        if (asset == null)
        {
            return;
        }

        // set state and update menu visually
        _assetItemId = assetItemId;
        _firstPersonController.SetLocked(true);
        AssetMenu.SetActive(true);

        // work out which menu options are available
        bool hasJobs = asset.Jobs.Count > 0;
        bool hasInspections = asset.Inspections.Count > 0;
        bool hasDefects = asset.Defects.Count > 0;

        var jobsButtonComponent = CloseJobsButton.GetComponent<Button>();
        jobsButtonComponent.interactable = hasJobs;
        var inspectionsButtonComponent = CloseInspectionsButton.GetComponent<Button>();
        inspectionsButtonComponent.interactable = hasInspections;
        var registerDefectButtonComponent = RegisterDefectButton.GetComponent<Button>();
        registerDefectButtonComponent.interactable = hasDefects;

        // open menu sound
        _levelSoundEffectsController.PlayMenuOpen();
    }

    public void OnCompleteJobsPressed()
    {
        _working = true;
        AssetMenu.SetActive(false);
        StartCoroutine(CompleteJobsForAsset());

        // select menu sound
        _levelSoundEffectsController.PlayMenuSelect();
    }

    public void OnCompleteInspectionsPressed()
    {

        _working = true;
        AssetMenu.SetActive(false);
        StartCoroutine(CompleteInspectionsForAsset());

        // select menu sound
        _levelSoundEffectsController.PlayMenuSelect();
    }

    public void OnRegisterDefectPressed()
    {
        _working = true;
        AssetMenu.SetActive(false);
        StartCoroutine(RegisterDefectForAsset());

        // select menu sound
        _levelSoundEffectsController.PlayMenuSelect();
    }

    public void OnClosePressed()
    {
        _assetItemId = null;
        AssetMenu.SetActive(false);
        _firstPersonController.SetLocked(false);

        // select menu close
        _levelSoundEffectsController.PlayMenuClosed();
    }

    private IEnumerator CompleteJobsForAsset()
    {
        // get the asset
        var asset = _levelController.GameStore.GetAsset(_assetItemId);

        // get all job ids on asset
        var jobs = asset.Jobs.ToList();

        // display mobile
        _alloyMobileController.ShowMobile();

        // do web request
        yield return new WaitForSeconds(jobs.Count * 3.0f);

        // remove all jobs from the asset
        foreach (var pair in jobs)
        {
            _levelController.GameStore.RemoveJob(pair.Value.ItemId);

            // add to the score
            _levelController.ScoreJob(pair.Value);
        }

        // unlock the person
        _firstPersonController.SetLocked(false);
        _alloyMobileController.HideMobile();

        _working = false;
        _assetItemId = null;
        yield return null;

        // success sound!
        _levelSoundEffectsController.PlaySuccess();
    }

    private IEnumerator CompleteInspectionsForAsset()
    {
        // get the asset
        var asset = _levelController.GameStore.GetAsset(_assetItemId);

        // get all inspection ids on asset
        var inspections = asset.Inspections.ToList();

        // display mobile
        _alloyMobileController.ShowMobile();

        // do web request
        yield return new WaitForSeconds(inspections.Count * 3.0f);

        // remove all inspections from the asset
        foreach (var pair in inspections)
        {
            _levelController.GameStore.RemoveInspection(pair.Value.ItemId);

            // add to the score
            _levelController.ScoreInspection(pair.Value);
        }

        // unlock the person
        _firstPersonController.SetLocked(false);
        _alloyMobileController.HideMobile();

        _working = false;
        _assetItemId = null;
        yield return null;

        // success sound!
        _levelSoundEffectsController.PlaySuccess();
    }

    private IEnumerator RegisterDefectForAsset()
    {
        // get the asset
        var asset = _levelController.GameStore.GetAsset(_assetItemId);

        // get all defect ids on asset
        var defects = asset.Defects.ToList();

        // display mobile
        _alloyMobileController.ShowMobile();

        // do web request
        yield return new WaitForSeconds(3.0f);

        // remove all defects from the asset
        foreach (var pair in defects)
        {
            _levelController.GameStore.RemoveDefect(pair.Value.ItemId);

            // add to the score
            _levelController.ScoreDefect(pair.Value);
        }

        // unlock the person
        _firstPersonController.SetLocked(false);
        _alloyMobileController.HideMobile();

        _working = false;
        _assetItemId = null;
        yield return null;

        // success sound!
        _levelSoundEffectsController.PlaySuccess();
    }
}
