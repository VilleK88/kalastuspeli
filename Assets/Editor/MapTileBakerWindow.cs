using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class MapTileBakerWindow : EditorWindow
{
    private string cityName = "kaupunki";

    [MenuItem("Tools/Mapbox/Bake Map Tiles (Custom City)")]
    public static void ShowWindow()
    {
        GetWindow<MapTileBakerWindow>("Map Tile Baker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Map Tile Baker", EditorStyles.boldLabel);
        cityName = EditorGUILayout.TextField("City Name", cityName);

        if(GUILayout.Button("Bake Tiles"))
        {
            if(string.IsNullOrEmpty(cityName.Trim()))
            {
                EditorUtility.DisplayDialog("Error", "City name cannot be empty.", "OK");
                return;
            }

            MapTileBaker.BakeTiles(cityName);
        }
    }
}