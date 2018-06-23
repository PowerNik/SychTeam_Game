using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedAffector : MonoBehaviour 
{
	[SerializeField]
	private float speedAffect = -1f;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.GetComponent<SpeedAffectorable>())
		{
			collision.GetComponent<SpeedAffectorable>().OnAffect(speedAffect);
			gameObject.SetActive(false);
		}
	}
}
