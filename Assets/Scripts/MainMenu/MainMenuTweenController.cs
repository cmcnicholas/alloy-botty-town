using System.Collections;
using UnityEngine;

public class MainMenuTweenController : MonoBehaviour
{
    public GameObject MainMenuAlloyExperiment;
    public GameObject MainMenuTitle;
    public GameObject MainMenuOptions;

    // Start is called before the first frame update
    void Start()
    {
        // initially set to transparent
        var renderer = MainMenuAlloyExperiment.GetComponent<Renderer>();
        var colour = renderer.material.color;
        renderer.material.color = new Color(colour.r, colour.g, colour.b, 0f);

        // run and tween
        StartCoroutine(FadeInCoroutine());
        
        // entry punch!
        iTween.PunchScale(MainMenuTitle, new Vector3(10f, 10f, 10f), 1.0f);
        StartCoroutine(PunchCoroutine());

        // hide options
        MainMenuOptions.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator FadeInCoroutine()
    {
        yield return new WaitForSeconds(1f);
        iTween.FadeTo(MainMenuAlloyExperiment, 1f, 2f);
    }

    private IEnumerator PunchCoroutine()
    {
        yield return new WaitForSeconds(1f);
        MainMenuOptions.SetActive(true);

        while (true)
        {
            yield return new WaitForSeconds(3.0f);
            iTween.PunchScale(MainMenuTitle, new Vector3(0, 0, 1f), 1.0f);
        }
    }
}
