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
}
