using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue")]
public class Dialogues : ScriptableObject
{
	public enum WindowTypes { Text, Choice, ChoiceAnswer }
	public enum NodeType { Start, Default, End }

	[HideInInspector]
	[SerializeField]
	private Window Current;

	[HideInInspector]
	[SerializeField]
	public List<WindowSet> Set = new List<WindowSet>();
	[HideInInspector]
	[SerializeField]
	public int CurrentSet = 0;
	[HideInInspector]
	[SerializeField]
	public List<string> TabList = new List<string>();

	[System.Serializable]
	public class WindowSet
	{
		public int CurrentId;
		public bool NewWindowOpen;
		public int FirstWindow = -562;
		public List<Window> Windows = new List<Window>();

		public WindowSet()
		{
			CurrentId = 0;
			NewWindowOpen = false;
		}

		public Window GetWindow(int ID)
		{
			for (int i = 0; i < Windows.Count; i++)
			{
				if (Windows[i].ID == ID)
					return Windows[i];
			}
			return null;
		}

		public int GetWindowIndex(int winID)
		{
			for (int i = 0; i < Windows.Count; i++)
			{
				if (Windows[i].ID == winID)
					return i;
			}

			return -1;
		}
	}

	[System.Serializable]
	public class Window
	{
		public int ID;

		public Rect Size;
		public string Text;
		public WindowTypes Type;
		public NodeType NodeType;
		public int Parent;
		public Speaker speaker;

		public List<QuestState> activateQuests = new List<QuestState>();
		public QuestCondition showCondition = new QuestCondition();

		public List<int> Connections = new List<int>();

		public Window(int id, int parent, Rect newSize, 
			WindowTypes type = WindowTypes.Text, NodeType nodeType = NodeType.Default)
		{
			ID = id;
			Parent = parent;
			Size = newSize;
			Text = "";
			Type = type;
			NodeType = nodeType;
		}

		public bool IsChoice()
		{
			return Type == WindowTypes.Choice;
		}
	}

	/// <summary>
	/// Set the current node back to the beginning
	/// </summary>
	/// <returns></returns>
	public void Reset()
	{
		if (CurrentSet < Set.Count)
			Current = Set[CurrentSet].Windows[Set[CurrentSet].FirstWindow];
	}

	/// <summary>
	/// Sets the current tree to be used
	/// </summary>
	/// <param name="TreeName"></param>
	/// <returns></returns>
	public bool SetTree(string TreeName)
	{
		for (int i = 0; i < Set.Count; i++)
		{
			if (TabList[i] == TreeName)
			{
				CurrentSet = i;
				Reset();
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Sets the first tree to be used
	/// </summary>
	/// <param name="TreeName"></param>
	/// <returns></returns>
	public bool SetFirstTree()
	{
		return SetTree(TabList[0]);
	}

	public string GetCurrentTree()
	{
		return TabList[CurrentSet];
	}

	public bool End()
	{
		return Current.Connections.Count == 0;
	}

	public Speaker CurrentSpeaker()
	{
		return Current.speaker;
	}

	/// <summary>
	/// Moves to the next item in the list.
	/// </summary>
	/// <returns># = Amount of choices it has | 0 = success | -1 = end</returns>
	public int Next()
	{
		if (Current.Type == WindowTypes.Choice)
			return Current.Connections.Count;
		else 
			if (Current.Connections.Count == 0)
				return -1;
			else
			{
				Current = Set[CurrentSet].GetWindow(Current.Connections[0]);
				return 0;
			}
	}

	/// <summary>
	/// Returns the choices the current node has. 
	/// </summary>
	/// <returns>null if the node isn't a decision node. An array of strings otherwise</returns>
	public string[] GetChoices()
	{
		if (Current.Type != WindowTypes.Choice)
			return new string[] { };
		else
		{
			List<Window> options = new List<Window>();
			for (int i = 0; i < Current.Connections.Count; i++)
			{
				options.Add(Set[CurrentSet].GetWindow(Current.Connections[i]));
			}
			var choises = options.OrderBy(w => w.Size.y)
				.Select(w => w.Text)
				.ToArray();
			return choises;
		}
	}

	/// <summary>
	/// Moves to the next selected choice
	/// </summary>
	/// <param name="choice"></param>
	/// <returns></returns>
	public bool NextChoice(string choice)
	{
		if (Current.Type != WindowTypes.Choice)
			return false;
		else
		{
			for (int i = 0; i < Current.Connections.Count; i++)
			{
				if (Set[CurrentSet].GetWindow(Current.Connections[i]).Text == choice)
				{
					Current = Set[CurrentSet].GetWindow(Set[CurrentSet].GetWindow(Current.Connections[i]).Connections[0]);
					return true;
				}
			}
			return false;
		}
	}

	public string GetCurrentDialogue()
	{
		if (Current == null)
			Reset();
		return Current.Text;
	}
}
