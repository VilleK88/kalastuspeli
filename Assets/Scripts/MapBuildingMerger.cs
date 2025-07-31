using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class MapBuildingMerger : MonoBehaviour
{
    [ContextMenu("Merge Connected Buildings")]
    public void MergeConnectedBuildings()
    {
        Transform parent = this.transform;

        List<GameObject> buildings = new List<GameObject>();
        foreach(Transform child in parent)
        {
            if (child.GetComponent<MeshFilter>() && child.GetComponent<MeshRenderer>())
                buildings.Add(child.gameObject);
        }

        HashSet<GameObject> visited = new HashSet<GameObject>();
        int groupIndex = 0;

        foreach(GameObject building in buildings)
        {
            if (visited.Contains(building))
                continue;

            List<GameObject> group = new List<GameObject> { building };
            visited.Add(building);

            Bounds groupBounds = building.GetComponent<Collider>().bounds;

            for(int i = 0; i < buildings.Count; i++)
            {
                GameObject other = buildings[i];
                if (visited.Contains(other) || other == building)
                    continue;

                if(groupBounds.Intersects(other.GetComponent<Collider>().bounds))
                {
                    group.Add(other);
                    visited.Add(other);
                    groupBounds.Encapsulate(other.GetComponent<Collider>().bounds);
                }
            }

            if (group.Count > 1)
                MergeGroup(group, parent, groupIndex++);
        }

        Debug.Log("Building merging complete.");
    }

    private void MergeGroup(List<GameObject> group, Transform parent, int index)
    {
        Dictionary<Material, List<CombineInstance>> materialToMeshes = new();

        foreach (GameObject go in group)
        {
            MeshFilter mf = go.GetComponent<MeshFilter>();
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            if (mf == null || mf.sharedMesh == null || mr == null)
                continue;

            Mesh mesh = mf.sharedMesh;
            Matrix4x4 transformMatrix = mf.transform.localToWorldMatrix;

            Material[] materials = mr.sharedMaterials;

            for (int submeshIndex = 0; submeshIndex < mesh.subMeshCount; submeshIndex++)
            {
                if (submeshIndex >= materials.Length)
                    continue; // skip unmatched material

                Material mat = materials[submeshIndex];
                if (!materialToMeshes.ContainsKey(mat))
                    materialToMeshes[mat] = new List<CombineInstance>();

                CombineInstance ci = new CombineInstance
                {
                    mesh = mesh,
                    subMeshIndex = submeshIndex,
                    transform = transformMatrix
                };

                materialToMeshes[mat].Add(ci);
            }
        }

        // Final GameObject
        //GameObject merged = new GameObject($"MergedBuilding_{index}");
        //merged.transform.parent = parent;

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        // Combine all submeshes (one per material)
        List<CombineInstance> finalCombineList = new();
        List<Material> finalMaterials = new();

        foreach (var kvp in materialToMeshes)
        {
            Mesh subMesh = new Mesh();
            subMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            subMesh.CombineMeshes(kvp.Value.ToArray(), true, true);

            CombineInstance ci = new CombineInstance
            {
                mesh = subMesh,
                transform = Matrix4x4.identity
            };

            finalCombineList.Add(ci);
            finalMaterials.Add(kvp.Key);
        }

        //combinedMesh.CombineMeshes(finalCombineList.ToArray(), false, false);
        //merged.AddComponent<MeshFilter>().sharedMesh = combinedMesh;
        //merged.AddComponent<MeshRenderer>().sharedMaterials = finalMaterials.ToArray();
        //merged.AddComponent<MeshCollider>().sharedMesh = combinedMesh;

        combinedMesh.CombineMeshes(finalCombineList.ToArray(), false, false);

        // Tallenna yhdistetty mesh assetiksi
#if UNITY_EDITOR
        string folderPath = "Assets/MergedMeshes/";
        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets", "MergedMeshes");

        string meshPath = $"{folderPath}MergedBuilding_{index}.asset";
        AssetDatabase.CreateAsset(Object.Instantiate(combinedMesh), meshPath);
        AssetDatabase.SaveAssets();

        combinedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
#endif

        // ? Luo GameObject nyt vasta kun mesh on valmis ja tallennettu
        GameObject merged = new GameObject($"MergedBuilding_{index}");
        merged.transform.parent = parent;

        merged.AddComponent<MeshFilter>().sharedMesh = combinedMesh;
        merged.AddComponent<MeshRenderer>().sharedMaterials = finalMaterials.ToArray();
        merged.AddComponent<MeshCollider>().sharedMesh = combinedMesh;

        // Cleanup
        foreach (GameObject go in group)
            DestroyImmediate(go);

    }
}