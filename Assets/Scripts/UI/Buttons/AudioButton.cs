using UnityEngine;
using UnityEngine.UI;

public class AudioButton : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    [SerializeField] GameObject checkmark;

    private void Start()
    {
        toggle.onValueChanged.AddListener(SetAudioMuted);
        bool audioOn = !AudioManager.Instance.IsMuted;
        toggle.isOn = audioOn;
        checkmark.SetActive(audioOn);
    }

    void SetAudioMuted(bool isOn)
    {
        bool muted = !isOn;
        AudioManager.Instance.SetMuted(muted);
        checkmark.SetActive(muted);
    }
}