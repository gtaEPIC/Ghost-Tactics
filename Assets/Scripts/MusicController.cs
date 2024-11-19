using UnityEngine;

public class MusicController : MonoBehaviour
{

    [SerializeField] private AudioClip[] _musicClips;
    private AudioSource _audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    void pickRandomMusic()
    {
        int randomIndex = Random.Range(0, _musicClips.Length);
        _audioSource.clip = _musicClips[randomIndex];
        _audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // If the music is not playing, play a random song
        if (!_audioSource.isPlaying)
        {
            pickRandomMusic();
        }
    }
}
