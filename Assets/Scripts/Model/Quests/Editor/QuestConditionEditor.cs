using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuestCondition))]
public class QuestConditionEditor : Editor
{
	QuestCondition questCondition;

	private void OnEnable()
	{
		questCondition = (QuestCondition)target;
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		var states = serializedObject.FindProperty("states");
		var logics = serializedObject.FindProperty("logics");

		if(states.arraySize > 0)
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
			EditorGUILayout.PrefixLabel(" ");
			LogicOperation op = (LogicOperation)logicOp.enumValueIndex;

			System.Enum value = op;
			if (op == LogicOperation.Or)
			{
				value = EditorGUILayout.EnumPopup((LogicOperation)logicOp.enumValueIndex);
			}

			if (op == LogicOperation.And)
			{
				GUILayout.Space(30);
				value = EditorGUILayout.EnumPopup((LogicOperation)logicOp.enumValueIndex);
				GUILayout.Space(30);
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
}
