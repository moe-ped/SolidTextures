using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

[System.Serializable]
public class GradientContainer 
{
	public Gradient Gradient;
}

public class FractalMaterialEditor : ShaderGUI
{
	[SerializeField]
	public GradientContainer gradientContainer;

	private string TEMPLATE_PATH = Application.dataPath + "/Shader/FractalTest 3.shader";
	private string OUTPUT_PATH = Application.dataPath + "/Shader/Generated/{0}";
	// TODO: think of a better name
	private const string SHADER_NAME_SYNTAX = "Custom/{0}";
	private const string PRE_EDITOR = "FractalMaterialPreEditor";
	private const string EDITOR = "FractalMaterialEditor";
	private const string ORIGINAL_SHADER_NAME = "FractalTest 3";

	private string customShaderPart;

	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) 
	{
		EditorGUILayout.LabelField("lol");
		// TODO: find a way to do stuff like this once (reading the file, finding lines ...)
		string content = File.ReadAllText(string.Format(OUTPUT_PATH, materialEditor.target.name + ".shader"));
		content = content.Replace(ORIGINAL_SHADER_NAME, materialEditor.target.name);
		content = content.Replace(PRE_EDITOR, EDITOR);
		string generatedContent = Regex.Match(content, "(// Generated)(.*)(// EndGenerated)", RegexOptions.Singleline).Value;
		generatedContent = Regex.Replace(generatedContent, "(// Generated)[\r\n]*([^\r\n].*[^\r\n])[\r\n]*(// EndGenerated)", "$2", RegexOptions.Singleline);
		generatedContent = EditorGUILayout.TextArea(generatedContent);
		content = Regex.Replace(content, "(// Generated)(.*)(// EndGenerated)", "$1\n" + generatedContent + "\n$3", RegexOptions.Singleline);
		/*customShaderPart = EditorGUILayout.TextArea(lines[GeneratedLine]);
		lines[GeneratedLine] = customShaderPart;*/
		File.WriteAllText(string.Format(OUTPUT_PATH, materialEditor.target.name + ".shader"), content);
	}
}
