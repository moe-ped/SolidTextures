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
		UserInputs.Add (Color.white);
		Content = "{0}";
		HasOutput = true;
	}
}

public class FloatNode : Node
{
	public FloatNode(Vector2 position)
		: base(position)
	{
		Name = "Float";
		UserInputs.Add (1.0f);
		Content = "{0}";
		HasOutput = true;
	}
}

public class PerlinNode : Node
{
	public PerlinNode(Vector2 position)
		: base(position)
	{
		Name = "Perlin";
		Inputs = new Node[3];
		InputNames = new string[] { "X Scale", "Y Scale", "Z Scale" };
		Content = "perlin3d({0}*x,{1}*y,{2}*z)";
		HasOutput = true;
	}
}

public class LerpNode : Node
{
	public LerpNode(Vector2 position)
		: base(position)
	{
		Name = "Lerp";
		Inputs = new Node[3];
		InputNames = new string[] { "Input 1", "Input 2", "Factor" };
		Content = "lerp({0},{1},{2})";
		HasOutput = true;
	}
}

public class MultiplyNode : Node
{
	public MultiplyNode(Vector2 position)
		: base(position)
	{
		Name = "Multiply";
		Inputs = new Node[2];
		InputNames = new string[] { "x", "y" };
		Content = "{0}*{1}";
		HasOutput = true;
	}
}

public class SawNode : Node
{
	public SawNode(Vector2 position)
		: base(position)
	{
		Name = "Saw";
		Inputs = new Node[1];
		InputNames = new string[] { "x" };
		Content = "saw({0})";
		HasOutput = true;
	}
}
