using UnityEngine;
using System.Collections;

public class OutputNode : Node
{
	public OutputNode(Vector2 position)
		: base(position)
	{
		Name = "Output";
		Content = "c={0};";
		Inputs = new Node[1];
		InputNames = new string[] { "Diffuse" };
		HasOutput = false;	//< lol
	}
}

public class ColorNode : Node
{
	public ColorNode(Vector2 position)
		: base(position)
	{
		Name = "Color";
		// Test
		Content = "float4(1,1,1,1)";
		HasOutput = true;
	}
}
