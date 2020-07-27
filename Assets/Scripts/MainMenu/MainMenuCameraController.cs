using UnityEngine;

public class MainMenuCameraController : MonoBehaviour
{
    public float Speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(0f, Time.deltaTime * Speed, 0f);
    }
}
