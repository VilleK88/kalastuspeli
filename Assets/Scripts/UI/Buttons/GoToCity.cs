using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToCity : MonoBehaviour
{
    public City city;

    public void GoToThisCity()
    {
        GameManager.Instance.SetCity(city);
        SceneManager.LoadScene("3 - City");
    }
}