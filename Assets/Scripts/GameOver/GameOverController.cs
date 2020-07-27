using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class GameOverController : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject PlayerController;
    public GameObject GameOverScore;
    public GameObject GameOverJobsText;
    public GameObject GameOverInspectionsText;
    public GameObject GameOverDefectsText;
    private LevelController _levelController;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        _levelController = PlayerController.GetComponent<LevelController>();

        // update the text telling how many things they did
        GameOverDefectsText.GetComponent<Text>().text = _levelController.DefectsCompleted == 1 ? 
            "1 DEFECT REPORTED" : $"{_levelController.DefectsCompleted} DEFECTS REPORTED";
        GameOverJobsText.GetComponent<Text>().text = _levelController.JobsCompleted == 1 ? 
            "1 JOB COMPLETED" : $"{_levelController.JobsCompleted} JOBS COMPLETED";
        GameOverInspectionsText.GetComponent<Text>().text = _levelController.InspectionsCompleted == 1 ? 
            "1 INSPECTED ASSET" : $"{_levelController.InspectionsCompleted} INSPECTED ASSETS";

        // update the score
        GameOverScore.GetComponent<HudCounterController>().Number = _levelController.LevelScore;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowScreen()
    {
        PlayerController.GetComponent<FirstPersonController>().SetLocked(true);
        PlayerController.SetActive(false);
        gameObject.SetActive(true);
    }

    public void OnLeavePressed()
    {
        // start the main menu screen
        MainMenu.GetComponent<MainMenuController>().ShowMenu(false);
    }

    public void OnSavePressed()
    {
        // start the main menu screen
        MainMenu.GetComponent<MainMenuController>().ShowMenu(false);
    }
}
