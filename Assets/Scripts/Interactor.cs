using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
	private Vector2 direction;

	void Start()
	{
		var input = FindObjectOfType<InputManager>();

		input.MoveInput.Move += OnMove;
		input.MoveInput.Interact += OnInteract;
	}

	private void OnMove(Vector2 direction)
	{
		if (direction.sqrMagnitude > 0)
		{
			this.direction = direction;
		}
	}

	private void OnInteract()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f);

		if (hit)
		{
			Debug.DrawLine(transform.position, hit.point, Color.green, 3);

			if(hit.collider.GetComponent<Interactable>())
			{
				Debug.Log("Interact with " + hit.collider.name);
				hit.collider.GetComponent<Interactable>().Interact();
			}
		}
		else
		{
			Debug.DrawRay(transform.position, direction * 0.5f, Color.red, 3);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.GetComponent<Interactable>())
		{
			Debug.Log("Interact with " + other.name);
			other.GetComponent<Interactable>().Interact();
		}
	}
}
