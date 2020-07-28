using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioLooper : MonoBehaviour
{
    public List<GameObject> Cameras;
    public List<AudioClip> Clips;
    private IList<AudioSource> _audioSources;
    private int _currentlyPlayingIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    void OnEnable()
    {
        // get the audio source(s) to play on
        _audioSources = Cameras.Select(c => c.GetComponent<AudioSource>()).ToList();

        // start looping
        if (Clips != null && Clips.Count > 0)
        {
            // need to restart this when game object is disabled/enabled
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
            var clip = Clips[_currentlyPlayingIndex];
            foreach (var audioSource in _audioSources)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
            yield return new WaitForSeconds(clip.length);

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
