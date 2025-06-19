using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image checkmarkImg;

    private void Start()
    {
        bool isMuted = AudioManager.Instance.IsMuted;
        checkmarkImg.enabled = isMuted;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool isCurrentlyMuted = AudioManager.Instance.IsMuted;
        bool newMuteState = !isCurrentlyMuted;

        AudioManager.Instance.SetMuted(newMuteState);
        checkmarkImg.enabled = newMuteState;
    }
}