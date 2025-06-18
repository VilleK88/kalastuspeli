using UnityEngine;
using Unity.AI.Navigation;

public class CityNavMeshSurfaceBuilder : MonoBehaviour
{
    #region Singleton
    public static CityNavMeshSurfaceBuilder Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    [SerializeField] NavMeshSurface surface;
    [SerializeField] GameObject npcs;

    private void Start()
    {
        surface.BuildNavMesh();

        if(npcs != null)
            npcs.SetActive(true);

        if (GridManager.Instance != null)
        {
            GridManager.Instance.SetGridManagerPosition();
            GridManager.Instance.InitializeGrid();
        }

        MarkerManager.Instance.InitializeMarkers();
    }

    public Vector3 GetNavMeshSurfaceCenter()
    {
        return surface.center;
    }
}