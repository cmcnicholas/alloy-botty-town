using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLooper : MonoBehaviour
{
    public List<AudioClip> Clips;
    private AudioSource _audioSource;
    private int _currentlyPlayingIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // get the audio source to play on
        _audioSource = GetComponent<AudioSource>();

        // start looping
        if (Clips != null && Clips.Count > 0)
        {
            StartCoroutine(LoopAudioCoroutine());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LoopAudioCoroutine()
    {
        while (true)
        {
            _audioSource.clip = Clips[_currentlyPlayingIndex];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);

            // increment to next track
            _currentlyPlayingIndex++;
            if (_currentlyPlayingIndex >= Clips.Count)
            {
                // go back to start
                _currentlyPlayingIndex = 0;
            }
        } 
    }
}
