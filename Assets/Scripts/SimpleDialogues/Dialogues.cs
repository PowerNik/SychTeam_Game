using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum WindowTypes { Phrase, Decision, Option }
public enum NodeType { Start, Default, End }

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue &D")]
public class Dialogues : ScriptableObject
{
    private Window currentWindow;

    [HideInInspector]
    [SerializeField]
    private List<WindowTree> treeList = new List<WindowTree>();

    public int TreeCount
    {
        get { return treeList.Count; }
    }

    public int TreeIndex
    {
        get; set;
    }

    public WindowTree Tree
    {
        get { return treeList[TreeIndex]; }
    }

    [System.Serializable]
    public class WindowTree
    {
        public string Name;
        public int Importance
        {
            get
            {
                if (showCondition.IsTrue)
                    return showCondition.states.Count;
                else
                    return -showCondition.states.Count;
            }
        }

        public int CurrentId;
        public int FirstWindowID = -562;
        public List<Window> Windows = new List<Window>();
        public QuestCondition showCondition = new QuestCondition();

        public Window FirstWindow
        {
            get { return GetWindow(FirstWindowID); }
        }

        public WindowTree(string name)
        {
            Name = name;
            CurrentId = 0;
        }

        public Window GetWindow(int ID)
        {
            return Windows.FirstOrDefault(w => w.ID == ID);
        }

        public int GetWindowIndex(int ID)
        {
            return Windows.FindIndex(w => w.ID == ID);
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
            WindowTypes type = WindowTypes.Phrase, NodeType nodeType = NodeType.Default)
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
            return Type == WindowTypes.Decision;
        }
    }

    private void Reset()
    {
        currentWindow = Tree.FirstWindow;
    }

    public string[] TabsNames
    {
        get { return treeList.Select(set => set.Name).ToArray(); }
    }

    public void SetTree(int treeIndex, out int nextTreeIndex)
    {
        var sortedList = treeList.Where(t => t.Importance >= 0)
            .OrderBy(t => t.Importance)
            .GroupBy(t => t.Importance);

        // if avaible zero-important trees only 
        if (sortedList.Count() == 1 && sortedList.First().Key == 0)
        {
            TreeIndex = treeIndex;
            nextTreeIndex = (treeIndex + 1) % sortedList.First().Count();
        }
        else
        {
            // first of most important trees
            var chosenTree = sortedList.Last().First();
            TreeIndex = treeList.IndexOf(chosenTree);

            /*foreach(var group in sortedList)
            {
                Debug.Log(group.Key + ": " + string.Join(", ", group.Select(w => w.Name).ToArray()));
            }*/
            nextTreeIndex = treeIndex;
        }
        Reset();
    }

    public void CreateTree(string name)
    {
        treeList.Add(new WindowTree(name));
        TreeIndex = treeList.Count - 1;
    }

    public void RemoveCurrentTree()
    {
        treeList.RemoveAt(TreeIndex);
        TreeIndex = 0;
    }

    public bool End()
    {
        return currentWindow.Connections.Count == 0;
    }

    public Speaker CurrentSpeaker()
    {
        return currentWindow.speaker;
    }

    /// <summary>
    /// Moves to the next item in the list.
    /// </summary>
    /// <returns># = Amount of choices it has | 0 = success | -1 = end</returns>
    public int Next()
    {
        ServiceLocator.QuestSystem.SetQuestProgress(currentWindow.activateQuests);

        if (currentWindow.Type == WindowTypes.Decision)
            return currentWindow.Connections.Count;
        else
            if (currentWindow.Connections.Count == 0)
            return -1;
        else
        {
            currentWindow = Tree.GetWindow(currentWindow.Connections[0]);
            return 0;
        }
    }

    /// <summary>
    /// Returns the choices the current node has. 
    /// </summary>
    /// <returns>null if the node isn't a decision node. An array of strings otherwise</returns>
    public string[] GetChoices()
    {
        if (currentWindow.Type != WindowTypes.Decision)
            return new string[] { };
        else
        {
            List<Window> options = new List<Window>();
            for (int i = 0; i < currentWindow.Connections.Count; i++)
            {
                options.Add(Tree.GetWindow(currentWindow.Connections[i]));
            }

            var optionsForShow = options.Where(w => w.showCondition.IsTrue);
            var choises = optionsForShow.OrderBy(w => w.Size.y)
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
        if (currentWindow.Type != WindowTypes.Decision)
            return false;
        else
        {
            for (int i = 0; i < currentWindow.Connections.Count; i++)
            {
                if (Tree.GetWindow(currentWindow.Connections[i]).Text == choice)
                {
                    ServiceLocator.QuestSystem.SetQuestProgress(currentWindow.activateQuests);

                    currentWindow = Tree.GetWindow(Tree.GetWindow(currentWindow.Connections[i]).Connections[0]);
                    return true;
                }
            }
            return false;
        }
    }

    public string GetCurrentPhrase()
    {
        if (currentWindow == null)
            Reset();
        return currentWindow.Text;
    }
}
