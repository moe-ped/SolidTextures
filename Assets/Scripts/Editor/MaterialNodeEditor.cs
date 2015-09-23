using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using LitJson;

public struct Double2
{
	public double x;
	public double y;
	
	public Double2 (double xCoord, double yCoord)
	{
		x = xCoord;
		y = yCoord;
	}
}

public struct Double4
{
	public double r;
	public double g;
	public double b;
	public double a;
	
	public Double4 (double rValue, double gValue, double bValue, double aValue)
	{
		r = rValue;
		g = gValue;
		b = bValue;
		a = aValue;
	}
}

public struct SerializableNode
{
	public string Name;
	public string Content;
	public Double2 Position;
	public List<int> Inputs;
	public List<string> InputNames;
	public List<double> FloatUserInputs;
	public List<Double2> Vector2UserInputs;
	public List<Double4> ColorUserInputs;
	public bool HasOutput;
}

public class MaterialNodeEditor : EditorWindow
{
	private List<Node> Nodes = new List<Node>();
	private bool doRepaint = false;
	private Rect dropTargetRect = new Rect(10.0f, 300.0f, 300.0f, 100.0f);

	private Vector2 scrollPosition = new Vector2 (1000, 1000);
	private string SelectedmaterialName = "";

	private string TEMPLATE_PATH = Application.dataPath + "/Shader/FractalTest 3.shader";
	private string OUTPUT_PATH = Application.dataPath + "/Shader/Generated/{0}";
	private string WINDOW_SAVESTATE_PATH = Application.dataPath + "/Shader/Generated/{0}";
	private string RELATIVE_OUTPUT_PATH = "Assets/Shader/Generated/{0}";
	// TODO: think of a better name
	private const string SHADER_NAME_SYNTAX = "Custom/{0}";
	private const string PRE_EDITOR = "FractalMaterialPreEditor";
	private const string EDITOR = "FractalMaterialEditor";
	private const string ORIGINAL_SHADER_NAME = "FractalTest 3";

	[MenuItem("Window/MaterialNodeEditor")]
	static void Init()
	{
		GetWindow<MaterialNodeEditor>();
	}

	private void Reset ()
	{
		Nodes = new List<Node> ();
		if (GetSelectedMaterialName () != "") 
		{
			Nodes.Add(new OutputNode(new Vector2(1200, 1200)));
		}
	}

	public void OnEnable()
	{
		Reset ();
		LoadState ();
	}

	void OnSelectionChange ()
	{
		// Save changes
		SaveState ();
		// Load new Material
		SelectedmaterialName = GetSelectedMaterialName ();
		LoadState ();
		// Repaint GUI
		Repaint();
	}

	void Update ()
	{
		if (doRepaint)
		{
			Repaint();
		}
	}

