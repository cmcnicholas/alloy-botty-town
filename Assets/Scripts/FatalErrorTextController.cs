using Assets.Server;
using UnityEngine;
using UnityEngine.UI;

public class FatalErrorTextController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Text>().text = string.IsNullOrWhiteSpace(ApplicationGlobals.FatalError) ? 
            "NO FATAL ERROR, WAT?" : ApplicationGlobals.FatalError.ToUpper();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape") || Input.GetMouseButton(0))
        {
            Application.Quit();
        }
    }
}
