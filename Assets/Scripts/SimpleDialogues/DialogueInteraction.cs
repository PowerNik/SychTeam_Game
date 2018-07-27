using UnityEngine;
using UnityEngine.UI;

public class DialogueInteraction : MonoBehaviour
{
	[SerializeField]
	private HeadKeeper headKeeper;

	[Space]
	[SerializeField]
	private GameObject rootPanel;
	[SerializeField]
	private Image leftHead;
	[SerializeField]
	private Image rightHead;

	[Space]
	[SerializeField]
	private Text dialogueText;
	[SerializeField]
	private Text[] choicesText;

	private Dialogues npc;

	private void Awake()
	{
		ServiceLocator.Register<DialogueInteraction>(this);
	}

	public void SetDialogue(Dialogues dialogues, string treeName = "")
	{
		FindObjectOfType<InputManager>().ChangeInput(InputType.Dialogue);

		npc = dialogues;

		if (treeName == "")
		{
			npc.SetFirstTree();
		}
		else
		{
			npc.SetTree(treeName);
		}

		rootPanel.SetActive(true);
		Display();
	}

	void Start()
	{
		rootPanel.SetActive(false);
	}

	public void Choice(int index)
	{
		if (npc.GetChoices().Length != 0)
		{
			npc.NextChoice(npc.GetChoices()[index]); //We make a choice out of the available choices based on the passed index.
		}
		else
		{
			Progress();
		}

		Display();
	}

	public void Progress()
	{
		if (npc.End()) //If this is the last dialogue, set it so the next time we hit "Continue" it will hide the panel
		{
			FindObjectOfType<InputManager>().ChangeInput(InputType.Move);
			rootPanel.SetActive(false);
			return;
		}

		npc.Next(); //This function returns the number of choices it has, in my case I'm checking that in the Display() function though.
	}

	public void Display()
	{
		dialogueText.text = npc.GetCurrentDialogue();

		if (npc.HasTrigger())
		{
			Debug.Log("Triggered: " + npc.GetTrigger());
		}

		foreach (var text in choicesText)
		{
			text.transform.parent.gameObject.SetActive(false);
		}

		if (npc.GetChoices().Length != 0)
		{
			for (int i = 0; i < npc.GetChoices().Length; i++)
			{
				choicesText[i].text = npc.GetChoices()[i];
				choicesText[i].transform.parent.gameObject.SetActive(true);
			}
		}
		else
		{
			choicesText[0].text = "Continue";
			choicesText[0].transform.parent.gameObject.SetActive(true);
		}

		var speaker = npc.CurrentSpeaker();
		var head = headKeeper.GetHead(speaker);
		if (speaker == Speaker.Victor || speaker == Speaker.Victor_young)
		{
			rightHead.gameObject.SetActive(true);
			leftHead.gameObject.SetActive(false);
			rightHead.sprite = head;
		}
		else
		{
			rightHead.gameObject.SetActive(false);
			leftHead.gameObject.SetActive(true);
			leftHead.sprite = head;
		}
	}
}
