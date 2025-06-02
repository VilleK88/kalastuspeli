using UnityEngine;

public class CharacterButton : MonoBehaviour
{
    public PlayerCharacter character;

    public void SelectCharacter()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        CharacterMenu.Instance.SelectCharacter(character);

        Debug.Log("Button clicked: " + character);
    }
}