	void OnGUI()
	{
		string label = GetSelectedMaterialName();
		GUILayout.Label(GetSelectedMaterialName() != "" ? "You are now editing " + label : "Select a material to edit");

		// Test
		Node toFront, dropDead;
		bool previousState, flipRepaint;
		Color color;

		GUILayout.BeginHorizontal ();
		// Test
		if (GUILayout.Button ("Generate"))
		{
			WriteOutput ();
		}

		if (GUILayout.Button ("Reset"))
		{
			Reset();
		}
		GUILayout.EndHorizontal ();

		Rect windowRect = this.position;
		scrollPosition = GUI.BeginScrollView (new Rect(0, 40, windowRect.width, windowRect.height-40), scrollPosition, new Rect(0, 0, 2000, 2000));
		dropTargetRect = new Rect (0, 0, 50, 50);
		dropTargetRect.x += scrollPosition.x + this.position.width - 80;
		dropTargetRect.y += scrollPosition.y + this.position.height - 120;
		GUI.Box(dropTargetRect, "Recycle");
		toFront = dropDead = null;
		doRepaint = false;
		flipRepaint = false;
		foreach (Node data in Nodes)
		{
			previousState = data.Dragging;

			color = GUI.color;

			if (previousState)
			{
				GUI.color = dropTargetRect.Contains(Event.current.mousePosition) ? Color.red : color;
			}

			data.OnGUI();

			GUI.color = color;

			if (data.ForceRepaint)
			{
				doRepaint = true;

				if (Nodes.IndexOf(data) != Nodes.Count - 1)
				{
					toFront = data;
				}
			}
			else if (previousState)
			{
				flipRepaint = true;

				if (dropTargetRect.Contains(Event.current.mousePosition))
				{
					dropDead = data;
				}
			}
		}

		foreach (Node data in Nodes)
		{
			data.OnLateGUI ();
		}

		if (toFront != null)
		// Move an object to front if needed
		{
			//Nodes.Remove(toFront);
			//Nodes.Add(toFront);
		}

		if (dropDead != null)
		// Destroy an object if needed
		{
			if (dropDead.Name != "Output")
			{
				Nodes.Remove(dropDead);
			}
			else 
			{
				Debug.LogError ("I'm afraid I can't let you do that.");
			}
		}

		if (flipRepaint)
		// If some object just stopped being dragged, we should repaint for the state change
		{
			Repaint();
		}

		GUI.EndScrollView ();

		Event evt = Event.current;
		if (evt.type == EventType.ContextClick)
		{
			Vector2 mousePos = evt.mousePosition+scrollPosition+Vector2.down*40;
			// Now create the menu, add items and show it
			GenericMenu menu = new GenericMenu ();

			menu.AddItem (new GUIContent ("New Variable/Color"), false, () => {Nodes.Add(new ColorNode(mousePos));});
			menu.AddItem (new GUIContent ("New Variable/Float"), false, () => {Nodes.Add(new FloatNode(mousePos));});
			menu.AddItem (new GUIContent ("New Function/Lerp"), false, () => {Nodes.Add(new LerpNode(mousePos));});
			menu.AddItem (new GUIContent ("New Function/Multiply"), false, () => {Nodes.Add(new MultiplyNode(mousePos));});
			menu.AddItem (new GUIContent ("New Function/Sawtooth"), false, () => {Nodes.Add(new SawNode(mousePos));});
			menu.AddItem (new GUIContent ("New Noise/Perlin"), false, () => {Nodes.Add(new PerlinNode(mousePos));});
			
			menu.ShowAsContext ();
			
			evt.Use();
		}
	}
	
	private string GetSelectedMaterialName ()
	{
		GameObject activeGameObject = Selection.activeGameObject;
		if (activeGameObject != null)
		{
			MeshRenderer renderer = activeGameObject.GetComponent<MeshRenderer>();
			if (renderer != null)
			{
				// Material's name minus that "(Instance)" stuff
				return renderer.sharedMaterial.name.Split(' ')[0];
			}
		}
		Object[] materials = Selection.GetFiltered(typeof(Material), SelectionMode.Assets);
		if (materials.Length > 0)
		{
			return materials[0].name;
		}
		return "";
	}

	public void WriteOutput ()
	{
		string generatedContent = Nodes [0].Content;
		string actualOutputPath = string.Format(OUTPUT_PATH, GetSelectedMaterialName()) + ".shader";
		string actualRelativeOutputPath = string.Format (RELATIVE_OUTPUT_PATH, GetSelectedMaterialName()) + ".shader";
		string content = File.ReadAllText(actualOutputPath);
		content = Regex.Replace(content, @"(/\*Generated\*/)"+"(.*)"+@"(/\*EndGenerated\*/)", "$1"+generatedContent+"$3", RegexOptions.Singleline);
		File.WriteAllText(actualOutputPath, content);
		// Compile shader
		AssetDatabase.ImportAsset (actualRelativeOutputPath);
	}

