using UnityEngine;
using UnityEditor;

public struct NestedStruct
{
	private float m_StructFloat;
	public void OnGUI()
	{
		m_StructFloat = EditorGUILayout.FloatField("Struct Float", m_StructFloat);
	}
}

[System.Serializable]
public class SerializeMe
{
	[SerializeField]
	private string m_Name;
	[SerializeField]
	private int m_Value;

	[SerializeField]
	private NestedStruct m_Struct;

	public SerializeMe()
	{
		m_Struct = new NestedStruct();
		m_Name = "";
	}

	public void OnGUI()
	{
		m_Name = EditorGUILayout.TextField("Name", m_Name);
		m_Value = EditorGUILayout.IntSlider("Value", m_Value, 0, 10);
		m_Struct.OnGUI();
	}
}

public class MyWindow : EditorWindow
{
	private SerializeMe m_SerialziedThing;

	[MenuItem("Window/Serialization")]
	static void Init()
	{
		GetWindow<MyWindow>();
	}

	void OnEnable()
	{
		hideFlags = HideFlags.HideAndDontSave;
		if (m_SerialziedThing == null)
			m_SerialziedThing = new SerializeMe();
	}

	void OnGUI()
	{
		GUILayout.Label("Serialized Things", EditorStyles.boldLabel);
		m_SerialziedThing.OnGUI();
		Handles.BeginGUI();
		Handles.DrawBezier(new Vector3(50, 50, 0), new Vector3(200, 100, 0), new Vector3(1, 0, 0), new Vector3(-1, 0, 0), Color.white, null, 6);
		Handles.EndGUI();
	}
}