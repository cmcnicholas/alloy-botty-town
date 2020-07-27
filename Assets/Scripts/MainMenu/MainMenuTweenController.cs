using System.Collections;
using UnityEngine;

public class MainMenuTweenController : MonoBehaviour
{
    public GameObject MainMenuAlloyExperiment;
    public GameObject MainMenuTitle;
    public GameObject MainMenuOptions;
    public GameObject MainMenuHintFixJobs;
    public GameObject MainMenuHintFixInspections;
    public GameObject MainMenuHintRegisterDefects;
    public GameObject MainMenuHintSelection;
    public float OffscreenMoveValue = 500f;

    // Start is called before the first frame update
    void Start()
    {
        // initially set to transparent
        var alloyExperimentRenderer = MainMenuAlloyExperiment.GetComponent<Renderer>();
        var alloyExperimentColour = alloyExperimentRenderer.material.color;
        alloyExperimentRenderer.material.color = new Color(alloyExperimentColour.r, alloyExperimentColour.g, alloyExperimentColour.b, 0f);

        // run and tween
        StartCoroutine(FadeInAlloyExperimentCoroutine());
        
        // entry punch for title
        iTween.PunchScale(MainMenuTitle, new Vector3(10f, 10f, 10f), 1.0f);
        StartCoroutine(PunchTitleCoroutine());

        // hide options intially
        MainMenuOptions.SetActive(false);

        // set scales
        MainMenuHintFixJobs.transform.localScale = Vector3.zero;
        MainMenuHintFixInspections.transform.localScale = Vector3.zero;

        // set initial offscreen for some hints
        iTween.MoveAdd(MainMenuHintRegisterDefects, new Vector3(-OffscreenMoveValue, 0, 0), 0f);
        iTween.MoveAdd(MainMenuHintSelection, new Vector3(OffscreenMoveValue, 0, 0), 0f);

        // cycle hints
        StartCoroutine(CycleHintsPage1Coroutine());
        StartCoroutine(CycleHintsPage2Coroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator FadeInAlloyExperimentCoroutine()
    {
        yield return new WaitForSeconds(1f);
        iTween.FadeTo(MainMenuAlloyExperiment, 1f, 2f);
    }

    private IEnumerator PunchTitleCoroutine()
    {
        yield return new WaitForSeconds(1f);

        // main menu options become available
        MainMenuOptions.SetActive(true);

        while (true)
        {
            yield return new WaitForSeconds(3.0f);
            iTween.PunchScale(MainMenuTitle, new Vector3(0, 0, 1f), 1.0f);
        }
    }

    private IEnumerator CycleHintsPage1Coroutine()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            // animate in
            iTween.ScaleTo(MainMenuHintFixJobs, new Vector3(1f, 1f, 1f), 1f);
            iTween.ScaleTo(MainMenuHintFixInspections, new Vector3(1f, 1f, 1f), 1f);

            yield return new WaitForSeconds(5f);

            // animate out
            iTween.ScaleTo(MainMenuHintFixJobs, Vector3.zero, 1f);
            iTween.ScaleTo(MainMenuHintFixInspections, Vector3.zero, 1f);

            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator CycleHintsPage2Coroutine()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            yield return new WaitForSeconds(5f);

            // animate in
            iTween.MoveAdd(MainMenuHintRegisterDefects, new Vector3(OffscreenMoveValue, 0f, 0f), 1f);
            iTween.MoveAdd(MainMenuHintSelection, new Vector3(-OffscreenMoveValue, 0f, 0f), 1f);

            yield return new WaitForSeconds(5f);

            // animate out
            iTween.MoveAdd(MainMenuHintRegisterDefects, new Vector3(-OffscreenMoveValue, 0f, 0f), 4f);
            iTween.MoveAdd(MainMenuHintSelection, new Vector3(OffscreenMoveValue, 0f, 0f), 4f);
        }
    }
}
