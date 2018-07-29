using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadDrawerActor : MonoBehaviour
{
	[SerializeField]
	private Image leftHead;
	[SerializeField]
	private Image rightHead;

	[SerializeField]
	private HeadKeeper headKeeper;

	public void ShowSpeakerHead(Speaker speaker)
	{
		var head = headKeeper.GetHead(speaker);

		leftHead.gameObject.SetActive(false);
		rightHead.gameObject.SetActive(false);
		
		if (speaker == Speaker.Victor || speaker == Speaker.Victor_young)
		{
			rightHead.gameObject.SetActive(true);
			rightHead.sprite = head;
		}
		else
		{
			leftHead.gameObject.SetActive(true);
			leftHead.sprite = head;
		}
	}
}
