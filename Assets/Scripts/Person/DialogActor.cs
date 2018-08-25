using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class DialogActor : MonoBehaviour
{
	[SerializeField]
	private Dialogues dialog;

    [SerializeField]
    private bool isCyclicalRepeating = false;

    private int dialogueIndex = 0;

	void Start()
	{
		GetComponent<Interactable>().Interacted += StartDialog;
	}

	private void StartDialog()
	{
        int nextIndex;
		ServiceLocator.DialogueSystem.SetDialogue(dialog, dialogueIndex, out nextIndex);

        if(isCyclicalRepeating)
        {
            dialogueIndex = nextIndex;
        }
	}
}
