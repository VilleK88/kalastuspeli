using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToScene : MonoBehaviour
{
    [SerializeField] string sceneName;

    public void GoToThisScene()
    {
        SceneManager.LoadScene(sceneName);
        Debug.Log("Change scene to: " + sceneName);
    }
}