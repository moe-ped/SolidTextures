using UnityEngine;
using System.Collections;
using UnityEditor;

public abstract class Node : GUIDraggableObject
{
	// TODO: find better name
	public static Node DraggedInputNode;
	public int DraggedInputID = -1;

	public Node[] Inputs;

	protected string Name;
	// hmmm ...
	protected bool HasOutput;
	protected string[] InputNames;

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

	private string _content;
	public string Content
	{
		get
		{
			if (Inputs == null) return _content;
			return string.Format(_content, InputValues);
		}
		set
		{
			_content = value;
		}
	}

	public Node(Vector2 position) : base(position)
	{
		Name = "Default";
		Content = "";
	}

	public void OnGUI()
	{
		Rect drawRect = new Rect(Position.x, Position.y, 100.0f, 100.0f), dragRect;

		GUILayout.BeginArea(drawRect, GUI.skin.GetStyle("Box"));
		GUILayout.Label(Name, GUI.skin.GetStyle("Box"), GUILayout.ExpandWidth(true));

		dragRect = GUILayoutUtility.GetLastRect();
		dragRect = new Rect(dragRect.x + Position.x, dragRect.y + Position.y, dragRect.width, dragRect.height);
		Rect inputRect = new Rect(dragRect.x, dragRect.y + 28, dragRect.width / 2, dragRect.height - 28);

		GUILayout.BeginHorizontal();
		// Inputs
		if (Inputs != null)
		{
			for (int i = 0; i < Inputs.Length; i++)
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("", GUILayout.Width(20)))
				{
					DraggedInputNode = this;
					DraggedInputID = i;
				}
				GUILayout.Label(InputNames[i]);
				GUILayout.EndHorizontal();
			}
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
				}
			}
		}
		GUILayout.EndHorizontal();

		if (GUILayout.Button("Yes!"))
		{
			Debug.Log("Yes. It is " + Content + "!");
		}
		GUILayout.EndArea();

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

		Drag(dragRect);
	}
}
