using UnityEngine;
using UnityEditor;
using Mapbox.Unity.MeshGeneration.Data;

public class MapTileBaker : MonoBehaviour
{
    [MenuItem("Tools/Mapbox/Bake Map Tiles")]
    public static void BakeTiles(string cityName)
    {
        GameObject originalMap = GameObject.Find("Map");
        if(originalMap == null)
        {
            Debug.LogError("Map object not found.");
            return;
        }

        string folderPath = $"Assets/MapPrefabs/{cityName}";
        string meshFolder = $"{folderPath}/Meshes";
        string materialFolder = $"{folderPath}/Materials";
        string textureFolder = $"{folderPath}/Textures";

        if (!AssetDatabase.IsValidFolder("Assets/MapPrefabs"))
            AssetDatabase.CreateFolder("Assets", "MapPrefabs");
        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets/MapPrefabs", cityName);
        if (!AssetDatabase.IsValidFolder(meshFolder))
            AssetDatabase.CreateFolder(folderPath, "Meshes");
        if (!AssetDatabase.IsValidFolder(materialFolder))
            AssetDatabase.CreateFolder(folderPath, "Materials");
        if (!AssetDatabase.IsValidFolder(textureFolder))
            AssetDatabase.CreateFolder(folderPath, "Textures");

        GameObject bakedMap = new GameObject("Map");

        foreach(Transform tile in originalMap.transform)
        {
            if (!tile.name.Contains("/")) continue;

            GameObject tileCopy = Object.Instantiate(tile.gameObject);
            tileCopy.name = tile.name;
            tileCopy.hideFlags = HideFlags.None;
            tileCopy.transform.SetParent(bakedMap.transform);

            foreach(Transform child in tileCopy.GetComponentsInChildren<Transform>())
            {
                MeshFilter meshFilter = child.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    string safeName = $"{tileCopy.name}_{child.name}".Replace("/", "_");
                    string meshPath = $"{meshFolder}/{safeName}_Mesh.asset";
                    meshPath = AssetDatabase.GenerateUniqueAssetPath(meshPath);

                    Mesh mesh = Object.Instantiate(meshFilter.sharedMesh);
                    AssetDatabase.CreateAsset(mesh, meshPath);
                    meshFilter.sharedMesh = mesh;

                    MeshCollider meshCollider = child.GetComponent<MeshCollider>();
                    if (meshCollider == null)
                        meshCollider = child.gameObject.AddComponent<MeshCollider>();
                    
                    meshCollider.sharedMesh = mesh;
                }

                MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                if (meshRenderer != null && meshRenderer.sharedMaterials.Length > 0)
                {
                    var originalMaterials = meshRenderer.sharedMaterials;
                    Material[] newMaterials = new Material[originalMaterials.Length];

                    for(int i = 0; i < originalMaterials.Length; i++)
                    {
                        var originalMat = originalMaterials[i];
                        if (originalMat == null)
                            continue;

                        string safeName = $"{tileCopy.name}_{child.name}_{i}".Replace("/", "_");
                        string materialPath = AssetDatabase.GenerateUniqueAssetPath($"{materialFolder}/{safeName}_Material.mat");

                        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                        Material matCopy = new Material(shader);
                        matCopy.CopyPropertiesFromMaterial(originalMat);

                        if (matCopy.HasProperty("_BaseColor"))
                            matCopy.SetColor("_BaseColor", originalMat.color);

                        Texture mainTex = originalMat.mainTexture;
                        if (mainTex is Texture2D tex2D)
                        {
                            string texturePath = $"{textureFolder}/{safeName}_MainTex.png";

                            byte[] pngData = tex2D.EncodeToPNG();
                            if (pngData != null)
                            {
                                System.IO.File.WriteAllBytes(texturePath, pngData);
                                AssetDatabase.ImportAsset(texturePath);

                                Texture2D importedTex = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                                if (importedTex != null && matCopy.HasProperty("_BaseMap"))
                                    matCopy.SetTexture("_BaseMap", importedTex);
                            }
                        }

                        AssetDatabase.CreateAsset(matCopy, materialPath);
                        newMaterials[i] = matCopy;
                    }

                    meshRenderer.sharedMaterials = newMaterials;
                }
            }

            var unityTile = tileCopy.GetComponent<UnityTile>();
            if(unityTile != null)
            {
                Object.DestroyImmediate(unityTile);
            }

            foreach(var comp in tileCopy.GetComponents<MonoBehaviour>())
            {
                if(comp != null)
                {
                    Object.DestroyImmediate(comp);
                }
            }
        }

        // Save as prefab
        string localPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/Map.prefab");
        PrefabUtility.SaveAsPrefabAsset(bakedMap, localPath);

        DestroyImmediate(bakedMap);

        Debug.Log($"Map for '{cityName}' baked and saved to: {localPath}");
    }
}