using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

public class FractalMaterialPreEditor : ShaderGUI 
{
	private string TEMPLATE_PATH = Application.dataPath + "/Shader/NoiseShaderTemplate.shader";
	private string OUTPUT_PATH = Application.dataPath + "/Shader/Generated/{0}";
	private string RELATIVE_OUTPUT_PATH = "Assets/Shader/Generated/{0}";
	// TODO: think of a better name
	private const string SHADER_NAME_SYNTAX = "Custom/Generated/{0}";
	private const string PRE_EDITOR = "FractalMaterialPreEditor";
	private const string EDITOR = "FractalMaterialEditor";
	private const string ORIGINAL_SHADER_NAME = "Custom/Noise Shader Template";

	//TODO: see if there is a better callback function
	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
	{
		string content = File.ReadAllText(TEMPLATE_PATH);
		content = content.Replace(ORIGINAL_SHADER_NAME, string.Format(SHADER_NAME_SYNTAX, materialEditor.target.name.Split(' ')[0]));
		content = content.Replace(PRE_EDITOR, EDITOR);
		File.WriteAllText(string.Format(OUTPUT_PATH, materialEditor.target.name.Split(' ')[0] + ".shader"), content);
		AssetDatabase.ImportAsset (string.Format(RELATIVE_OUTPUT_PATH, materialEditor.target.name.Split(' ')[0] + ".shader"));
		Shader shader = Shader.Find(string.Format(SHADER_NAME_SYNTAX, materialEditor.target.name));
		materialEditor.SetShader(shader);
	}
}
