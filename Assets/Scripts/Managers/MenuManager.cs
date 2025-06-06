using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Objects")]
    [SerializeField] GameObject settingsBG;
    [SerializeField] GameObject characterMenuBG;
    [Header("Audio")]
    [SerializeField] GameObject audioSettings;
    public Slider masterVol, musicVol, sfxVol;
    public AudioMixer MainAudioMixer;

    private void Start()
    {
        masterVol.value = PlayerPrefs.GetFloat("MasterVolume", masterVol.value);
        musicVol.value = PlayerPrefs.GetFloat("MusicVolume", musicVol.value);
        sfxVol.value = PlayerPrefs.GetFloat("SFXVolume", sfxVol.value);
        ChangeMasterVolume();
        ChangeMusicVolume();
        ChangeSFXVolume();
        masterVol.onValueChanged.AddListener(delegate { ChangeMasterVolume(); });
        musicVol.onValueChanged.AddListener(delegate { ChangeMusicVolume(); });
        sfxVol.onValueChanged.AddListener(delegate { ChangeSFXVolume(); });
    }

    public void StartGame()
    {
        characterMenuBG.SetActive(false);
        SceneManager.LoadScene("2 - Map");
    }

    public void OpenCharacterMenu()
    {
        characterMenuBG.SetActive(true);
    }

    public void CloseCharacterMenu()
    {
        characterMenuBG.SetActive(false);
    }

    public void OpenSettingsMenu()
    {
        settingsBG.SetActive(true);
    }

    public void CloseSettingsMenu()
    {
        settingsBG.SetActive(false);
    }

    public void OpenAudioSettings()
    {
        audioSettings.SetActive(true);
    }

    public void CloseAudioSettings()
    {
        audioSettings.SetActive(false);
    }

    public void ChangeMasterVolume()
    {
        MainAudioMixer.SetFloat("MasterVol", masterVol.value);
        PlayerPrefs.SetFloat("MasterVolume", masterVol.value);
        PlayerPrefs.Save();
    }
    public void ChangeMusicVolume()
    {
        MainAudioMixer.SetFloat("MusicVol", musicVol.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVol.value);
        PlayerPrefs.Save();
    }
    public void ChangeSFXVolume()
    {
        MainAudioMixer.SetFloat("SFXVol", sfxVol.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVol.value);
        PlayerPrefs.Save();
    }
}