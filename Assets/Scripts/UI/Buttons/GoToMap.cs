using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class GoToMap : MonoBehaviour
{
    public void GoBackToMap()
    {
        SceneManager.LoadScene("2 - Map");
    }
}