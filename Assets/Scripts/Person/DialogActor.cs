using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class DialogActor : MonoBehaviour
{
	[SerializeField]
	private Dialogues dialog;

	void Start()
	{
		GetComponent<Interactable>().Interacted += StartDialog;
	}

	private void StartDialog()
	{
		var mono = ServiceLocator.GetService(ServiceType.DialogSystem);
		if (dialog != null)
		{
			((DialogueInteraction)mono).SetDialogue(dialog);
		}
	}
}
