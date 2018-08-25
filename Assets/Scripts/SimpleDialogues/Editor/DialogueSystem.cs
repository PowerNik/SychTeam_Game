using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueSystem : EditorWindow {

    static Texture2D BackgroundTexture;
    static Texture2D PanelTexture; 

    Dialogues dialogue;

    //Ids for sending to the Windows
    Dictionary<int, Dialogues.Window> Ids = new Dictionary<int, Dialogues.Window>();

    bool Connecting = false;
    Dialogues.Window ConnectingCurrent = null;

    float Timer = 0;

    Color BigLines = new Color(0.25f, 0.25f, 0.25f);
    Color SmallLines = new Color(0.30f, 0.30f, 0.30f);

    //Used for scrolling in the main view
    Vector2 ScrollPosition = Vector2.zero;
    Vector2 PreviousPosition = Vector2.zero;

    bool isNewWindowShow = false;
    Rect NewTree = new Rect(10, 10, 500, 200);
    string NewTreeName = "";
    string NewTreeInfo = "";

    Vector2 WindowSize = new Vector2(150, 150);
    Vector2 WorkSize = new Vector2(3000, 2000);

    [MenuItem("Window/Dialogue System")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(DialogueSystem));
        MakeTextures();     
    }

    void Update()
    {     
        //If making a connection, constantly repaint so the line draws to the mouse pointer
        if (Connecting)
            Repaint();

        //Calls repaint more frequently for updating when a Dialogues component is added, etc.
        Timer += 0.01f;
        if (Timer > 1)
        {
            Timer = 0;
            Repaint();
        }
    }

    #region Helper Functions
    Dialogues.Window CreateNewWindow(Vector2 Position, int ParentId, Dialogues.WindowTypes Type = Dialogues.WindowTypes.Text)
    {
        Dialogues.Window NewWindow = new Dialogues.Window(
			dialogue.CurrentTree.CurrentId, 
			ParentId, 
			new Rect(Position, WindowSize), 
			Type);
        dialogue.CurrentTree.CurrentId++;
		EditorUtility.SetDirty(dialogue);
        return NewWindow;
    }

    bool CheckDialogueExists()
    {
		var curTree = dialogue.CurrentTree;
		if (curTree.FirstWindow == -562 || curTree.Windows[curTree.FirstWindow].Connections == null)
            return false;
        else
            return true;
    }

    void ClearIds()
    {
        Ids.Clear();
    }

    Dialogues.Window FindPreviousWindow(Dialogues.Window winToFind)
    {
        //If this is the first window, there is no previous
        if (dialogue.CurrentTree.FirstWindow == winToFind.ID)
            return null;
        //Checks all the connections
        for (int i = 0; i < dialogue.CurrentTree.Windows.Count; i++)
        {
            //If any of the connections is equal to the one we're trying to find, we return this Window
            for (int j = 0; j < dialogue.CurrentTree.Windows[i].Connections.Count; j++)
            {
                Dialogues.Window Curr = dialogue.CurrentTree.GetWindow(dialogue.CurrentTree.Windows[i].Connections[j]);
                if (Curr == winToFind)
                    return dialogue.CurrentTree.Windows[i];
            }
        }
        return null;
    }

    List<Dialogues.Window> FindPreviousWindows(Dialogues.Window winToFind)
    {
        if (dialogue.CurrentTree.FirstWindow == winToFind.ID)
            return null;
        List<Dialogues.Window> TheList = new List<Dialogues.Window>();

        //Checks all the connections
        for (int i = 0; i < dialogue.CurrentTree.Windows.Count; i++)
        {
            //If any of the connections is equal to the one we're trying to find, we return this Window
            for (int j = 0; j < dialogue.CurrentTree.Windows[i].Connections.Count; j++)
            {
                Dialogues.Window Curr = dialogue.CurrentTree.GetWindow(dialogue.CurrentTree.Windows[i].Connections[j]);
                if (Curr == winToFind)
                    TheList.Add(dialogue.CurrentTree.Windows[i]);
            }
        }

        return TheList;
    }

    bool Find(Dialogues.Window win, Dialogues.Window winToFind)
    {
        for (int i = 0; i < dialogue.CurrentTree.Windows.Count; i++)
        {
            if (dialogue.CurrentTree.Windows[i] == winToFind)
                return true;
        }
        return false;
        
    }

    static void MakeTextures()
    {
        BackgroundTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        BackgroundTexture.SetPixel(0, 0, new Color(0.35f, 0.35f, 0.35f));
        BackgroundTexture.Apply();

        PanelTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        PanelTexture.SetPixel(0, 0, new Color(0.65f, 0.65f, 0.65f));
        PanelTexture.Apply();
    }

    private Vector2 RotateVector2(Vector2 aPoint, Vector2 aPivot, float aDegree)
    {
        Vector3 pivot = aPivot;
        return pivot + Quaternion.Euler(0, 0, aDegree) * (aPoint - aPivot);
    }
    private float AngleBetweenVector2(Vector2 a, Vector2 b)
    {
        Vector2 difference = b - a;
        float sign = (b.y < a.y) ? -1f : 1f;
        return Vector2.Angle(Vector2.right, difference) * sign;
    }
    private void DrawConnectionArrow(Vector2 posA, Vector2 posB, bool flipArrow = false)
    {
        Vector2 middle = new Vector2((posA.x + posB.x) / 2, (posA.y + posB.y) / 2) - new Vector2(5, 5);
        Vector2 top = new Vector2(middle.x - 10, middle.y);
        Vector2 bottom = new Vector2(middle.x + 10, middle.y);
        Vector2 side = new Vector2(middle.x, middle.y + 10);

        float additionAngle = flipArrow ? -90 : 90;
        Vector2 rotationTop = RotateVector2(top, middle, AngleBetweenVector2(posA, posB) + additionAngle) + new Vector2(4, 4);
        Vector2 rotationBottom = RotateVector2(bottom, middle, AngleBetweenVector2(posA, posB) + additionAngle) + new Vector2(4, 4);
        Vector2 rotationSide = RotateVector2(side, middle, AngleBetweenVector2(posA, posB) + additionAngle) + new Vector2(4, 4);

        Handles.DrawAAConvexPolygon(rotationTop, rotationBottom, rotationSide);
    }

    bool CheckDialogue()
    {
        if (Selection.activeObject is Dialogues)
        {
			dialogue = (Dialogues)Selection.activeObject;//.GetComponent<Dialogues>();
            return true;
        }
        else
        {
            dialogue = null;
            GUILayout.Label("No object is currently selected");
            return false;
        }
    }

    #endregion

    #region Window Functions

    void AddWindowWrapper(object data)
    {
        AddWindow(data);
    }

    Dialogues.Window AddWindow(object data)
    {
        Undo.RecordObject(dialogue, "Dialogue");

        object[] Data = (object[])data;
        //Retrieving Data
        Vector2 Position = (Vector2)Data[0];
        Dialogues.Window WindowClickedOn = (Dialogues.Window)Data[1];
        Dialogues.WindowTypes Type = (Dialogues.WindowTypes)Data[2];

        int ParentId = -1;
        if (WindowClickedOn != null)
            ParentId = WindowClickedOn.ID;
        //Creates the new window
        Dialogues.Window NewlyCreatedWindow = CreateNewWindow(Position, ParentId, Type);

        //Checks if this is the first node
        if (WindowClickedOn == null)
        {
            //It is the first node
            dialogue.CurrentTree.FirstWindow = NewlyCreatedWindow.ID;
        }
        else
        {
            //It is not the first node
            WindowClickedOn.Connections.Add(NewlyCreatedWindow.ID);
        }
        dialogue.CurrentTree.Windows.Add(NewlyCreatedWindow);
		EditorUtility.SetDirty(dialogue);
		return NewlyCreatedWindow;
    }

    void AddWindowBefore(object win)
    {
        Undo.RecordObject(dialogue, "Dialogue");

        Dialogues.Window Curr = (Dialogues.Window)win;

        //If this is the first window
        if (Curr.ID == dialogue.CurrentTree.FirstWindow)
        {
            Dialogues.Window NewlyCreatedWindow = CreateNewWindow(Curr.Size.position - new Vector2(160, 0),-1);
            //The newly created window adds the first node as a connection
            NewlyCreatedWindow.Connections.Add(dialogue.CurrentTree.FirstWindow);
            //The first node is set to the new node
            dialogue.CurrentTree.FirstWindow = NewlyCreatedWindow.ID;
            dialogue.CurrentTree.Windows.Add(NewlyCreatedWindow);
            return;
        }
        Dialogues.Window PrevWindow = FindPreviousWindow(Curr);
        if (PrevWindow != null)
        {
            object[] Vals = { Curr.Size.position - new Vector2(160, 0), PrevWindow, Dialogues.WindowTypes.Text };
            AddWindow(Vals).Connections.Add(Curr.ID);
            PrevWindow.Connections.Remove(Curr.ID);
        }

		EditorUtility.SetDirty(dialogue);
	}

    void AddWindowAfter(object win)
    {
        Undo.RecordObject(dialogue, "Dialogue");

		Dialogues.Window Curr = (Dialogues.Window)win;

        Dialogues.Window NewlyCreatedWindow = CreateNewWindow(Curr.Size.position + new Vector2(160, 0),Curr.ID);

        for (int i = 0; i < Curr.Connections.Count; i++)
        {
            NewlyCreatedWindow.Connections.Add(Curr.Connections[i]);
        }
        Curr.Connections.Clear();
        Curr.Connections.Add(NewlyCreatedWindow.ID);
        dialogue.CurrentTree.Windows.Add(NewlyCreatedWindow);

		EditorUtility.SetDirty(dialogue);
    }

    void RemoveWindow(object win)
    {
        Dialogues.Window Curr = (Dialogues.Window)win;
        ClearIds();

        //If the window we're removing is the start window, we have a custom check
        if (Curr.ID == dialogue.CurrentTree.FirstWindow)
        {
            //We don't allow the user to remove the first node
            if (Curr.Connections.Count == 0)
                dialogue.CurrentTree.FirstWindow = -562;
            return;
        }

        Dialogues.Window PrevWindow = FindPreviousWindow(Curr);
        //If the window to remove has connections
        if (Curr.Connections.Count != 0 && PrevWindow != null)
        {
            //We go through it's connections, and add them to the previous window
            for (int i = 0; i < Curr.Connections.Count; i++)
            {
                PrevWindow.Connections.Add(Curr.Connections[i]);
            }
        }
        if (PrevWindow != null)
        {
            if (Curr.Connections.Count > 1 && PrevWindow.Type == Dialogues.WindowTypes.Text)
                PrevWindow.Type = Dialogues.WindowTypes.Choice;
            if (PrevWindow.ID == dialogue.CurrentTree.FirstWindow)
                AddWindowBefore(PrevWindow);
            //Removes the window from existence
            PrevWindow.Connections.Remove(Curr.ID);
            Curr.Parent = -2;
            for (int i = 0; i < Curr.Connections.Count; i++)
            {
                dialogue.CurrentTree.GetWindow(Curr.Connections[i]).Parent = -2;
            }
        }
        dialogue.CurrentTree.Windows.Remove(Curr);
        ClearIds();

		EditorUtility.SetDirty(dialogue);
	}

    void RemoveWindowTree(object win)
    {
        Dialogues.Window Curr = (Dialogues.Window)win;
        //If this is the first node, removes everything
        if (Curr.ID == dialogue.CurrentTree.FirstWindow)
        {
            if (Curr.Connections.Count == 0)
                dialogue.CurrentTree.FirstWindow = -562;
            return;
        }
        //Simply removes the node, and lets everything connected die
        Dialogues.Window PrevWindow = FindPreviousWindow(Curr);
        Curr.Parent = -2;
        PrevWindow.Connections.Remove(Curr.ID);
		EditorUtility.SetDirty(dialogue);

	}

	void CheckConnections()
    {
        if (dialogue.TreeCount == 0) return;

        for (int i = 0; i < dialogue.CurrentTree.Windows.Count; i++)
        {
            List<Dialogues.Window> WindowList = FindPreviousWindows(dialogue.CurrentTree.Windows[i]);
            if ((WindowList == null || WindowList.Count == 0) 
                && dialogue.CurrentTree.Windows[i].NodeType != Dialogues.NodeType.Start
                && dialogue.CurrentTree.Windows.Count > 1)
            {
                RemoveWindow(dialogue.CurrentTree.Windows[i]);
            }
        }
		EditorUtility.SetDirty(dialogue);
	}

	void StartConnection(object win)
    {
        Connecting = true;
        ConnectingCurrent = (Dialogues.Window)win;
    }

    void CreateConnection(object win)
    {
        Dialogues.Window Curr = (Dialogues.Window)win;

        if (Find(Curr, ConnectingCurrent) || ConnectingCurrent.Connections.Contains(Curr.ID))
        {
            if (!ConnectingCurrent.Connections.Contains(Curr.ID))
            {
                ConnectingCurrent.Connections.Add(Curr.ID);
            }
            Connecting = false;

            if (Curr.Connections.Count > 1)
                Curr.Type = Dialogues.WindowTypes.Choice;

			EditorUtility.SetDirty(dialogue);

			return;
        }

        for (int i = 0; i < ConnectingCurrent.Connections.Count; i++)
        {
            if (ConnectingCurrent.Connections[i] == Curr.ID)
            {
                Connecting = false;
                return;
            }
        }

        ConnectingCurrent.Connections.Add(Curr.ID);
        Connecting = false;
		EditorUtility.SetDirty(dialogue);
	}

	void EstablishNewWindowConnection(object data)
    {
        object[] Data = (object[])data;
        Vector2 Position = (Vector2)Data[0];
        Dialogues.WindowTypes Type = (Dialogues.WindowTypes)Data[1];

        object[] Vals = { Position, ConnectingCurrent, Type };
        CreateConnection(AddWindow(Vals));
		EditorUtility.SetDirty(dialogue);
	}

	void RemoveConnection(object data)
    {
        object[] Data = (object[])data;
        Dialogues.Window Curr = (Dialogues.Window)Data[0];
        int ToRemove = (int)Data[1];

        bool Remove = true;
        for (int i = 0; i < dialogue.CurrentTree.Windows.Count; i++)
        {
            for (int j = 0; j < dialogue.CurrentTree.Windows[i].Connections.Count; j++)
            {
                if(dialogue.CurrentTree.Windows[i].Connections[j] == ToRemove 
                    && Curr.ID != dialogue.CurrentTree.Windows[i].Connections[j])
                {
                    Remove = false;
                }
            }
        }
        if(Remove)
            dialogue.CurrentTree.Windows.Remove(dialogue.CurrentTree.GetWindow(ToRemove));
        Curr.Connections.Remove(ToRemove);

		EditorUtility.SetDirty(dialogue);
	}

	void CancelConnection()
    {
        Connecting = false;
    }

    void Convert(object win)
    {
        Dialogues.Window Curr = (Dialogues.Window)win;
        if (Curr.Type == Dialogues.WindowTypes.Choice)
            Curr.Type = Dialogues.WindowTypes.Text;
        else
            Curr.Type = Dialogues.WindowTypes.Choice;
    }

    void CheckPosition(Dialogues.Window win)
    {
        Vector2 newPos = win.Size.position;
        if (win.Size.center.x < 0)
            win.Size.position = new Vector2(0, newPos.y);
        if(win.Size.center.x > WorkSize.x)
            win.Size.position = new Vector2(WorkSize.x - (win.Size.width), newPos.y);
        if(win.Size.center.y < 0)
            win.Size.position = new Vector2(newPos.x, 0);
        if (win.Size.center.y > WorkSize.y)
            win.Size.position = new Vector2(newPos.x, WorkSize.y - (win.Size.height));
    }

    void Clear()
    {
        dialogue.CurrentTree.Windows.Clear();
        dialogue.CurrentTree.FirstWindow = -562;

		EditorUtility.SetDirty(dialogue);
	}

	void SaveChanges()
	{
		EditorUtility.SetDirty(dialogue);
		AssetDatabase.SaveAssets();
	}

	#endregion


	void DrawConnections(Color LineColor)
    {
        if (!CheckDialogueExists()) return;
        //Goes through the window's connections
        for (int j = 0; j < dialogue.CurrentTree.Windows.Count; j++)
        {
            for (int i = 0; i < dialogue.CurrentTree.Windows[j].Connections.Count; i++)
            {
                Dialogues.Window WindowList = dialogue.CurrentTree.Windows[j];

                Color Use = LineColor;

                if (WindowList.Type == Dialogues.WindowTypes.Choice)
                    Use = Color.green;

                //Draws a line with the correct color between the current window and connection
                Handles.DrawBezier(WindowList.Size.center, 
                    dialogue.CurrentTree.GetWindow(WindowList.Connections[i]).Size.center, 
                    new Vector2(WindowList.Size.center.x, WindowList.Size.center.y), 
                    new Vector2(dialogue.CurrentTree.GetWindow(WindowList.Connections[i]).Size.center.x, 
                    dialogue.CurrentTree.GetWindow(WindowList.Connections[i]).
                    Size.center.y), Use, null, 5f);

                Use = LineColor;

                //Finds the center between the points
                float xPos = (WindowList.Size.center.x) + ((dialogue.CurrentTree.GetWindow(WindowList.Connections[i]).Size.center.x - WindowList.Size.center.x) / 2);
                float yPos = (WindowList.Size.center.y) + ((dialogue.CurrentTree.GetWindow(WindowList.Connections[i]).Size.center.y - WindowList.Size.center.y) / 2);
                Vector2 Middle = new Vector2(xPos, yPos);

                //Draws arrows 
                DrawConnectionArrow(WindowList.Size.center, Middle, true);
                DrawConnectionArrow(Middle, dialogue.CurrentTree.GetWindow(WindowList.Connections[i]).Size.center, true);
                DrawConnectionArrow(WindowList.Size.center, dialogue.CurrentTree.GetWindow(WindowList.Connections[i]).Size.center, true);
            }
        }

        if (Connecting)
        {
            Color ManualColor = Color.magenta;
            Handles.DrawBezier(ConnectingCurrent.Size.center, Event.current.mousePosition, new Vector2(ConnectingCurrent.Size.xMax + 50f, ConnectingCurrent.Size.center.y), new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y), ManualColor, null, 5f);
        }
    }

    void BuildWindows()
    {
        if (CheckDialogue() && !CheckDialogueExists()) return;

        for (int j = 0; j < dialogue.CurrentTree.Windows.Count; j++)
        {
            Dialogues.Window WindowList = dialogue.CurrentTree.Windows[j];

            List<Dialogues.Window> PrevWindow = FindPreviousWindows(WindowList);
            if (PrevWindow != null)
            {
                for (int i = 0; i < PrevWindow.Count; i++)
                {
                    if (PrevWindow[i].Type == Dialogues.WindowTypes.Choice)
                    {
                        WindowList.Type = Dialogues.WindowTypes.ChoiceAnswer;
                        break;
                    }
                    if (PrevWindow[i].Type == Dialogues.WindowTypes.ChoiceAnswer && WindowList.Type == Dialogues.WindowTypes.ChoiceAnswer)
                        WindowList.Type = Dialogues.WindowTypes.Text;
                    if (PrevWindow[i].Type == Dialogues.WindowTypes.Text && WindowList.Type == Dialogues.WindowTypes.ChoiceAnswer)
                        WindowList.Type = Dialogues.WindowTypes.Text;
                }
            }

            //Default naming
            string BoxName = "";
            switch (WindowList.Type)
            {
                case Dialogues.WindowTypes.Text:
                    BoxName = "Dialogue";
                    break;
                case Dialogues.WindowTypes.Choice:
                    BoxName = "Decision";
                    break;
                case Dialogues.WindowTypes.ChoiceAnswer:
                    BoxName = "Option";
                    break;
            }
            //Determines what type of node it is
            if (WindowList.Connections.Count == 0 && WindowList.ID != dialogue.CurrentTree.FirstWindow && WindowList.Type != Dialogues.WindowTypes.Choice)
                WindowList.NodeType = Dialogues.NodeType.End;
            else if (WindowList.ID == dialogue.CurrentTree.FirstWindow)
                WindowList.NodeType = Dialogues.NodeType.Start;
            else
                WindowList.NodeType = Dialogues.NodeType.Default;

            //Changes the name accordingly
            if (WindowList.Type != Dialogues.WindowTypes.Choice)
            {
                switch (WindowList.NodeType)
                {
                    case Dialogues.NodeType.Start:
                        BoxName = "Start";
                        break;
                    case Dialogues.NodeType.End:
                        BoxName = "End";
                        if (WindowList.Type == Dialogues.WindowTypes.ChoiceAnswer)
                            BoxName = "End (Option)";
                        break;
                    default: break;
                }
            }

            if (!Ids.ContainsKey(WindowList.ID))
                Ids.Add(WindowList.ID, WindowList);

            //Creates the actual window
            string Style = "flow node 0";
            if (WindowList.Type == Dialogues.WindowTypes.Choice) Style = "flow node 3";
            if (WindowList.NodeType == Dialogues.NodeType.Start) Style = "flow node 1";
            if (WindowList.Type == Dialogues.WindowTypes.ChoiceAnswer) Style = "flow node 2";
            if (WindowList.NodeType == Dialogues.NodeType.End) Style = "flow node 6";
            if (WindowList.Type == Dialogues.WindowTypes.Choice && WindowList.Connections.Count == 0) Style = "flow node 4";

            GUIStyle FinalStyle = new GUIStyle(Style);
            FinalStyle.fontSize = 14;
            FinalStyle.contentOffset = new Vector2(0, -30);
            CheckPosition(WindowList);

			WindowList.Size.width = WindowSize.x;
			WindowList.Size.height = WindowSize.y;
			WindowList.Size = GUI.Window(WindowList.ID, WindowList.Size, WindowFunction, BoxName, FinalStyle);
        }
    }

    void BuildMenus(GenericMenu Menu)
    {
        Vector2 AdjustedMousePosition = Event.current.mousePosition + ScrollPosition - new Vector2(50, 50);
        Vector2 AdjustedMousePositionLine = Event.current.mousePosition + ScrollPosition;

        if (!CheckDialogueExists())
        {
            object[] Vals = { AdjustedMousePosition, null, Dialogues.WindowTypes.Text };
            Menu.AddItem(new GUIContent("Create First Window"), false, AddWindowWrapper, Vals);

            return;
        }

        for (int j = 0; j < dialogue.CurrentTree.Windows.Count; j++)
        {
            Dialogues.Window WindowList = dialogue.CurrentTree.Windows[j];

            //Accounts for the area when the user has scrolled
            Rect AdjustedArea = new Rect(WindowList.Size.position - ScrollPosition, WindowList.Size.size);

            if (Connecting)
            {
                object[] CreateInfoDialogue = { AdjustedMousePosition, Dialogues.WindowTypes.Text };
                object[] CreateInfoChoice = { AdjustedMousePosition, Dialogues.WindowTypes.Choice };
                if (ConnectingCurrent.Type == Dialogues.WindowTypes.Text && ConnectingCurrent.Connections.Count == 0 || ConnectingCurrent.Type == Dialogues.WindowTypes.ChoiceAnswer && ConnectingCurrent.Connections.Count == 0 || ConnectingCurrent.Type == Dialogues.WindowTypes.Choice)
                {
                    Menu.AddItem(new GUIContent("Create Dialogue Window"), false, EstablishNewWindowConnection, CreateInfoDialogue);
                    Menu.AddItem(new GUIContent("Create Decision Window"), false, EstablishNewWindowConnection, CreateInfoChoice);
                }

            }

            //Checks if the mouse is close enough to a line
            for (int i = 0; i < WindowList.Connections.Count; i++)
            {
                float xPos = (WindowList.Size.center.x) + ((dialogue.CurrentTree.GetWindow(WindowList.Connections[i]).Size.center.x - WindowList.Size.center.x) / 2);
                float yPos = (WindowList.Size.center.y) + ((dialogue.CurrentTree.GetWindow(WindowList.Connections[i]).Size.center.y - WindowList.Size.center.y) / 2);
                Vector2 Middle = new Vector2(xPos, yPos);
                float xPosLower = (WindowList.Size.center.x) + ((Middle.x - WindowList.Size.center.x) / 2);
                float yPosLower = (WindowList.Size.center.y) + ((Middle.y - WindowList.Size.center.y) / 2);
                Vector2 Lower = new Vector2(xPosLower, yPosLower);
                float xPosHigher = (Middle.x) + ((dialogue.CurrentTree.GetWindow(WindowList.Connections[i]).Size.center.x - Middle.x) / 2);
                float yPosHigher = (Middle.y) + ((dialogue.CurrentTree.GetWindow(WindowList.Connections[i]).Size.center.y - Middle.y) / 2);
                Vector2 Higher = new Vector2(xPosHigher, yPosHigher);


                if (Mathf.Abs((AdjustedMousePositionLine - Middle).magnitude) < 50 || Mathf.Abs((AdjustedMousePositionLine - Lower).magnitude) < 50 || Mathf.Abs((AdjustedMousePositionLine - Higher).magnitude) < 50)
                {
                    object[] Vals = { WindowList, WindowList.Connections[i] };
                    Menu.AddItem(new GUIContent("Remove Connection"), false, RemoveConnection, Vals);
                }
            }

            //Checks if the mouse is on the current box
            if (AdjustedArea.Contains(Event.current.mousePosition))
            {
                if (Connecting)
                {
                    if (AdjustedArea.Contains(Event.current.mousePosition))
                    {
                        if (ConnectingCurrent.Type == Dialogues.WindowTypes.Text && ConnectingCurrent.Connections.Count == 0 || ConnectingCurrent.Type == Dialogues.WindowTypes.ChoiceAnswer && ConnectingCurrent.Connections.Count == 0 || ConnectingCurrent.Type == Dialogues.WindowTypes.Choice)
                            Menu.AddItem(new GUIContent("Establish Connection"), false, CreateConnection, WindowList);
                    }
                }
                else
                {
                    if (WindowList.Type == Dialogues.WindowTypes.Text)
                        Menu.AddItem(new GUIContent("Convert To Decision Window"), false, Convert, WindowList);
                    else if (WindowList.Connections.Count <= 1 && WindowList.Type != Dialogues.WindowTypes.ChoiceAnswer)
                        Menu.AddItem(new GUIContent("Convert To Dialogue Window"), false, Convert, WindowList);
                    else if (WindowList.Type != Dialogues.WindowTypes.ChoiceAnswer)
                        Menu.AddDisabledItem(new GUIContent("Convert To Dialogue Window"));
                    if (WindowList.Type != Dialogues.WindowTypes.ChoiceAnswer)
                        Menu.AddSeparator("");

                    if (WindowList.Type == Dialogues.WindowTypes.Text && WindowList.Connections.Count == 0 || WindowList.Type == Dialogues.WindowTypes.ChoiceAnswer && WindowList.Connections.Count == 0 || WindowList.Type == Dialogues.WindowTypes.Choice)
                        Menu.AddItem(new GUIContent("Create Connection"), false, StartConnection, WindowList);
                    Menu.AddItem(new GUIContent("Remove Window"), false, RemoveWindow, WindowList);
                    Menu.AddItem(new GUIContent("Remove Window Tree"), false, RemoveWindowTree, WindowList);
                }

            }
        }


        if(Connecting)
            Menu.AddItem(new GUIContent("Cancel"), false, CancelConnection);

		EditorUtility.SetDirty(dialogue);

	}

	private void OnGUI()
    {
        CheckDialogue();

        if (!dialogue)
        {
            GUILayout.Label("This object has no Dialogues component");
            return;
        }

        CheckConnections();

        if (BackgroundTexture == null)
        {
            MakeTextures();
        }

        if (dialogue.TreeCount == 0)
        {
            GUIStyle Style = GUI.skin.GetStyle("Label");
            Style.alignment = TextAnchor.MiddleCenter;
            
            GUILayout.Label("\n\nThere are no existing Dialogue Trees\nClick the 'New' button to add a tab",Style);
        }
        else
        {
            //Creates large scroll view for the work area
            ScrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), 
				ScrollPosition, new Rect(Vector2.zero, WorkSize), GUIStyle.none, GUIStyle.none);
            //Makes the background a dark gray
            GUI.DrawTexture(new Rect(0, 0, WorkSize.x, WorkSize.y), BackgroundTexture, ScaleMode.StretchToFill);
            Handles.BeginGUI();

            //Draws the small, light squares all over the work area
            int Count = 0;
            while ((Count * 10) < WorkSize.x)
            {
                EditorGUI.DrawRect(new Rect(Count * 10, 0, 2, WorkSize.y), SmallLines);
                Count++;
            }
            Count = 0;
            while ((Count * 10) < WorkSize.y)
            {
                EditorGUI.DrawRect(new Rect(0, Count * 10, WorkSize.x, 2), SmallLines);
                Count++;
            }

            //Draws the larger, thicker lines on the work area
            for (int i = 0; i < WorkSize.x / 100; i++)
            {
                EditorGUI.DrawRect(new Rect(i * 100, 0, 2, WorkSize.y), BigLines);
            }
            for (int i = 0; i < WorkSize.y / 100; i++)
            {
                EditorGUI.DrawRect(new Rect(0, i * 100, WorkSize.x, 2), BigLines);
            }

            DrawConnections(Color.white);

            Handles.EndGUI();

            BeginWindows();
            ClearIds();

            BuildWindows();

            if (isNewWindowShow)
            {
                NewTree = GUI.Window(99999, NewTree, AddNewWindow, "Add New Dialogue Tree");
            }

            EndWindows();

            GUI.EndScrollView();
            GUILayout.BeginArea(new Rect(0, 20, Screen.width, 20));
            dialogue.CurrentTreeIndex = GUILayout.Toolbar(dialogue.CurrentTreeIndex, dialogue.TabsNames);
            GUILayout.EndArea();

            if (new Rect(0, 0, position.width, position.height).Contains(Event.current.mousePosition))
            {
                if (Event.current.button == 1)
                {
                    GenericMenu Menu = new GenericMenu();

                    BuildMenus(Menu);
					if (CheckDialogueExists())
					{
						Menu.AddItem(new GUIContent("Save Changes"), false, SaveChanges);
						Menu.AddItem(new GUIContent(""), false, () => { });
						Menu.AddItem(new GUIContent("Clear All"), false, Clear);
					}
                    Menu.ShowAsContext();
                }
            }

            if (new Rect(0, 0, position.width, position.height).Contains(Event.current.mousePosition))
            {
                if (Event.current.button == 2 && Event.current.type == EventType.MouseDrag)
                {
                    Vector2 CurrentPos = Event.current.mousePosition;

                    if (Vector2.Distance(CurrentPos, PreviousPosition) < 50)
                    {
                        float x = PreviousPosition.x - CurrentPos.x;
                        float y = PreviousPosition.y - CurrentPos.y;

                        ScrollPosition.x += x;
                        ScrollPosition.y += y;
                        Event.current.Use();
                    }
                    PreviousPosition = CurrentPos;
                }
            }
        }

        //Extra layouts
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, 50));
        GUILayout.BeginHorizontal("GUIEditor.BreadcrumbLeft");
        if (GUILayout.Button("New Dialogue Tree", new GUIStyle("TE toolbarbutton"), GUILayout.Width(150), GUILayout.Height(18)))
        {
            NewTree = new Rect(50 + ScrollPosition.x, 50 + ScrollPosition.y, 400, 150);
            NewTreeName = "";
            NewTreeInfo = "";
            isNewWindowShow = true;
        }

        if (GUILayout.Button("Remove Tree", new GUIStyle("TE toolbarbutton"), GUILayout.Width(100), GUILayout.Height(18)))
        {
            if (dialogue.TreeCount > 0)
                dialogue.RemoveCurrentTree();
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();


        if (isNewWindowShow && dialogue.TreeCount == 0)
        {
            BeginWindows();
            NewTree = GUI.Window(99999, NewTree, AddNewWindow, "Add New Dialogue Tree");
            EndWindows();
        }

        //Found this easy cut/copy/paste solution from http://answers.unity3d.com/questions/181333/how-can-i-add-copy-and-paste-support-for-my-editor.html
        TextEditor textEditor = EditorGUIUtility.GetStateObject(typeof(TextEditor), EditorGUIUtility.keyboardControl) as TextEditor;
        if (textEditor != null)
        {
            if (focusedWindow == this)
            {
                // shift + x
                if (Event.current.Equals(Event.KeyboardEvent("#x")))
                    textEditor.Cut();
                // shift + c
                if (Event.current.Equals(Event.KeyboardEvent("#c")))
                    textEditor.Copy();
                // shift + v
                if (Event.current.Equals(Event.KeyboardEvent("#v")))
                    textEditor.Paste();
            }
        }

    }

    void WindowFunction(int windowID)
    {
        if (!Ids.ContainsKey(windowID)) return;
		 Dialogues.Window Win = Ids[windowID];

        int xSize = 150;
        int ySize = 84;

        if(Win.NodeType == Dialogues.NodeType.Start)
        {
            if (GUI.Button(new Rect(0, -20, 70, 20), "Tree conditions: " + dialogue.CurrentTree.Importance))
            {
                TreeShowCondition();
            }
        }

        if(Win.Type != Dialogues.WindowTypes.Choice)
        {
            if(GUI.Button(new Rect(0, 0, 15, 15), "-")) AddWindowBefore(Win);
            if(GUI.Button(new Rect(135, 0, 15, 15), "+")) AddWindowAfter(Win) ;
        }

        Win.Text = GUI.TextArea(new Rect(0, 15, xSize, ySize), Win.Text);

		GUI.Label(new Rect(0, 100, 60, 20), "Speaker: ");
		Win.speaker = (Speaker)EditorGUI.EnumPopup(
			new Rect(60, 102, 85, 20),
			Win.speaker);

		if (Win.Type == Dialogues.WindowTypes.ChoiceAnswer)
		{
			if (GUI.Button(new Rect(10, 125, 130, 20), "Show conditions: " + Win.showCondition.states.Count))
			{
				ShowQuests(windowID, true);
			}
		}
        else
        {
            if (GUI.Button(new Rect(35, 125, 80, 20), "Quests: " + Win.activateQuests.Count))
            {
                ShowQuests(windowID, false);
            }
        }

		GUI.DragWindow();
    }

	private void ShowQuests(int windowID, bool isConditions)
	{
		int windowIndex = dialogue.CurrentTree.GetWindowIndex(windowID);

		SerializedObject obj = new SerializedObject(dialogue);
		var treeList = obj.FindProperty("treeList");
		var curTree = treeList.GetArrayElementAtIndex(dialogue.CurrentTreeIndex);
		var curWindow = curTree.FindPropertyRelative("Windows").GetArrayElementAtIndex(windowIndex);

		var questProp = curWindow.FindPropertyRelative("activateQuests");
		if (isConditions)
			questProp = curWindow.FindPropertyRelative("showCondition");

		string label = "Dialogue: " + dialogue.name;
		label += "\nTab: " + dialogue.CurrentTree.Name;
		label += "\nWindow text: " + dialogue.CurrentTree.GetWindow(windowID).Text;

		QuestWindow.ShowQuestWindow(obj, questProp, label, SaveChanges, isConditions);
	}

    private void TreeShowCondition()
    {
        SerializedObject obj = new SerializedObject(dialogue);
        var treeList = obj.FindProperty("treeList");
        var curTree = treeList.GetArrayElementAtIndex(dialogue.CurrentTreeIndex);
        var questProp = curTree.FindPropertyRelative("showCondition");

        string label = "Dialogue: " + dialogue.name;
        label += "\nTab: " + dialogue.CurrentTree.Name;

        QuestWindow.ShowQuestWindow(obj, questProp, label, SaveChanges, true);
    }

    void AddNewWindow(int winID)
    {
        NewTreeName = GUI.TextField(new Rect(100, 50, 200, 20), NewTreeName);
        GUI.Label(new Rect(100, 25, 200, 20), NewTreeInfo);

        if(GUI.Button(new Rect(150,100,100,40), "Add") || (Event.current.isKey && Event.current.keyCode == KeyCode.Return))
        {
            if (NewTreeName == "" || NewTreeName == " ")
                NewTreeInfo = "Must Enter a Name";
            else
            {
                isNewWindowShow = false;
                dialogue.CreateTree(NewTreeName);
            }
        }

        if (GUI.Button(new Rect(370, 0, 30, 30), "X"))
        {
            isNewWindowShow = false;
        }

        GUI.DragWindow();
		EditorUtility.SetDirty(dialogue);
	}
}
