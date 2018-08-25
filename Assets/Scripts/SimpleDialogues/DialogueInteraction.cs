using UnityEngine;
using UnityEngine.UI;

public class DialogueInteraction : MonoBehaviour
{
	[SerializeField]
	private GameObject rootPanel;

	[SerializeField]
	private Text phraseText;

	private Dialogues dialogue;

	private UIChoiceActor choiceActor;
	private HeadDrawerActor headDrawer;

	private void Awake()
	{
		ServiceLocator.DialogueSystem = this;
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

	public void SetDialogue(Dialogues dialogue, int index, out int nextIndex)
	{
		InputManager.Instance.ChangeInput(InputType.Dialogue);

		this.dialogue = dialogue;
		this.dialogue.SetTree(index, out nextIndex);

		rootPanel.SetActive(true);
		Display();
	}

	private void Choice(int index)
	{
		dialogue.NextChoice(dialogue.GetChoices()[index]);
		Display();
	}

	public void Continue()
	{
		if (dialogue.GetChoices().Length > 0)
		{
			int index = choiceActor.ExtractCurrentChoice();
			if(index == -1)
			{
				return;
			}

			dialogue.NextChoice(dialogue.GetChoices()[index]);
		}
		else
		{
			Progress();
		}

		Display();
	}

	private void Progress()
	{
		if (dialogue.End())
		{
			InputManager.Instance.ChangeInput(InputType.Move);
			rootPanel.SetActive(false);
		}

		dialogue.Next();
	}

	private void Display()
	{
		phraseText.text = dialogue.GetCurrentPhrase();

		choiceActor.ShowChoices(dialogue.GetChoices());
		headDrawer.ShowSpeakerHead(dialogue.CurrentSpeaker());
	}
}
