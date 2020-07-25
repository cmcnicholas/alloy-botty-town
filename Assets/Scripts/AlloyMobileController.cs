using UnityEngine;

public class AlloyMobileController : MonoBehaviour
{
    public float Speed;
    public Vector3 PositionOnScreen;
    private bool _visible = false;
    private Vector3 _positionOffScreen;

    // Start is called before the first frame update
    void Start()
    {
        _positionOffScreen = gameObject.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float step = Speed * Time.deltaTime;
        
        if (_visible)
        {
            if (!transform.localPosition.Equals(PositionOnScreen))
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, PositionOnScreen, step);
            }
        }
        else
        {
            if (!transform.localPosition.Equals(_positionOffScreen))
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, _positionOffScreen, step);
            }
        }
    }

    public void ShowMobile()
    {
        _visible = true;
    }

    public void HideMobile()
    {
        _visible = false;
    }
}
