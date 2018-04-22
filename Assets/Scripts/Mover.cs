using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
	[SerializeField]
	private float speed = 50f;

	private Rigidbody2D rb;

	void Start ()
	{
		rb = GetComponent<Rigidbody2D>();

		var input = FindObjectOfType<InputController>();
		input.Move += OnMove;
	}

	private void OnMove(Vector2 direction)
	{
		rb.velocity = direction * speed * Time.deltaTime;
	}
}
