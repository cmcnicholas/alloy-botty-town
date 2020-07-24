using cakeslice;
using UnityEngine;

public class AssetController : MonoBehaviour
{
    public GameObject Asset;
    private GameObject _fire;
    private ParticleSystem _fireParticleSystem;

    // Start is called before the first frame update
    void Start()
    {
        // add the outline component
        Asset.AddComponent<Outline>().enabled = false;

        // make a fire object on the game object, don't emit and clear any effects
        // TODO maybe dynamically make fires on SetFire?
        _fire = Instantiate(Resources.Load("Fire") as GameObject, gameObject.transform);
        _fireParticleSystem = _fire.GetComponent<ParticleSystem>();
        //_fireParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // if we can work out where to place it, do so
        var mr = Asset.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            // make the fire bigger
            var particleSystemShape = _fireParticleSystem.shape;
            particleSystemShape.scale = new Vector3(mr.bounds.size.x, mr.bounds.size.y, mr.bounds.size.z);

            // put the fire on top of the thing
            _fire.transform.localPosition = new Vector3(0.0f, mr.bounds.size.y, 0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFire(bool value)
    {
        if (value)
        {
            _fireParticleSystem.Play(true);
        }
        else
        {
            _fireParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    public void SetInspect(bool value)
    {

    }
}
