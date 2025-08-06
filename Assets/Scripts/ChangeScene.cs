using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string sceneName;

    private void Start()
    {
        SceneManager.LoadScene(sceneName);
    }
}