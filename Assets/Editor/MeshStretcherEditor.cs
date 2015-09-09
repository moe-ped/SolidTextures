using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MeshStretcher))]
public class MeshStretcherEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MeshStretcher Stretcher = (MeshStretcher)target;
        if (GUILayout.Button("Stretch"))
        {
            Stretcher.Stretch();
        }
        if (GUILayout.Button("Reset"))
        {
            Stretcher.Reset();
        }
    }
}
