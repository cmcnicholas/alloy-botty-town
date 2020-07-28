using Assets.Server;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // do some initialisation before everything!
        try
        {
            ApplicationGlobals.Init();

            // load the game
            SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            Debug.Log("Failed to init from application globals: " + e.Message);
            ApplicationGlobals.FatalError = "FAILED TO INITIALISE GLOBALS, CHECK YOUR CONFIGURATION OR DELETE YOUR .CONFIG FILE @ " + ApplicationGlobals.GetConfigFilePath();
            SceneManager.LoadScene("FatalErrorScene");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
