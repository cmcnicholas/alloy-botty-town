using System.Collections.Generic;
using UnityEngine;

public class LevelSoundEffectsController : MonoBehaviour
{
    public GameObject AudioMenuSounds;
    public GameObject AudioTimerSounds;
    public GameObject AudioScoreSounds;
    public List<AudioClip> SuccessClips;
    public AudioClip MenuOpenClip;
    public AudioClip MenuCloseClip;
    public AudioClip MenuSelectClip;
    public AudioClip TimerCountdownClip;
    private AudioSource _menuAudioSource;
    private AudioSource _timerAudioSource;
    private AudioSource _scoreAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        _menuAudioSource = AudioMenuSounds.GetComponent<AudioSource>();
        _timerAudioSource = AudioTimerSounds.GetComponent<AudioSource>();
        _scoreAudioSource = AudioScoreSounds.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySuccess()
    {
        _scoreAudioSource.clip = SuccessClips[Random.Range(0, SuccessClips.Count - 1)];
        _scoreAudioSource.Play();
        _scoreAudioSource.playOnAwake = false;
    }

    public void PlayMenuOpen()
    {
        PlayMenuSound(MenuOpenClip);
    }

    public void PlayMenuClosed()
    {
        PlayMenuSound(MenuCloseClip);
    }

    public void PlayMenuSelect()
    {
        PlayMenuSound(MenuSelectClip);
    }

    public void PlayTimerCountdown()
    {
        if (_timerAudioSource.isPlaying)
        {
            return;
        }
        _timerAudioSource.clip = TimerCountdownClip;
        _timerAudioSource.loop = true;
        _timerAudioSource.Play();
        _timerAudioSource.playOnAwake = true;
    }

    public void StopTimerCountdown()
    {
        _timerAudioSource.Stop();
    }

    private void PlayMenuSound(AudioClip clip)
    {
        _menuAudioSource.clip = clip;
        _menuAudioSource.Play();
        _menuAudioSource.playOnAwake = false;
    }
}
