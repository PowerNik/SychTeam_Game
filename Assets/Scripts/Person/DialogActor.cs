using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class DialogActor : MonoBehaviour
{
	[SerializeField]
	private string[] phrases;

	private int phraseNumber = 0;

	void Start()
	{
		GetComponent<Interactable>().Interacted += StartDialog;
	}

	private void StartDialog()
	{
		if (phraseNumber < phrases.Length)
		{
			Debug.Log(phrases[phraseNumber]);

			if (phraseNumber + 1 < phrases.Length)
			{
				phraseNumber++;
			}
		}
	}

	/*private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Interactable>())
		{
			Debug.Log("Interact with " + other.name);
			other.GetComponent<Interactable>().Interact();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.GetComponent<Interactable>())
		{
			Debug.Log("Interact with " + collision.collider.name);
			collision.collider.GetComponent<Interactable>().Interact();
		}
	}*/
}
