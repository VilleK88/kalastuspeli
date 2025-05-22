using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject settingsBG;

    public void OpenSettingsMenu()
    {
        settingsBG.SetActive(true);
    }

    public void CloseSettingsMenu()
    {
        settingsBG.SetActive(false);
    }
}