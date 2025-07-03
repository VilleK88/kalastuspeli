using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshCombiner))]
public class MeshCombinerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MeshCombiner combiner = (MeshCombiner)target;
        if (GUILayout.Button("Combine Meshes"))
            combiner.CombineMeshes();
    }
}