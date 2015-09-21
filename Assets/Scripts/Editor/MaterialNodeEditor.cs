using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MaterialNodeEditor : EditorWindow
{
	private List<Node> Nodes = new List<Node>();
	private bool doRepaint = false;
	private Rect dropTargetRect = new Rect(10.0f, 10.0f, 30.0f, 30.0f);

	[MenuItem("Window/MaterialNodeEditor")]
	static void Init()
	{
		GetWindow<MaterialNodeEditor>();
	}

	public void OnEnable()
	{
		Nodes.Add(new OutputNode(new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));
		Nodes.Add(new ColorNode(new Vector2(20.0f * Random.Range(1.0f, 10.0f), 20.0f * Random.Range(1.0f, 10.0f))));
	}


	void OnSelectionChange ()
	{
		// Repaint GUI
		Repaint();
		// Save changes
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
		GUILayout.Label(label);

		// Test
		Node toFront, dropDead;
		bool previousState, flipRepaint;
		Color color;

		GUI.Box(dropTargetRect, "Die");

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

		if (toFront != null)
		// Move an object to front if needed
		{
			Nodes.Remove(toFront);
			Nodes.Add(toFront);
		}

		if (dropDead != null)
		// Destroy an object if needed
		{
			Nodes.Remove(dropDead);
		}

		if (flipRepaint)
		// If some object just stopped being dragged, we should repaing for the state change
		{
			Repaint();
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
				return renderer.material.name.Split(' ')[0];
			}
		}
		Object[] materials = Selection.GetFiltered(typeof(Material), SelectionMode.Assets);
		if (materials.Length > 0)
		{
			return materials[0].name;
		}
		return "";
	}
}