using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystemLinker : MonoBehaviour 
{
	[SerializeField]
	private QuestSystem questSystem;

	void Awake () 
	{
		ServiceLocator.QuestSystem = questSystem;
	}
}
