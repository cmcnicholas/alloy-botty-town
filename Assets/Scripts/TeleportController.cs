using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;

public class TeleportController : MonoBehaviour
{
    public GameObject PlayerController;
    public SteamVR_Input_Sources HandType;
    public SteamVR_Action_Boolean TeleportAction;
    private float? _lastTeleport;
    private SteamVR_LaserPointer laserPointer;
    
    private int layermask1 = 1 << 8;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        laserPointer = GetComponent<SteamVR_LaserPointer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TeleportAction.GetState(HandType) && (_lastTeleport == null || _lastTeleport + 0.5f < Time.time))
        {
            // find any new hit
            if (Physics.Raycast(laserPointer.transform.position, laserPointer.transform.forward, out RaycastHit hit, 50f, layermask1))
            {
                PlayerController.transform.position = new Vector3(hit.point.x, 0f, hit.point.z);
                _lastTeleport = Time.time;
            }
        }
    }
}
