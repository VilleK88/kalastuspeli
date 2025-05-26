using UnityEngine;
using UnityEngine.UI;

public class CharacterMenu : MonoBehaviour
{
    [SerializeField] GameObject teuvoObject;
    [SerializeField] GameObject tarjaObject;
    [SerializeField] PlayerCharacter character;

    public void SelectTeuvo()
    {
        SetBG(true, false);
        SelectThisCharacter(PlayerCharacter.Timmy);
    }

    public void SelectTarja()
    {
        SetBG(false, true);
        SelectThisCharacter(PlayerCharacter.Claire);
    }

    void SetBG(bool a, bool b)
    {
        Image teuvoImg = teuvoObject.GetComponent<Image>();
        teuvoImg.enabled = a;
        Image tarjaImg = tarjaObject.GetComponent<Image>();
        tarjaImg.enabled = b;
    }

    void SelectThisCharacter(PlayerCharacter thisCharacter)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SetCharacter(thisCharacter);
    }
}