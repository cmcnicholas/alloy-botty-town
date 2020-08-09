using Assets.Server;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using Valve.VR;

public class MainMenuController : MonoBehaviour
{
    public GameObject GameOver;
    public GameObject PlayerController;
    public GameObject MainMenuCamera;
    public GameObject MainMenuResumeButton;
    public GameObject MainMenuTimeTrialButton;
    public GameObject MainMenuFreePlayButton;
    public GameObject AssetMenuCanvas;
    public SteamVR_Action_Boolean MenuAction;
    private LevelController _levelController;
    private AssetMenuController _assetMenuController;
    private Button _mainMenuResumeButton;
    private Button _mainMenuTimeTrialButton;
    private Button _mainMenuFreePlayButton;

    // Start is called before the first frame update
    void Start()
    {
        _levelController = PlayerController.GetComponent<LevelController>();
        _mainMenuResumeButton = MainMenuResumeButton.GetComponent<Button>();
        _mainMenuTimeTrialButton = MainMenuTimeTrialButton.GetComponent<Button>();
        _mainMenuFreePlayButton = MainMenuFreePlayButton.GetComponent<Button>();
        _assetMenuController = AssetMenuCanvas.GetComponent<AssetMenuController>();

        // default main menu, no resume
        _mainMenuResumeButton.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        _mainMenuTimeTrialButton.interactable = ApplicationGlobals.ApiTokenVerified;
        _mainMenuFreePlayButton.interactable = ApplicationGlobals.ApiTokenVerified;

        if ((Input.GetKey("escape") || MenuAction.GetState(SteamVR_Input_Sources.Any)) && PlayerController.activeInHierarchy)
        {
            // hide the asset menu
            _assetMenuController.CloseMenu();

            // if the player controller is active then we was playing
            ShowMenu(PlayerController.activeInHierarchy);
        }
    }

    public void ShowMenu(bool paused)
    {
        Debug.Log("Show main menu");
        ApplicationGlobals.LastVrControllerPress = Time.time;

        // if paused, resume is allowed
        _mainMenuResumeButton.interactable = paused;

        MainMenuCamera.SetActive(true);
        GameOver.SetActive(false);
        PlayerController.GetComponent<FirstPersonController>().SetLocked(true);
        PlayerController.SetActive(false);
    }

    public void OnResumePressed()
    {
        Debug.Log("Main menu resume");
        ApplicationGlobals.LastVrControllerPress = Time.time;

        PlayerController.SetActive(true);
        PlayerController.GetComponent<FirstPersonController>().SetLocked(false);
        MainMenuCamera.SetActive(false);
    }

    public void OnFreePlayPressed()
    {
        Debug.Log("Main menu free play");
        ApplicationGlobals.LastVrControllerPress = Time.time;

        // reset the level and play
        _levelController.ResetFreePlay();

        PlayerController.SetActive(true);
        PlayerController.GetComponent<FirstPersonController>().SetLocked(false);
        MainMenuCamera.SetActive(false);
    }

    public void OnTimeTrialPressed()
    {
        Debug.Log("Main menu time trial");
        ApplicationGlobals.LastVrControllerPress = Time.time;

        // reset the level and play
        _levelController.ResetTimeTrial();

        PlayerController.SetActive(true);
        PlayerController.GetComponent<FirstPersonController>().SetLocked(false);
        MainMenuCamera.SetActive(false);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }
}
