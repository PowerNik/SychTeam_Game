using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
	public event Action<Vector2> Move;
	public event Action Use;

	private KeyCode useKey = KeyCode.Space;
	private Vector2 direction;

	void Update ()
	{
		MovingInput();
		if (Input.GetKeyDown(useKey))
		{
			Use();
		}
	}

	private void MovingInput()
	{
		direction = Vector2.zero;

		if (Input.GetKey(KeyCode.W))
		{
			direction += Vector2.up;
		}
		if (Input.GetKey(KeyCode.A))
		{
			direction += Vector2.left;
		}
		if (Input.GetKey(KeyCode.S))
		{
			direction += Vector2.down;
		}
		if (Input.GetKey(KeyCode.D))
		{
			direction += Vector2.right;
		}

		Move(direction);
	}
}
