using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip _winSound, _vacumCleanerWorkSound, _vacumOnOffSound;
    [SerializeField] private AudioClip _returningInTimeSound;

    [SerializeField] private List<AudioClip> _ghostCaught;

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

    public void PlayWinSound()
    {
        _soundAudioSource.PlayOneShot(_winSound, 0.5f);
    }

    public void PlayVacumCleanerWorkSound()
    {
        _soundAudioSource.PlayOneShot(_vacumCleanerWorkSound, 0.1f);
    }

    public void PlayVacumOnOffSound()
    {
        _soundAudioSource.PlayOneShot(_vacumOnOffSound, 0.4f);
    }

    public void PlayGhostCaughtSound()
    {
        _soundAudioSource.PlayOneShot(_ghostCaught[Random.Range(0, _ghostCaught.Count)], 0.5f);
    }
}
