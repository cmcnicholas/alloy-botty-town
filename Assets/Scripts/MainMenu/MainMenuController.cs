using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MainMenuController : MonoBehaviour
{
    public GameObject PlayerController;
    public GameObject MainMenuCamera;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
        {
            ShowMenu();
        }
    }

    public void ShowMenu()
    {
        MainMenuCamera.SetActive(true);
        PlayerController.GetComponent<FirstPersonController>().SetLocked(true);
        PlayerController.SetActive(false);
    }

    public void OnResumePressed()
    {
        PlayerController.SetActive(true);
        PlayerController.GetComponent<FirstPersonController>().SetLocked(false);
        MainMenuCamera.SetActive(false);
    }

    public void OnFreePlayPressed()
    {

    }

    public void OnTimeTrialPressed()
    {
        PlayerController.SetActive(true);
        PlayerController.GetComponent<FirstPersonController>().SetLocked(false);
        MainMenuCamera.SetActive(false);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }
}
