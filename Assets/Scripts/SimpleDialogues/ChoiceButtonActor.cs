using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChoiceButtonActor : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
	public Action ButtonHovered;
	public Action ButtonClicked;

	[SerializeField]
	private Text choiceText;

	public void ShowText(string str)
	{
		choiceText.text = str;
		gameObject.SetActive(true);
	}

	public void Select()
	{
		GetComponent<Button>().Select();
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ButtonHovered();
		Select();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		ButtonClicked();
	}
}
