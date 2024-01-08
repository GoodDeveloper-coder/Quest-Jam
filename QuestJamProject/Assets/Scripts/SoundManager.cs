using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip _vacumCleanerSuckUpGhostSound, _winSound, _vacumCleanerWorkSound, _backInTimeSound;

    [SerializeField] private AudioSource _soundAudioSource;

    [SerializeField] private AudioSource _tracksAudioSource;
    [SerializeField] private AudioClip[] _gameplayTracks;

    void Update()
    {
        if (!_tracksAudioSource.isPlaying)
        {
            int randomNumber = Random.Range(0, _gameplayTracks.Length);
            _tracksAudioSource.clip = _gameplayTracks[randomNumber];
            _tracksAudioSource.Play();
        }
    }

    public void PlaySuckUpGhostSound()
    {
        _soundAudioSource.PlayOneShot(_vacumCleanerSuckUpGhostSound, 0.5f);
    }

    public void PlayWinSound()
    {
        _soundAudioSource.PlayOneShot(_winSound, 0.5f);
    }

    public void PlayVacumCleanerWorkSound()
    {
        _soundAudioSource.PlayOneShot(_vacumCleanerWorkSound, 0.5f);
    }

    public void PlayBackInTimeSound()
    {
        _soundAudioSource.PlayOneShot(_backInTimeSound, 0.5f);
    }
}
