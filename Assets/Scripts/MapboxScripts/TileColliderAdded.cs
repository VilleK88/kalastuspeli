using UnityEngine;
using Mapbox.Unity.Map;
using System.Collections;

public class TileColliderAdded : MonoBehaviour
{
    public AbstractMap map;

    private void Start()
    {
        StartCoroutine(AddCollidersToMapTiles());
    }

    IEnumerator AddCollidersToMapTiles()
    {
        yield return new WaitForSeconds(3f);

        var tiles = map.MapVisualizer.ActiveTiles;
        foreach(var tile in tiles)
        {
            var tileTransform = tile.Value.transform;
            if(tileTransform != null)
            {
                Debug.Log("Tile found");
                AddCollidersRecursively(tileTransform);
            }
            else
            {
                Debug.Log("Tile not found");
            }
        }
    }

    void AddCollidersRecursively(Transform parent)
    {
        foreach(Transform child in parent)
        {
            var meshFilter = child.GetComponent<MeshFilter>();
            if(meshFilter && child.GetComponent<MeshCollider>() == null)
            {
                var collider = child.gameObject.AddComponent<MeshCollider>();
                collider.sharedMesh = meshFilter.sharedMesh;
            }

            AddCollidersRecursively(child);
        }
    }
}