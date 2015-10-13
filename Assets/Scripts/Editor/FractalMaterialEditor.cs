using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

public class FractalMaterialEditor : ShaderGUI
{
	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) 
	{
		if (GUILayout.Button("Edit"))
		{
			EditorWindow.GetWindow<MaterialNodeEditor>();
		}
	}
}
