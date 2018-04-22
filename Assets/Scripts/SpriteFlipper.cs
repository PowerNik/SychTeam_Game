using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlipper : MonoBehaviour
{
	private bool isRight = false;
	private SpriteRenderer renderer;

	void Start()
	{
		renderer = GetComponent<SpriteRenderer>();

		var input = FindObjectOfType<InputController>();
		input.Move += OnMove;
	}

	private void OnMove(Vector2 move)
	{
		if(move.x > 0 && isRight == false)
		{
			isRight = true;
		}
		else if (move.x < 0 && isRight == true)
		{
			isRight = false;
		}

		renderer.flipX = isRight;
	}
}
