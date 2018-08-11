using UnityEngine;
using UnityEditor;

public class QuestConditionWindow : EditorWindow
{
	SerializedObject serializedObject;
	float repaintTimer = 0;

	[MenuItem("Window/Quest Condition")]
	public static void ShowWindow()
	{
		var window = GetWindow<QuestConditionWindow>("Quest Condition");
		window.minSize = new Vector2(250, 250);
	}

	void Update()
	{
		repaintTimer += 0.01f;
		if (repaintTimer > 1)
		{
			repaintTimer = 0;
			Repaint();
		}
	}

	private void OnGUI()
	{
		if (CheckQuestCondition() == false)
		{
			return;
		}

		serializedObject.Update();

		var states = serializedObject.FindProperty("states");
		var logics = serializedObject.FindProperty("logics");

		if (states.arraySize > 0)
		{
			EditorGUILayout.PropertyField(states.GetArrayElementAtIndex(0), true);
			for (int i = 1; i < states.arraySize; i++)
			{
				DrawLogicOperation(logics.GetArrayElementAtIndex(i - 1));
				EditorGUILayout.PropertyField(states.GetArrayElementAtIndex(i), true);
			}
		}

		DrawButtons(states, logics);
		serializedObject.ApplyModifiedProperties();
	}

	private void DrawLogicOperation(SerializedProperty logicOp)
	{
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.LabelField("", GUILayout.Width(position.width / 2 - 20));
			LogicOperation op = (LogicOperation)logicOp.enumValueIndex;

			System.Enum value = op;
			if (op == LogicOperation.Or)
			{
				value = EditorGUILayout.EnumPopup((LogicOperation)logicOp.enumValueIndex,
					GUILayout.Width(position.width / 2));
			}

			if (op == LogicOperation.And)
			{
				GUILayout.Space(40);
				value = EditorGUILayout.EnumPopup((LogicOperation)logicOp.enumValueIndex,
					GUILayout.Width(position.width / 2 - 80));
				GUILayout.Space(40);
			}

			logicOp.enumValueIndex = (int)((LogicOperation)value);
		}
		EditorGUILayout.EndHorizontal();
	}

	private void DrawButtons(SerializedProperty states, SerializedProperty logic)
	{
		GUILayout.Space(10);
		EditorGUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("Add new", GUILayout.Height(25)))
			{
				states.InsertArrayElementAtIndex(states.arraySize);

				if (states.arraySize > 1)
				{
					logic.InsertArrayElementAtIndex(logic.arraySize);
				}
			}

			GUILayout.Space(20);
			if (GUILayout.Button("Remove last", GUILayout.Height(25)))
			{
				if (states.arraySize > 1)
				{
					states.DeleteArrayElementAtIndex(states.arraySize - 1);
					logic.DeleteArrayElementAtIndex(logic.arraySize - 1);
				}
			}
			GUILayout.Space(10);
		}
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(10);
	}

	bool CheckQuestCondition()
	{
		if (Selection.activeGameObject)
		{
			var questCondition = Selection.activeGameObject.GetComponent<QuestCondition>();
			if (questCondition != null)
			{
				serializedObject = new SerializedObject(questCondition);
				return true;
			}
		}

		GUILayout.Label("No object is currently selected");
		return false;
	}
}
