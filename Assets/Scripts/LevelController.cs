using System;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public GameObject TimerCounter;
    public GameObject ScoreCounter;
    private HudCounterController _timerCounterController;
    private HudCounterController _scoreCounterController;
    private int _levelTime = 300;
    private int _levelScore = 0;
    private float _startTime;

    // Start is called before the first frame update
    void Start()
    {
        _timerCounterController = TimerCounter.GetComponent<HudCounterController>();
        _scoreCounterController = ScoreCounter.GetComponent<HudCounterController>();

        // log start time
        _startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // update the time left in the game
        int timeLeft = _levelTime - (int)Math.Ceiling(Time.time - _startTime);
        _timerCounterController.Number = timeLeft;

        // update the score
        _scoreCounterController.Number = _levelScore;
    }

    public void AddScore(int score)
    {
        _levelScore += Math.Abs(score);
    }
}
