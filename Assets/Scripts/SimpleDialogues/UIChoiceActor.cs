using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChoiceActor : MonoBehaviour
{
	public Action<int> ChoiceApplied;

	[SerializeField]
	private ChoiceButtonActor[] choiceButtons;

	private int currentChoice = -1;
	private int activeChoiceCount = 0;

	void Start()
	{
		InputManager.Instance.DialogueInput.ChoiceChanged += ChangeChoiceHighlightion;
		InputManager.Instance.DialogueInput.Continued += ApplyChoice;

		for (int i = 0; i < choiceButtons.Length; i++)
		{
			int index = i;
			choiceButtons[index].ButtonHovered += () => OnChoiceButtonHovered(index);
			choiceButtons[index].GetComponent<Button>().onClick.AddListener(ApplyChoice);
			choiceButtons[index].Deactivate();
		}
	}

	public void ShowChoices(string[] choices)
	{
		foreach (var choise in choiceButtons)
		{
			choise.Deactivate();
		}

		activeChoiceCount = choices.Length;
		if (activeChoiceCount != 0)
		{
			for (int i = 0; i < activeChoiceCount; i++)
			{
				choiceButtons[i].ShowText(choices[i]);
			}
		}
	}

	public void OnChoiceButtonHovered(int index)
	{
		currentChoice = index;
	}

	public void ChangeChoiceHighlightion(int delta)
	{
		if(activeChoiceCount == 0)
		{
			return;
		}

		if (currentChoice == -1)
		{
			currentChoice = 0;
		}
		else
		{
			currentChoice += delta;
			if (currentChoice >= 0)
			{
				currentChoice = currentChoice % activeChoiceCount;
			}
			else
			{
				currentChoice = activeChoiceCount + currentChoice % activeChoiceCount;
			}
		}

		choiceButtons[currentChoice].Select();
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
