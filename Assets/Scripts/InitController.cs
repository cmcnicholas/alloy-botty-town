using Assets.Server;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // do some initialisation before everything!
        ApplicationGlobals.Init();

        // load the game
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
