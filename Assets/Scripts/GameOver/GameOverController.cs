using Assets.Server;
using Assets.Server.Api;
using Assets.Server.ApiModels;
using Newtonsoft.Json.Linq;
using System.Collections;
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
    public GameObject GameOverPlayerName;
    public GameObject GameOverSaveButton;
    public GameObject GameOverSaveButtonText;
    public GameObject GameOverLeaveButton;
    private Button _gameOverSaveButton;
    private Text _gameOverSaveText;
    private Button _gameOverLeaveButton;
    private InputField _playerNameInputField;
    private LevelController _levelController;
    private bool _saving = false;

    private const string SaveScoreText = "SAVE SCORE";
    private const string SaveScoreSavingText = "SAVING...";

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        _gameOverSaveButton = GameOverSaveButton.GetComponent<Button>();
        _gameOverSaveText = GameOverSaveButtonText.GetComponent<Text>();
        _gameOverLeaveButton = GameOverLeaveButton.GetComponent<Button>();
        _playerNameInputField = GameOverPlayerName.GetComponent<InputField>();
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

        // reset saving
        _saving = false;

        // reset name
        _playerNameInputField.text = null;
    }

    // Update is called once per frame
    void Update()
    {
        _playerNameInputField.interactable = ApplicationGlobals.ScoreSaveAllowed && !_saving;
        _gameOverSaveButton.interactable = ApplicationGlobals.ScoreSaveAllowed && !_saving && !string.IsNullOrWhiteSpace(_playerNameInputField.text);
        _gameOverLeaveButton.interactable = ApplicationGlobals.ScoreSaveAllowed && !_saving;
        _gameOverSaveText.text = _saving ? SaveScoreSavingText : SaveScoreText;
    }

    public void ShowScreen()
    {
        Debug.Log("Show Game Over");
        ApplicationGlobals.LastVrControllerPress = Time.time;

        PlayerController.GetComponent<FirstPersonController>().SetLocked(true);
        PlayerController.SetActive(false);
        gameObject.SetActive(true);
    }

    public void OnLeavePressed()
    {
        Debug.Log("Game Over Leave");
        ApplicationGlobals.LastVrControllerPress = Time.time;

        // start the main menu screen
        MainMenu.GetComponent<MainMenuController>().ShowMenu(false);
    }

    public void OnSavePressed()
    {
        Debug.Log("Game Over Save");
        ApplicationGlobals.LastVrControllerPress = Time.time;

        if (string.IsNullOrWhiteSpace(_playerNameInputField.text))
        {
            return;
        }

        // mark as we are saving
        _saving = true;

        // save player name
        StartCoroutine(SaveScoreCoroutine(_playerNameInputField.text, _levelController.LevelScore));
    }

    public void OnKeyboardPressedQ()
    {
        _playerNameInputField.text += "Q";
    }

    public void OnKeyboardPressedW()
    {
        _playerNameInputField.text += "W";
    }

    public void OnKeyboardPressedE()
    {
        _playerNameInputField.text += "E";
    }

    public void OnKeyboardPressedR()
    {
        _playerNameInputField.text += "R";
    }

    public void OnKeyboardPressedT()
    {
        _playerNameInputField.text += "T";
    }

    public void OnKeyboardPressedY()
    {
        _playerNameInputField.text += "Y";
    }

    public void OnKeyboardPressedU()
    {
        _playerNameInputField.text += "U";
    }

    public void OnKeyboardPressedI()
    {
        _playerNameInputField.text += "I";
    }

    public void OnKeyboardPressedO()
    {
        _playerNameInputField.text += "O";
    }

    public void OnKeyboardPressedP()
    {
        _playerNameInputField.text += "P";
    }

    public void OnKeyboardPressedA()
    {
        _playerNameInputField.text += "A";
    }

    public void OnKeyboardPressedS()
    {
        _playerNameInputField.text += "S";
    }

    public void OnKeyboardPressedD()
    {
        _playerNameInputField.text += "D";
    }

    public void OnKeyboardPressedF()
    {
        _playerNameInputField.text += "F";
    }

    public void OnKeyboardPressedG()
    {
        _playerNameInputField.text += "G";
    }

    public void OnKeyboardPressedH()
    {
        _playerNameInputField.text += "H";
    }

    public void OnKeyboardPressedJ()
    {
        _playerNameInputField.text += "J";
    }

    public void OnKeyboardPressedK()
    {
        _playerNameInputField.text += "K";
    }

    public void OnKeyboardPressedL()
    {
        _playerNameInputField.text += "L";
    }

    public void OnKeyboardPressedZ()
    {
        _playerNameInputField.text += "Z";
    }

    public void OnKeyboardPressedX()
    {
        _playerNameInputField.text += "X";
    }

    public void OnKeyboardPressedC()
    {
        _playerNameInputField.text += "C";
    }

    public void OnKeyboardPressedV()
    {
        _playerNameInputField.text += "V";
    }

    public void OnKeyboardPressedB()
    {
        _playerNameInputField.text += "B";
    }

    public void OnKeyboardPressedN()
    {
        _playerNameInputField.text += "N";
    }

    public void OnKeyboardPressedM()
    {
        _playerNameInputField.text += "M";
    }

    public void OnKeyboardPressedSpace()
    {
        _playerNameInputField.text += " ";
    }

    public void OnKeyboardPressedBackspace()
    {
        if (_playerNameInputField.text == null || _playerNameInputField.text.Length == 0)
        {
            return;
        }
        _playerNameInputField.text = _playerNameInputField.text.Substring(0, _playerNameInputField.text.Length - 1);
    }

    private IEnumerator SaveScoreCoroutine(string playerName, int score)
    {
        var itemCreate = new ItemCreateWebRequestModel(ApplicationGlobals.ScoreSaveDesignCode, ApplicationGlobals.ScoreSaveCollectionCode);
        itemCreate.SetAttributes(new JObject
        {
            { ApplicationGlobals.ScoreSaveNameAttributeCode, playerName },
            { ApplicationGlobals.ScoreSaveScoreAttributeCode, score },
        });
        var itemCreateClient = new ItemCreateClient(ApplicationGlobals.ApiUrl, ApplicationGlobals.ApiToken, itemCreate);
        yield return itemCreateClient.Send();

        if (itemCreateClient.Error != null)
        {
            Debug.Log("Failed to save score, check config for 'ScoreSaveDesignCode', 'ScoreSaveCollectionCode', 'ScoreSaveNameAttributeCode' and 'ScoreSaveScoreAttributeCode' Error: " + itemCreateClient.Error.Message);
        }
        else if (itemCreateClient.Response == null)
        {
            Debug.Log("Failed to save score, check config for 'ScoreSaveDesignCode', 'ScoreSaveCollectionCode', 'ScoreSaveNameAttributeCode' and 'ScoreSaveScoreAttributeCode' Error: response was null");
        }
        else
        {
            Debug.Log("Score saved: " + itemCreateClient.Response.Item.ItemId);
        }

        // set text back and state
        _saving = false;

        // start the main menu screen
        MainMenu.GetComponent<MainMenuController>().ShowMenu(false);
    }
}
