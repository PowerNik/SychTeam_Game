using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedAffectorable : MonoBehaviour 
{
	private bool isAffected = false;
	private float speedChange = 1f;

	private Mover mover;
	void Start () 
	{
		mover = GetComponent<Mover>();
		InputManager.Instance.MoveInput.Move += OnMove;
	}

	private void OnMove(Vector2 direction)
	{
		if(isAffected && direction.sqrMagnitude == 0)
		{
			speedChange = 1f;
		}

		mover.OnMove(direction * speedChange);
	}

	public void OnAffect(float speedAffect)
	{
		isAffected = true;
		speedChange = speedAffect;
	}
}
