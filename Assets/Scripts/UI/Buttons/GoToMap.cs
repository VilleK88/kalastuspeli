using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToMap : MonoBehaviour
{
    public void GoBackToMap()
    {
        SceneManager.LoadScene("2 - Map");
    }
}