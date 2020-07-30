using Assets.Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public GameObject GameOver;
    public GameObject TimerCounter;
    public GameObject ScoreCounter;
    public GameObject AssetMenuCanvas;
    public int WarnSecondsRemaining;
    public float WarnSecondsRemainingColourOffsetY = 0.7f;
    public int LevelTime = 15;
    public int LevelScore = 0;
    public bool IsFreePlay = false;
    private GameOverController _gameOverController;
    private LevelSoundEffectsController _levelSoundEffectsController;
    private HudCounterController _timerCounterController;
    private HudCounterController _scoreCounterController;
    private AssetMenuController _assetMenuController;
    private IList<InspectionModel> _scoredInspections = new List<InspectionModel>();
    private IList<TempDefectModel> _scoredDefects = new List<TempDefectModel>();
    private IList<JobModel> _scoredJobs = new List<JobModel>();
    private float _timeLeft;
    private float _startColourTilingOffsetY;

    // this holds all our models e.g. assets/jobs/inspections
    public GameStore GameStore { get; } = new GameStore();

    public int InspectionsCompleted { get { return _scoredInspections.Count; } }
    public int JobsCompleted { get { return _scoredJobs.Count; } }
    public int DefectsCompleted { get { return _scoredDefects.Count; } }

    // Start is called before the first frame update
    void Start()
    {
        _gameOverController = GameOver.GetComponent<GameOverController>();

        _levelSoundEffectsController = GetComponent<LevelSoundEffectsController>();
        _timerCounterController = TimerCounter.GetComponent<HudCounterController>();
        _scoreCounterController = ScoreCounter.GetComponent<HudCounterController>();
        _assetMenuController = AssetMenuCanvas.GetComponent<AssetMenuController>();

        // set the time left
        _timeLeft = LevelTime;

        // log starting colour
        _startColourTilingOffsetY = _timerCounterController.ColourTilingOffsetY;
    }

    // Update is called once per frame
    void Update()
    {
        // update the time left in the game
        _timeLeft -= Time.deltaTime;
        _timerCounterController.Number = (int)Math.Ceiling(_timeLeft);

        if (_timeLeft <= 0f)
        {
            // remove HUD first then open game over
            _assetMenuController.CloseMenu();
            _gameOverController.ShowScreen();
            return;
        }

        bool isWarning = _timeLeft <= WarnSecondsRemaining;
        _timerCounterController.ColourTilingOffsetY = isWarning ? WarnSecondsRemainingColourOffsetY : _startColourTilingOffsetY;

        if (isWarning)
        {
            _levelSoundEffectsController.PlayTimerCountdown();
        }
        else
        {
            _levelSoundEffectsController.StopTimerCountdown();
        }

        // update the score
        _scoreCounterController.Number = LevelScore;
    }

    public void ScoreInspection(InspectionModel inspection)
    {
        _scoredInspections.Add(inspection);
        RecalculateScore();
    }

    public void ScoreDefect(TempDefectModel defect)
    {
        _scoredDefects.Add(defect);
        RecalculateScore();
    }

    public void ScoreJob(JobModel job)
    {
        _scoredJobs.Add(job);
        RecalculateScore();
    }

    public void ResetTimeTrial()
    {
        IsFreePlay = false;
        LevelScore = 0;
        _scoredDefects.Clear();
        _scoredInspections.Clear();
        _scoredJobs.Clear();
        _timeLeft = LevelTime;

        // turn on the timer
        TimerCounter.SetActive(true);
    }

    public void ResetFreePlay()
    {
        IsFreePlay = true;
        LevelScore = 0;
        _scoredDefects.Clear();
        _scoredInspections.Clear();
        _scoredJobs.Clear();
        _timeLeft = LevelTime;

        // turn off the timer
        TimerCounter.SetActive(false);
    }

    private void RecalculateScore()
    {
        int totalUniqueJobs = _scoredJobs.Select(j => j.ParentAssetItemId).Distinct().Count();
        int totalUniqueDefects = _scoredDefects.Select(d => d.ParentAssetItemId).Distinct().Count();
        int totalUniqueInspections = _scoredInspections.Select(i => i.ParentAssetItemId).Distinct().Count();

        LevelScore = Math.Abs(
            // unique tasks and defects are worth 800 each (1k with the 200 pts below)
            (totalUniqueJobs * 800) + 
            (totalUniqueDefects * 800) + 
            (totalUniqueInspections * 800) + 

            // additionally jobs, defects and inspections are worth 200 on their own
            (_scoredJobs.Count * 200) + 
            (_scoredDefects.Count * 200) + 
            (_scoredInspections.Count * 200)
        );
    }
}
