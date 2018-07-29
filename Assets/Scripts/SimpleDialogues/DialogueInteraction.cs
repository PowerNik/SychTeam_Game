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

	private void Start()
	{
		headDrawer = GetComponent<HeadDrawerActor>();
		choiceActor = GetComponent<UIChoiceActor>();

		choiceActor.ChoiceApplied += Choice;
		InputManager.Instance.DialogueInput.Continued += Continue;
		phraseText.GetComponent<Button>().onClick.AddListener(Continue);

		rootPanel.SetActive(false);
	}

	public void SetDialogue(Dialogues dialogues, string treeName = "")
	{
		InputManager.Instance.ChangeInput(InputType.Dialogue);

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

	private void Choice(int index)
	{
		npc.NextChoice(npc.GetChoices()[index]);
		Display();
	}

	public void Continue()
	{
		if (npc.GetChoices().Length > 0)
		{
			int index = choiceActor.ExtractCurrentChoice();
			if(index == -1)
			{
				return;
			}

			npc.NextChoice(npc.GetChoices()[index]);
		}
		else
		{
			Progress();
		}

		Display();
	}

	private void Progress()
	{
		if (npc.End())
		{
			InputManager.Instance.ChangeInput(InputType.Move);
			rootPanel.SetActive(false);
			return;
		}

		npc.Next();
	}

	private void Display()
	{
		phraseText.text = npc.GetCurrentDialogue();

		choiceActor.ShowChoices(npc.GetChoices());
		headDrawer.ShowSpeakerHead(npc.CurrentSpeaker());
	}
}
