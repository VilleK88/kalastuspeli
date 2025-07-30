using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mapbox.Unity.MeshGeneration.Data;
using System.IO;

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
        FolderCheck(folderPath, cityName, meshFolder, materialFolder, textureFolder);

        // Create a new GameObject to store baked content
        GameObject bakedMap = new GameObject("Map");
        MeshFilter targetMeshFilter = bakedMap.AddComponent<MeshFilter>();
        MeshRenderer targetRenderer = bakedMap.AddComponent<MeshRenderer>();

        /*Shader defaultShader = Shader.Find("Universal Render Pipeline/Lit");
        if (defaultShader == null)
            defaultShader = Shader.Find("Standard"); // fallback
        targetRenderer.sharedMaterial = new Material(defaultShader);

        List<MeshFilter> listMeshFilter = new List<MeshFilter>();*/

        List<Mesh> meshes = new List<Mesh>();
        List<Matrix4x4> transforms = new List<Matrix4x4>();
        List<Vector2[]> uvsList = new List<Vector2[]>();
        List<Texture2D> textures = new List<Texture2D>();

        foreach (Transform tile in originalMap.transform)
        {
            // Skip invalid or non-tile objects
            if (!tile.name.Contains("/")) continue;

            MeshFilter mf = tile.GetComponent<MeshFilter>();
            MeshRenderer mr = tile.GetComponent<MeshRenderer>();
            if (mf == null || mf.sharedMesh == null || mr == null || mr.sharedMaterial == null)
                continue;

            Texture2D tex = mr.sharedMaterial.mainTexture as Texture2D;
            if (tex == null)
                continue;

            meshes.Add(Instantiate(mf.sharedMesh));
            transforms.Add(mf.transform.localToWorldMatrix);
            uvsList.Add(mf.sharedMesh.uv);
            textures.Add(tex);
        }

        if (meshes.Count == 0)
        {
            Debug.LogWarning("No valid tile meshes with textures found.");
            return;
        }

        // Pack textures into an atlas
        Texture2D atlas = new Texture2D(1, 1);
        Rect[] uvRects = atlas.PackTextures(textures.ToArray(), 2, 8192, false);

        // Save the atlas
        string atlasPath = $"{textureFolder}/CombinedAtlas.png";
        File.WriteAllBytes(atlasPath, atlas.EncodeToPNG());
        AssetDatabase.ImportAsset(atlasPath);
        Texture2D savedAtlas = AssetDatabase.LoadAssetAtPath<Texture2D>(atlasPath);


        // Create new material
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        Material combinedMat = new Material(shader);
        combinedMat.SetTexture("_BaseMap", savedAtlas);
        if (combinedMat.HasProperty("_MainTex"))
            combinedMat.mainTexture = savedAtlas;

        string materialPath = $"{materialFolder}/CombinedMaterial.mat";
        AssetDatabase.CreateAsset(combinedMat, materialPath);
        AssetDatabase.SaveAssets();

        // Build combined mesh
        List<CombineInstance> combineInstances = new List<CombineInstance>();
        Mesh finalMesh = new Mesh();
        finalMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        for (int i = 0; i < meshes.Count; i++)
        {
            Mesh m = meshes[i];
            Vector2[] originalUV = uvsList[i];
            Rect rect = uvRects[i];
            Vector2[] remappedUV = new Vector2[originalUV.Length];

            for (int j = 0; j < originalUV.Length; j++)
            {
                remappedUV[j] = new Vector2(
                    Mathf.Lerp(rect.xMin, rect.xMax, originalUV[j].x),
                    Mathf.Lerp(rect.yMin, rect.yMax, originalUV[j].y)
                    );
            }

            m.uv = remappedUV;

            combineInstances.Add(new CombineInstance
            {
                mesh = m,
                transform = transforms[i]
            });
        }

        finalMesh.CombineMeshes(combineInstances.ToArray(), true, true);

        string meshAssetPath = AssetDatabase.GenerateUniqueAssetPath($"{meshFolder}/CombinedMapMesh.asset");
        AssetDatabase.CreateAsset(finalMesh, meshAssetPath);
        AssetDatabase.SaveAssets();

        targetMeshFilter.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshAssetPath);
        targetRenderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

        MeshCollider mc = bakedMap.AddComponent<MeshCollider>();
        mc.sharedMesh = finalMesh;

        string prefabPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/Map.prefab");
        PrefabUtility.SaveAsPrefabAsset(bakedMap, prefabPath);
        DestroyImmediate(bakedMap);

        Debug.Log($"Map '{cityName}' baked with atlas and saved at: {prefabPath}");

        // old code
        /*// Loop over all tiles in the original map
        foreach(Transform tile in originalMap.transform)
        {
            // Skip invalid or non-tile objects
            if (!tile.name.Contains("/")) continue;

            // Duplicate the tile
            GameObject tileCopy = DuplicateTile(tile, bakedMap);

            // Process all child objects (e.g., buildings, roads)
            foreach (Transform child in tileCopy.GetComponentsInChildren<Transform>())
            {
                // Process meshes
                ProcessMeshes(child, tileCopy, meshFolder);

                // Process materials
                ProcessMaterials(child, tileCopy, materialFolder, textureFolder);
            }

            // Remove UnityTile component (Mapbox-related)
            var unityTile = tileCopy.GetComponent<UnityTile>();
            if(unityTile != null)
                DestroyImmediate(unityTile);

            // Remove any other leftover MonoBehaviours
            foreach (var comp in tileCopy.GetComponents<MonoBehaviour>())
            {
                if(comp != null)
                    DestroyImmediate(comp);
            }
        }

        // Save the result as a prefab
        string localPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/Map.prefab");
        PrefabUtility.SaveAsPrefabAsset(bakedMap, localPath);

        // Clean up
        DestroyImmediate(bakedMap);

        Debug.Log($"Map for '{cityName}' baked and saved to: {localPath}");*/
    }

    static void ProcessMeshes(Transform child, GameObject tileCopy, string meshFolder)
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
    }

    static void ProcessMaterials(Transform child, GameObject tileCopy, string materialFolder, string textureFolder)
    {
        MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.sharedMaterials.Length > 0)
        {
            var originalMaterials = meshRenderer.sharedMaterials;
            Material[] newMaterials = new Material[originalMaterials.Length];

            for (int i = 0; i < originalMaterials.Length; i++)
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

    static void FolderCheck(string folderPath, string cityName, string meshFolder, string materialFolder, string textureFolder)
    {
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
    }

    static GameObject DuplicateTile(Transform tile, GameObject bakedMap)
    {
        GameObject tileCopy = Instantiate(tile.gameObject);
        tileCopy.name = tile.name;
        tileCopy.hideFlags = HideFlags.None;
        tileCopy.transform.SetParent(bakedMap.transform);
        return tileCopy;
    }
}