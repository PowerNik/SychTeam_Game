using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
	private Vector2 direction;

	void Start()
	{
		var input = FindObjectOfType<InputController>();

		input.Move += OnMove;
		input.Use += OnUse;
	}

	private void OnMove(Vector2 direction)
	{
		if (direction.sqrMagnitude > 0)
		{
			this.direction = direction;
		}
	}

	private void OnUse()
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
}
