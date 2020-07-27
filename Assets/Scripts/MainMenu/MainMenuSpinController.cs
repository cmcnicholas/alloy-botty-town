using UnityEngine;

public class MainMenuSpinController : MonoBehaviour
{
    public float Speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, 1f, 0f), Speed * Time.deltaTime);
    }
}
