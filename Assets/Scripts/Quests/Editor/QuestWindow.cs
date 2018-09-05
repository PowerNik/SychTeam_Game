using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

public class QuestWindow : EditorWindow
{
	float repaintTimer = 0;
	float savedLabelTime = 0;

	static bool isQuestConditions = true;

	static SerializedObject serializedObject;
	static SerializedProperty prop;

	static string assetName = null;
	static Action saved = null;

	public static void ShowQuestWindow(SerializedObject obj, SerializedProperty questCondition, 
		string label, Action savedCallback, bool isConditions)
	{
		serializedObject = obj;
		prop = questCondition;

		assetName = label;
		saved = savedCallback;
		isQuestConditions = isConditions;

		QuestWindow window;
		if (isQuestConditions)
			window = GetWindow<QuestWindow>("Show Conditions");
		else
			window = GetWindow<QuestWindow>("Quest States");

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

		if (savedLabelTime > 0)
		{
			savedLabelTime -= 0.01f;
		}
	}

	private void OnGUI()
	{
		DrawLabel();

		if (serializedObject == null)
			return;

		serializedObject.Update();

		if (isQuestConditions)
			DrawQuestConditions();
		else
			DrawQuestStates();

		serializedObject.ApplyModifiedProperties();
		DrawSaveButton();
	}

	private void DrawLabel()
	{
		if (savedLabelTime > 0)
			GUILayout.Label("Data saved!");
		else if (assetName != null)
			GUILayout.Label(assetName);
		else
			GUILayout.Label("No object is currently selected");
	}

	private void DrawQuestConditions()
	{
		var states = prop.FindPropertyRelative("states");
		var logics = prop.FindPropertyRelative("logics");

		if (states == null || logics == null)
			return;

		if (states.arraySize > 0)
		{
			EditorGUILayout.PropertyField(states.GetArrayElementAtIndex(0), true);
			for (int i = 1; i < states.arraySize; i++)
			{
				DrawLogicOperation(logics.GetArrayElementAtIndex(i - 1));
				EditorGUILayout.PropertyField(states.GetArrayElementAtIndex(i), true);
			}
		}

		DrawCapacityButtons(states, logics);
	}

	private void DrawQuestStates()
	{
		for (int i = 0; i < prop.arraySize; i++)
		{
			EditorGUILayout.PropertyField(prop.GetArrayElementAtIndex(i), true);
		}

		GUILayout.Space(10);
		EditorGUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("Add new", GUILayout.Height(25)))
			{
				prop.InsertArrayElementAtIndex(prop.arraySize); ;
			}

			GUILayout.Space(20);
			if (GUILayout.Button("Remove last", GUILayout.Height(25)))
			{
				if (prop.arraySize > 0)
					prop.DeleteArrayElementAtIndex(prop.arraySize - 1);
			}
			GUILayout.Space(10);
		}
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(10);
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

	private void DrawCapacityButtons(SerializedProperty states, SerializedProperty logic)
	{
		GUILayout.Space(10);
		EditorGUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("Add new", GUILayout.Height(25)))
			{
				states.InsertArrayElementAtIndex(states.arraySize);

				if (states.arraySize > 1)
					logic.InsertArrayElementAtIndex(logic.arraySize);
			}

			GUILayout.Space(20);
			if (GUILayout.Button("Remove last", GUILayout.Height(25)))
			{
				if (states.arraySize > 0)
					states.DeleteArrayElementAtIndex(states.arraySize - 1);

				if (logic.arraySize > 0)
					logic.DeleteArrayElementAtIndex(logic.arraySize - 1);
			}
			GUILayout.Space(10);
		}
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(10);
	}

	private void DrawSaveButton()
	{
		if (GUILayout.Button("Save changes", GUILayout.Height(25)))
		{
			saved();
			serializedObject = null;

			assetName = null;
			savedLabelTime = 2;
		}
	}
}
