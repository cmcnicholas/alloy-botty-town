using UnityEngine;

public class CloudManager : MonoBehaviour
{
    public float MinX;
    public float MaxX;
    public float MinZ;
    public float MaxZ;
    public float Speed;

    private float _directionX;
    private float _directionZ;

    // Start is called before the first frame update
    void Start()
    {
        // random start direction for x and z
        _directionX = Random.value > 0.5f ? Speed : -Speed;
        _directionZ = Random.value > 0.5f ? Speed : -Speed;
    }

    // Update is called once per frame
    void Update()
    {
        // move the cloud
        gameObject.transform.Translate(new Vector3(_directionX, 0.0f, _directionZ));

        // check if we have to change direction
        if (gameObject.transform.position.x > MaxX)
        {
            _directionX = -Speed;
        }
        else if (gameObject.transform.position.x < MinX)
        {
            _directionX = Speed;
        }
        if (gameObject.transform.position.z > MaxZ)
        {
            _directionZ = -Speed;
        }
        else if (gameObject.transform.position.z < MinZ)
        {
            _directionZ = Speed;
        }
    }
}
