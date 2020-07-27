using Assets.Server.Game;
using System;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public GameObject TimerCounter;
    public GameObject ScoreCounter;
    public int WarnSecondsRemaining;
    public float WarnSecondsRemainingColourOffsetY = 0.7f;
    private LevelSoundEffectsController _levelSoundEffectsController;
    private HudCounterController _timerCounterController;
    private HudCounterController _scoreCounterController;
    private int _levelTime = 300;
    private int _levelScore = 0;
    private float _startTime;
    private float _startColourTilingOffsetY;

    // this holds all our models e.g. assets/jobs/inspections
    public GameStore GameStore { get; } = new GameStore();

    // Start is called before the first frame update
    void Start()
    {
        _levelSoundEffectsController = GetComponent<LevelSoundEffectsController>();
        _timerCounterController = TimerCounter.GetComponent<HudCounterController>();
        _scoreCounterController = ScoreCounter.GetComponent<HudCounterController>();

        // log start time
        _startTime = Time.time;

        // log starting colour
        _startColourTilingOffsetY = _timerCounterController.ColourTilingOffsetY;
    }

    // Update is called once per frame
    void Update()
    {
        // update the time left in the game
        int timeLeft = _levelTime - (int)Math.Ceiling(Time.time - _startTime);
        _timerCounterController.Number = timeLeft;

        bool isWarning = timeLeft <= WarnSecondsRemaining;
        _timerCounterController.ColourTilingOffsetY = isWarning ? WarnSecondsRemainingColourOffsetY : _startColourTilingOffsetY;

        if (timeLeft > 0 && isWarning)
        {
            _levelSoundEffectsController.PlayTimerCountdown();
        }
        else
        {
            _levelSoundEffectsController.StopTimerCountdown();
        }

        // update the score
        _scoreCounterController.Number = _levelScore;
    }

    public void AddScore(int score)
    {
        _levelScore += Math.Abs(score);
    }
}
