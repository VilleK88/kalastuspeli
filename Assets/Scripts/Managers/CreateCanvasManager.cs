using UnityEngine;

public class CreateCanvasManager : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CreateLoadingCanvas();
            GameManager.Instance.DestroyCityInstance();
        }
    }
}