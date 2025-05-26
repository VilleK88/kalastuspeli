using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterMenu : MonoBehaviour
{
    [SerializeField] GameObject teuvoObject;
    [SerializeField] GameObject tarjaObject;
    [SerializeField] PlayerCharacter character;

    private void Start()
    {
        if(GameManager.Instance != null)
            InitializeCharacterBGs(GameManager.Instance.character);
    }

    void InitializeCharacterBGs(PlayerCharacter currentCharacter)
    {
        if (currentCharacter == PlayerCharacter.Timmy)
            SetBG(true, false);
        else
            SetBG(false, true);
    }

    public void SelectTeuvo()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        SetBG(true, false);
        SelectThisCharacter(PlayerCharacter.Timmy);
    }

    public void SelectTarja()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
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