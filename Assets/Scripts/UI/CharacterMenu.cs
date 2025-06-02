using UnityEngine;
using UnityEngine.UI;

public class CharacterMenu : MonoBehaviour
{
    #region Singleton
    public static CharacterMenu Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    [SerializeField] GameObject[] characterButtons;
    [SerializeField] PlayerCharacter character;

    private void Start()
    {
        SetBG();
    }

    public void SelectCharacter(PlayerCharacter thisCharacter)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SetCharacter(thisCharacter);
        SetBG();
    }

    void SetBG()
    {
        for(int i = 0; i < characterButtons.Length; i++)
        {
            GameObject buttonObject = characterButtons[i];
            Image buttonImage = buttonObject.GetComponent<Image>();
            if (GameManager.Instance.character == buttonObject.GetComponent<CharacterButton>().character)
                buttonImage.color = SetColor(buttonImage.color, 1f);
            else
                buttonImage.color = SetColor(buttonImage.color, 0f);
        }
    }

    Color SetColor(Color c, float value)
    {
        c.a = value;
        return c;
    }
}