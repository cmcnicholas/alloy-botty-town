using Assets.Server;
using Assets.Server.Api;
using Assets.Server.ApiModels;
using System.Collections;
using System.Collections.Generic;
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
        bool hasDefects = asset.TempDefects.Count > 0;

        var jobsButtonComponent = CloseJobsButton.GetComponent<Button>();
        jobsButtonComponent.interactable = ApplicationGlobals.JobFixAllowed && hasJobs;
        var inspectionsButtonComponent = CloseInspectionsButton.GetComponent<Button>();
        inspectionsButtonComponent.interactable = ApplicationGlobals.InspectionCompleteAllowed && hasInspections;
        var registerDefectButtonComponent = RegisterDefectButton.GetComponent<Button>();
        registerDefectButtonComponent.interactable = ApplicationGlobals.DefectSpawn && hasDefects;

        // open menu sound
        _levelSoundEffectsController.PlayMenuOpen();
    }

    public void OnCompleteJobsPressed()
    {
        // paranoid check
        if (!ApplicationGlobals.JobFixAllowed)
        {
            return;
        }

        _working = true;
        AssetMenu.SetActive(false);
        StartCoroutine(CompleteJobsForAsset());

        // select menu sound
        _levelSoundEffectsController.PlayMenuSelect();
    }

    public void OnCompleteInspectionsPressed()
    {
        // paranoid check
        if (!ApplicationGlobals.InspectionCompleteAllowed)
        {
            return;
        }

        _working = true;
        AssetMenu.SetActive(false);
        StartCoroutine(CompleteInspectionsForAsset());

        // select menu sound
        _levelSoundEffectsController.PlayMenuSelect();
    }

    public void OnRegisterDefectPressed()
    {
        // paranoid check
        if (!ApplicationGlobals.DefectSpawn)
        {
            return;
        }

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
        var jobs = asset.Jobs.Values.ToList();

        // display mobile
        _alloyMobileController.ShowMobile();

        float startTime = Time.time;

        // complete jobs
        foreach (var job in jobs)
        {
            var itemEdit = new ItemEditWebRequestModel("Live", job.Signature);
            itemEdit.SetAttributes(ApplicationGlobals.JobFixAttributes);
            var jobUpdateClient = new JobUpdateClient(ApplicationGlobals.ExtendedApiUrl, ApplicationGlobals.ApiToken, job.ItemId, itemEdit);
            yield return jobUpdateClient.Send();

            if (jobUpdateClient.Error != null)
            {
                // blacklist the job item from further updates
                _levelController.GameStore.AddBlacklistedItemId(job.ItemId);
                Debug.Log("Failed to update job '" + job.ItemId + "' Error: " + jobUpdateClient.Error.Message);
            }
            else if (jobUpdateClient.Response == null)
            {
                // blacklist the job item from further updates
                _levelController.GameStore.AddBlacklistedItemId(job.ItemId);
                Debug.Log("Failed to update job '" + job.ItemId + "' Error: response was null");
            }
            else
            {
                Debug.Log("Job was completed: " + jobUpdateClient.Response?.JobItem?.ItemId);
            }
        }

        float timeRemaining = (ApplicationGlobals.ReportingTime * jobs.Count) - (Time.time - startTime);

        // wait for min amount
        if (timeRemaining > 0)
        {
            yield return new WaitForSeconds(timeRemaining);
        }

        // remove all jobs from the asset
        foreach (var job in jobs)
        {
            _levelController.GameStore.RemoveJob(job.ItemId);

            // add to the score
            _levelController.ScoreJob(job);
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
        var inspections = asset.Inspections.Values.ToList();

        // display mobile
        _alloyMobileController.ShowMobile();

        float startTime = Time.time;

        // complete inspections
        foreach (var inspection in inspections)
        {
            var itemEdit = new ItemEditWebRequestModel("Live", inspection.Signature);
            itemEdit.SetAttributes(ApplicationGlobals.InspectionCompleteAttributes);
            var inspectionUpdateClient = new InspectionUpdateClient(ApplicationGlobals.ExtendedApiUrl, ApplicationGlobals.ApiToken, inspection.ItemId, itemEdit);
            yield return inspectionUpdateClient.Send();

            if (inspectionUpdateClient.Error != null)
            {
                // blacklist the inspection item from further updates
                _levelController.GameStore.AddBlacklistedItemId(inspection.ItemId);
                Debug.Log("Failed to update inspection '" + inspection.ItemId + "' Error: " + inspectionUpdateClient.Error.Message);
            }
            else if (inspectionUpdateClient.Response == null)
            {
                // blacklist the inspection item from further updates
                _levelController.GameStore.AddBlacklistedItemId(inspection.ItemId);
                Debug.Log("Failed to update inspection '" + inspection.ItemId + "' Error: response was null");
            }
            else
            {
                Debug.Log("Inspection was completed: " + inspectionUpdateClient.Response?.InspectionItem?.ItemId);
            }
        }

        float timeRemaining = (ApplicationGlobals.ReportingTime * inspections.Count) - (Time.time - startTime);

        // wait for min of 3 seconds per inspection
        if (timeRemaining > 0)
        {
            yield return new WaitForSeconds(timeRemaining);
        }
        
        // remove all inspections from the asset
        foreach (var inspection in inspections)
        {
            _levelController.GameStore.RemoveInspection(inspection.ItemId);

            // add to the score
            _levelController.ScoreInspection(inspection);
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

        // get all defects on asset
        var defects = asset.TempDefects.Values.ToList();

        // display mobile
        _alloyMobileController.ShowMobile();

        var startTime = Time.time;

        // complete defects
        foreach (var defect in defects)
        {
            var itemCreate = new ItemCreateWebRequestModel(ApplicationGlobals.DefectCreateDesignCode, ApplicationGlobals.DefectCreateCollection);
            itemCreate.SetAttributes(ApplicationGlobals.DefectCreateAttributes);
            itemCreate.Parents.Add("attributes_defectsAssignableDefects", new List<string> { defect.ParentAssetItemId });
            var defectClient = new DefectCreateClient(ApplicationGlobals.ExtendedApiUrl, ApplicationGlobals.ApiToken, itemCreate);
            
            yield return defectClient.Send();
            
            // check if we made the defect
            if (defectClient.Error != null)
            {
                Debug.Log("Failed to create defect... Error: " + defectClient.Error.Message);
            }
            else if (defectClient.Response == null)
            {
                Debug.Log("Failed to create defect... Error: response was null");
            }
            else
            {
                Debug.Log("A wild defect evolved to reported: " + defectClient.Response?.DefectItem?.ItemId);
            }
        }

        float timeRemaining = (ApplicationGlobals.ReportingTime * defects.Count) - (Time.time - startTime);

        // wait for min required time
        if (timeRemaining > 0)
        {
            yield return new WaitForSeconds(timeRemaining);
        }

        // remove all defects from the asset
        foreach (var defect in defects)
        {
            _levelController.GameStore.RemoveTempDefect(defect.TempId);

            // add to the score
            _levelController.ScoreDefect(defect);
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
