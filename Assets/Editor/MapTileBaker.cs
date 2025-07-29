using UnityEngine;
using UnityEditor;
using Mapbox.Unity.MeshGeneration.Data;

public class MapTileBaker : MonoBehaviour
{
    [MenuItem("Tools/Mapbox/Bake Map Tiles")]
    public static void BakeTiles(string cityName)
    {
        // Find the map root object in the scene
        GameObject originalMap = GameObject.Find("Map");
        if(originalMap == null)
        {
            Debug.LogError("Map object not found.");
            return;
        }

        // Prepare directory paths for saving baked assets
        string folderPath = $"Assets/MapPrefabs/{cityName}";
        string meshFolder = $"{folderPath}/Meshes";
        string materialFolder = $"{folderPath}/Materials";
        string textureFolder = $"{folderPath}/Textures";

        // Ensure folders exist or create them
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

        // Create a new GameObject to store baked content
        GameObject bakedMap = new GameObject("Map");

        // Loop over all tiles in the original map
        foreach(Transform tile in originalMap.transform)
        {
            // Skip invalid or non-tile objects
            if (!tile.name.Contains("/")) continue;

            // Duplicate the tile
            GameObject tileCopy = Instantiate(tile.gameObject);
            tileCopy.name = tile.name;
            tileCopy.hideFlags = HideFlags.None;
            tileCopy.transform.SetParent(bakedMap.transform);

            // Process all child objects (e.g., buildings, roads)
            foreach(Transform child in tileCopy.GetComponentsInChildren<Transform>())
            {
                // Process meshes
                MeshFilter meshFilter = child.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    // Create a safe name and path for mesh asset
                    string safeName = $"{tileCopy.name}_{child.name}".Replace("/", "_");
                    string meshPath = $"{meshFolder}/{safeName}_Mesh.asset";
                    meshPath = AssetDatabase.GenerateUniqueAssetPath(meshPath);

                    // Duplicate and save mesh
                    Mesh mesh = Instantiate(meshFilter.sharedMesh);
                    AssetDatabase.CreateAsset(mesh, meshPath);
                    meshFilter.sharedMesh = mesh;

                    // Ensure a MeshCollider is added and uses the same mesh
                    MeshCollider meshCollider = child.GetComponent<MeshCollider>();
                    if (meshCollider == null)
                        meshCollider = child.gameObject.AddComponent<MeshCollider>();
                    
                    meshCollider.sharedMesh = mesh;
                }

                // Process materials
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

                        // Safe material file name
                        string safeName = $"{tileCopy.name}_{child.name}_{i}".Replace("/", "_");
                        string materialPath = AssetDatabase.GenerateUniqueAssetPath($"{materialFolder}/{safeName}_Material.mat");

                        // Create a new material based on URP Lit
                        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                        Material matCopy = new Material(shader);
                        matCopy.CopyPropertiesFromMaterial(originalMat);

                        // Optional: Set base color if supported
                        if (matCopy.HasProperty("_BaseColor"))
                            matCopy.SetColor("_BaseColor", originalMat.color);

                        // Handle main texture saving
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

                    // Assign the new material array
                    meshRenderer.sharedMaterials = newMaterials;
                }
            }

            // Remove UnityTile component (Mapbox-related)
            var unityTile = tileCopy.GetComponent<UnityTile>();
            if(unityTile != null)
            {
                DestroyImmediate(unityTile);
            }

            // Remove any other leftover MonoBehaviours
            foreach (var comp in tileCopy.GetComponents<MonoBehaviour>())
            {
                if(comp != null)
                {
                    DestroyImmediate(comp);
                }
            }
        }

        // Save the result as a prefab
        string localPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/Map.prefab");
        PrefabUtility.SaveAsPrefabAsset(bakedMap, localPath);

        // Clean up
        DestroyImmediate(bakedMap);

        Debug.Log($"Map for '{cityName}' baked and saved to: {localPath}");
    }
}