using Assets.Server.Game;
using System;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public GameObject TimerCounter;
    public GameObject ScoreCounter;
    public int WarnSecondsRemaining;
    public float WarnSecondsRemainingColourOffsetY = 0.7f;
    public int LevelTime = 300;
    private LevelSoundEffectsController _levelSoundEffectsController;
    private HudCounterController _timerCounterController;
    private HudCounterController _scoreCounterController;
    private int _levelScore = 0;
    private float _timeLeft;
    private float _startColourTilingOffsetY;

    // this holds all our models e.g. assets/jobs/inspections
    public GameStore GameStore { get; } = new GameStore();

    // Start is called before the first frame update
    void Start()
    {
        _levelSoundEffectsController = GetComponent<LevelSoundEffectsController>();
        _timerCounterController = TimerCounter.GetComponent<HudCounterController>();
        _scoreCounterController = ScoreCounter.GetComponent<HudCounterController>();

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

        bool isWarning = _timeLeft <= WarnSecondsRemaining;
        _timerCounterController.ColourTilingOffsetY = isWarning ? WarnSecondsRemainingColourOffsetY : _startColourTilingOffsetY;

        if (_timeLeft > 0 && isWarning)
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
