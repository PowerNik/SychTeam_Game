using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChoiceActor : MonoBehaviour
{
	public Action<int> ChoiceApplied;

	[SerializeField]
	private Text[] choicesText;

	private int currentChoice = -1;
	private int activeChoiceCount = 0;

	void Start()
	{
		foreach (var text in choicesText)
		{
			var button = text.transform.parent.GetComponent<Button>();
		}
	}

	public void ShowChoices(string[] choices)
	{
		foreach (var text in choicesText)
		{
			text.transform.parent.gameObject.SetActive(false);
		}

		activeChoiceCount = choices.Length;
		if (activeChoiceCount != 0)
		{
			for (int i = 0; i < activeChoiceCount; i++)
			{
				choicesText[i].transform.parent.gameObject.SetActive(true);
				choicesText[i].text = choices[i];
			}
		}
	}

	public void HighlighteChoiceButton(int index)
	{
		currentChoice = index;
	}

	public void ChangeChoiceHighlightion(int delta)
	{
		currentChoice = (currentChoice + delta) % activeChoiceCount;
		HighlighteChoiceButton(currentChoice);
	}

	public void ApplyChoice()
	{
		if(currentChoice != -1)
		{
			ChoiceApplied(currentChoice);
			currentChoice = -1;
			activeChoiceCount = 0;
		}
	}
}
