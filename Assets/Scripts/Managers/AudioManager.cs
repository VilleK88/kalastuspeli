using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadMuteState();
        }
        else
            Destroy(Instance);
    }
    #endregion

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource, footstepsSource;
    const string MutePrefKey = "AudioMuted";
    public bool IsMuted { get; private set; } = false;

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
            Debug.Log("Sound not found");
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
            Debug.Log("Sound not found");
        else
            sfxSource.PlayOneShot(s.clip);
    }

    public void PlayFootstepsSound()
    {
        Sound s = Array.Find(sfxSounds, x => x.name == "Footstep");

        if (s == null)
            Debug.Log("Sound not found");
        else
        {
            footstepsSource.clip = s.clip;
            footstepsSource.Play();
        }
    }
    
    public void StopFootstepsSound()
    {
        footstepsSource.Stop();
    }

    public void SetMuted(bool muted)
    {
        IsMuted = muted;

        musicSource.mute = muted;
        sfxSource.mute = muted;
        footstepsSource.mute = muted;

        PlayerPrefs.SetInt(MutePrefKey, muted ? 1 : 0);
        PlayerPrefs.Save();
    }

    void LoadMuteState()
    {
        IsMuted = PlayerPrefs.GetInt(MutePrefKey, 0) == 1;
        musicSource.mute = IsMuted;
        sfxSource.mute = IsMuted;
        footstepsSource.mute = IsMuted;
    }
}