using UnityEngine;
using System.Collections;

public class GUIDraggableObject
{
	public Vector2 Position;
	private Vector2 DragStart;
	private bool _dragging;
	protected bool _draggingInput;


	public GUIDraggableObject(Vector2 position)
	{
		Position = position;
	}

	public bool Dragging
	{
		get
		{
			return _dragging;
		}
	}

	public bool ForceRepaint
	{
		get
		{
			return _dragging || _draggingInput;
		}
	}

	public void Drag(Rect draggingRect)
	{
		if (Event.current.type == EventType.MouseUp)
		{
			_dragging = false;
		}
		else if (Event.current.type == EventType.MouseDown && draggingRect.Contains(Event.current.mousePosition))
		{
			_dragging = true;
			DragStart = Event.current.mousePosition - Position;
			Event.current.Use();
		}

		if (_dragging)
		{
			Position = Event.current.mousePosition - DragStart;
		}
	}
}
