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
		ServiceLocator.DialogueSystem.SetDialogue(dialog);
	}
}
