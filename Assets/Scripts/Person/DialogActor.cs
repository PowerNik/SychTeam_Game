using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class DialogActor : MonoBehaviour
{
	void Start()
	{
		GetComponent<Interactable>().Interacted += StartDialog;
	}

	private void StartDialog()
	{
		var mono = ServiceLocator.GetService(ServiceType.DialogSystem);
		if (GetComponent<Dialogues>())
		{
			((DialogueInteraction)mono).SetDialogue(GetComponent<Dialogues>());
		}
	}
}
