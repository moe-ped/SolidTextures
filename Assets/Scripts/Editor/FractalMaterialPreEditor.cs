using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

public class FractalMaterialPreEditor : ShaderGUI 
{
	private string TEMPLATE_PATH = Application.dataPath + "/Shader/FractalTest 3.shader";
	private string OUTPUT_PATH = Application.dataPath + "/Shader/Generated/{0}";
	// TODO: think of a better name
	private const string SHADER_NAME_SYNTAX = "Custom/{0}";
	private const string PRE_EDITOR = "FractalMaterialPreEditor";
	private const string EDITOR = "FractalMaterialEditor";
	private const string ORIGINAL_SHADER_NAME = "FractalTest 3";

	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
	{
		// Test
		if (GUILayout.Button("Generate"))
		{
			string content = File.ReadAllText(TEMPLATE_PATH);
			content = content.Replace(ORIGINAL_SHADER_NAME, materialEditor.target.name);
			content = content.Replace(PRE_EDITOR, EDITOR);
			File.WriteAllText(string.Format(OUTPUT_PATH, materialEditor.target.name.Split(' ')[0] + ".shader"), content);
			Shader shader = Shader.Find(string.Format(SHADER_NAME_SYNTAX, materialEditor.target.name));
			materialEditor.SetShader(shader);
		}
	}
}
