using UnityEngine;
using System.Collections;
using UnityEditor;

public class Node : GUIDraggableObject
{
	// TODO: find better name
	public static Node DraggedInputNode;
	public int DraggedInputID = -1;
	public Node[] Inputs;
	public ArrayList UserInputs = new ArrayList();
	public string Name;
	// hmmm ...
	public bool HasOutput;
	public string[] InputNames;

	private string[] InputValues
	{
		get
		{
			string[] values = new string[Inputs.Length];
			for (int i = 0; i < Inputs.Length; i++ )
			{
				if (Inputs[i] != null)
				{
					values[i] = Inputs[i].Content;
				}
				else
				{
					// TODO: handle properly 
					values[i] = "null";
				}
			}
			return values;
		}
	}

	private string[] UserInputValues
	{
		get
		{
			string[] values = new string[UserInputs.Count];
			if (UserInputs.Count > 0)
			{
				for (int i = 0; i < UserInputs.Count; i++ )
				{
					if (UserInputs[i] != null)
					{
						if (UserInputs[i].GetType() == typeof(Color))
						{
							Color c = (Color)UserInputs[i];
							values[i] = "float4({0},{1},{2},{3})";
							string[] rgba = new string[]{c.r.ToString(), c.g.ToString(), c.b.ToString(), c.b.ToString()};
							values[i] = string.Format(values[i], rgba);
						}
						else 
						{
							values[i] = UserInputs[i].ToString();
						}
					}
					else
					{
						// TODO: handle properly 
						values[i] = "null";
					}
				}
			}
			return values;
		}
	}

	public string _content;
	public string Content
	{
		get
		{
			string retVal = _content;
			if (Inputs != null && Inputs.Length > 0) retVal = string.Format(retVal, InputValues);
			if (UserInputs.Count > 0) retVal = string.Format(retVal, UserInputValues);
			return retVal;
		}
		set
		{
			_content = value;
		}
	}

	public Node() : base(Vector2.zero)
	{
		Name = "Default";
		Content = "";
	}

	public Node(Vector2 position) : base(position)
	{
		Name = "Default";
		Content = "";
	}

	public void OnGUI()
	{
		int rows = 1;
		if (Inputs != null) rows = Inputs.Length;
		rows += UserInputs.Count;
		Rect drawRect = new Rect(Position.x, Position.y, 100.0f, 60.0f+rows*20), dragRect;

		GUILayout.BeginArea(drawRect, GUI.skin.GetStyle("Box"));
		GUILayout.Label(Name, GUI.skin.GetStyle("Box"), GUILayout.ExpandWidth(true));

		dragRect = GUILayoutUtility.GetLastRect();
		dragRect = new Rect(dragRect.x + Position.x, dragRect.y + Position.y, dragRect.width, dragRect.height);
		Rect inputRect = new Rect(dragRect.x, dragRect.y + 28, dragRect.width / 2, dragRect.height - 28);

		GUILayout.BeginHorizontal();
		// Inputs
		if (Inputs != null)
		{
			GUILayout.BeginVertical ();
			for (int i = 0; i < Inputs.Length; i++)
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("", GUILayout.Width(20)))
				{
					DraggedInputNode = this;
					DraggedInputID = i;
				}
				GUILayout.Label(InputNames[i] != null ? InputNames[i] : "Missing Label");
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
		}
		if (HasOutput)
		{
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("", GUILayout.Width(20)))
			{
				if (DraggedInputNode != null)
				{
					// TODO: refactor
					DraggedInputNode.Inputs[DraggedInputNode.DraggedInputID] = this;
					DraggedInputNode.DraggedInputID = -1;
					DraggedInputNode = null;
					// Generate. Takes a bit too long to do this all the time
					//EditorWindow.GetWindow<MaterialNodeEditor>().WriteOutput ();
				}
			}
		}
		GUILayout.EndHorizontal();
		if (UserInputs != null)
		{
			//Debug.Log();
			for (int i = 0; i < UserInputs.Count; i++)
			{
				switch (UserInputs[i].GetType().ToString())
				{
				case "UnityEngine.Color":
					UserInputs[i] = EditorGUILayout.ColorField((Color)UserInputs[i], null);
					break;
				case "System.Single":
					UserInputs[i] = EditorGUILayout.FloatField((float)UserInputs[i]);
					break;
				default:
					Debug.Log("no case defined for type " + UserInputs[i].GetType().ToString());
					break;
				}
			}
		}
		GUILayout.EndArea();

		Drag(dragRect);
	}

	public void OnLateGUI ()
	{
		Rect drawRect = new Rect(Position.x, Position.y, 100.0f, 100.0f), dragRect;

		// Draw Bezier curves
		if (Inputs != null)
		{
			for (int i = 0; i < Inputs.Length; i++)
			{
				Vector3 startPosition;
				if (DraggedInputNode == this && i == DraggedInputID)
				{
					startPosition = Event.current.mousePosition;
					_draggingInput = true;
					// stop dragging when user clicks
					if (Event.current.type == EventType.mouseDown)
					{
						DraggedInputNode.DraggedInputID = -1;
						DraggedInputNode = null;
					}
				}
				else if (Inputs[i] != null)
				{
					// ...
					startPosition = Inputs[i].Position + new Vector2 (drawRect.width - 15, 38);
				}
				else continue;
				Vector3 endPosition = Position + new Vector2(15, 28 + i * 20 + 10);
				float distance = (endPosition - startPosition).x * 0.75f;
				distance = Mathf.Abs(distance);
				Vector3 startTangent = startPosition + Vector3.right * distance;
				Vector3 endTangent = endPosition + Vector3.left * distance;
				
				Handles.BeginGUI();
				Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, Color.white, null, 5);
				Handles.EndGUI();
			}
		}
	}
}
