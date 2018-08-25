using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

		for (int i = 0; i < choiceButtons.Length; i++)
		{
			int index = i;
			choiceButtons[index].ButtonHovered += () => OnChoiceButtonHovered(index);
			choiceButtons[index].ButtonClicked += ApplyChoice;
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

            // if there is only one choice - there is no choice at all
            if (activeChoiceCount == 1)
            {
                choiceButtons[0].Deactivate();
                currentChoice = 0;
                activeChoiceCount = 0;
            }
		}
	}

	public int ExtractCurrentChoice()
	{
		int res = currentChoice;
		Reset();

		return res;
	}

	private void OnChoiceButtonHovered(int index)
	{
		currentChoice = index;
	}

	private void ChangeChoiceHighlightion(int delta)
	{
		if(activeChoiceCount == 0)
		{
			return;
		}

		if (currentChoice == -1)
		{
			InitializeFirstChoice(delta);
		}
		else
		{
			ProcessCurrentChoice(delta);
		}

		choiceButtons[currentChoice].Select();
	}

	private void InitializeFirstChoice(int delta)
	{
		if(delta == 1)
		{
			currentChoice = 0;
		}

		if(delta == -1)
		{
			currentChoice = activeChoiceCount - 1;
		}
	}

	private void ProcessCurrentChoice(int delta)
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

	private void ApplyChoice()
	{
		if(currentChoice != -1)
		{
			ChoiceApplied(currentChoice);
			Reset();
		}
	}

	private void Reset()
	{
		currentChoice = -1;
		activeChoiceCount = 0;
		EventSystem.current.SetSelectedGameObject(null);
	}
}
