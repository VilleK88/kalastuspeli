using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TMP_LinkHandler : MonoBehaviour, IPointerClickHandler
{
    TextMeshProUGUI tmp;

    public System.Action<string> OnLinkClicked;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmp, eventData.position, eventData.enterEventCamera);
        if(linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = tmp.textInfo.linkInfo[linkIndex];
            string linkID = linkInfo.GetLinkID();
            OnLinkClicked?.Invoke(linkID);
            Debug.Log($"Clicked link: {linkID}");
        }
    }
}