using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
    public List<AudioClip> voiceSounds = new List<AudioClip>();
    public AudioSource musicSource, sfxSource, footstepsSource, voiceSource;
    const string MutePrefKey = "AudioMuted";
    public bool IsMuted { get; private set; } = false;

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "3 - City")
            StartCoroutine(RandomVoiceLoop(30f));
    }

    public void PlayMusic(string name)
    {
        Sound s = System.Array.Find(musicSounds, x => x.name == name);

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
        Sound s = System.Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
            Debug.Log("Sound not found");
        else
            sfxSource.PlayOneShot(s.clip);
    }

    public IEnumerator RandomVoiceLoop(float duration)
    {
        while(true)
        {
            yield return new WaitForSeconds(duration);
            PlayRandomVoice();
        }
    }

    public void PlayRandomVoice()
    {
        int randomIndex = Random.Range(0, voiceSounds.Count);
        voiceSource.PlayOneShot(voiceSounds[randomIndex]);
    }

    public void PlayFootstepsSound()
    {
        Sound s = System.Array.Find(sfxSounds, x => x.name == "Footstep");

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