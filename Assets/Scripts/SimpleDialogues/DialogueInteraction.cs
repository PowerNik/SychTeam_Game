using UnityEngine;
using UnityEngine.UI;

public class DialogueInteraction : MonoBehaviour
{
	[SerializeField]
	private GameObject rootPanel;

	[SerializeField]
	private Text phraseText;

	private Dialogues npc;

	private UIChoiceActor choiceActor;
	private HeadDrawerActor headDrawer;

	private void Awake()
	{
		ServiceLocator.Register<DialogueInteraction>(this);
	}

	void Start()
	{
		choiceActor = GetComponent<UIChoiceActor>();
		headDrawer = GetComponent<HeadDrawerActor>();
		rootPanel.SetActive(false);
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

	public void Choice(int index)
	{
		npc.NextChoice(npc.GetChoices()[index]); //We make a choice out of the available choices based on the passed index.
		Display();
	}

	public void Continue()
	{
		if (npc.GetChoices().Length != 0)
		{
			return;
		}

		Progress();
		Display();
	}

	public void Progress()
	{
		if (npc.End())
		{
			FindObjectOfType<InputManager>().ChangeInput(InputType.Move);
			rootPanel.SetActive(false);
			return;
		}

		npc.Next();
	}

	public void Display()
	{
		phraseText.text = npc.GetCurrentDialogue();

		choiceActor.ShowChoices(npc.GetChoices());
		headDrawer.ShowSpeakerHead(npc.CurrentSpeaker());
	}
}