	private void SaveState ()
	{
		List<SerializableNode> serializableNodes = new List<SerializableNode> ();
		foreach (Node node in Nodes) 
		{
			SerializableNode newNode = new SerializableNode();
			newNode.Name = node.Name;
			newNode.Position = new Double2 (node.Position.x, node.Position.y);
			newNode.HasOutput = node.HasOutput;
			newNode.Content = node._content;

			newNode.Inputs = new List<int>();
			newNode.InputNames = new List<string>();
			if (node.Inputs != null)
			{
				int i=0;
				foreach (Node input in node.Inputs)
				{
					newNode.InputNames.Add (node.InputNames[i]);
					if (input != null)
					{
						newNode.Inputs.Add(Nodes.IndexOf(input));
					}
					else
					{
						newNode.Inputs.Add(-1);
					}
					i++;
				}
			}

			newNode.FloatUserInputs = new List<double>();
			newNode.Vector2UserInputs = new List<Double2>();
			newNode.ColorUserInputs = new List<Double4>();
			foreach (object userInput in node.UserInputs)
			{
				if (userInput.GetType() == typeof(float))
				{
					double value = (float)userInput;
					newNode.FloatUserInputs.Add(value);
				}
				if (userInput.GetType() == typeof(Vector2))
				{
					Double2 value = new Double2(((Vector2)userInput).x, ((Vector2)userInput).y);
					newNode.Vector2UserInputs.Add(value);
				}
				if (userInput.GetType() == typeof(Color))
				{
					Double4 value = new Double4(((Color)userInput).r, ((Color)userInput).g, ((Color)userInput).b, ((Color)userInput).a);
					newNode.ColorUserInputs.Add(value);
				}
			}
			serializableNodes.Add(newNode);
		}
		string json = JsonMapper.ToJson (serializableNodes);
		string actualSavePath = string.Format(WINDOW_SAVESTATE_PATH, SelectedmaterialName) + ".txt";
		File.WriteAllText (actualSavePath, json);
	}

	private void LoadState ()
	{
		if (GetSelectedMaterialName () == "") 
		{
			Reset ();
			return;
		}
		string actualSavePath = string.Format(WINDOW_SAVESTATE_PATH, SelectedmaterialName) + ".txt";
		if (!File.Exists(actualSavePath))
		{
			Reset ();
			SaveState ();
		}
		string json = File.ReadAllText (actualSavePath);
		Nodes = new List<Node> ();
		List<SerializableNode> nodes = JsonMapper.ToObject<List<SerializableNode>> (json);
		foreach (SerializableNode node in nodes)
		{
			Node newNode = new Node();
			newNode.Name = node.Name;
			newNode.Position = new Vector2 ((float)node.Position.x, (float)node.Position.y);
			newNode.HasOutput = node.HasOutput;
			newNode._content = node.Content;

			newNode.UserInputs = new ArrayList();
			foreach (double floatUserInput in node.FloatUserInputs)
			{
				float value = (float)floatUserInput;
				newNode.UserInputs.Add(value);
			}
			foreach (Double2 vector2UserInput in node.Vector2UserInputs)
			{
				Vector2 value = new Vector2 ((float)vector2UserInput.x, (float)vector2UserInput.y);
				newNode.UserInputs.Add(value);
			}
			foreach (Double4 colorUserInput in node.ColorUserInputs)
			{
				Color value =  new Color((float)colorUserInput.r, (float)colorUserInput.g, (float)colorUserInput.b, (float)colorUserInput.a);
				newNode.UserInputs.Add(value);
			}
			Nodes.Add(newNode);
		}

		int n = 0;
		foreach (SerializableNode node in nodes)
		{
			Nodes[n].Inputs = new Node[node.Inputs.Count];
			Nodes[n].InputNames = new string[node.Inputs.Count];
			int i=0;
			foreach (int input in node.Inputs)
			{
				Nodes[n].InputNames[i] = node.InputNames[i] != null ? node.InputNames[i] : "Missing. Lol";
				if (input > -1)
				{
					Nodes[n].Inputs[i] = Nodes[input];
				}
				else
				{
					Nodes[n].Inputs[i] = null;
				}
				i++;
			}
			n++;
		}
	}
}