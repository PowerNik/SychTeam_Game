using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
	public event System.Action Interacted;

	public void Interact()
	{
		Interacted();
	}
}
