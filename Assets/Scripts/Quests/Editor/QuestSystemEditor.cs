using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuestSystem))]
public class QuestSystemEditor : Editor
{
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.BeginHorizontal();
			GUILayout.Label("№       ");
			GUILayout.Label("Quest   ");
			GUILayout.Label("Progress");
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(10);

		GUI.enabled = false;
		var list = serializedObject.FindProperty("questList");
		for (int i = 0; i < list.arraySize; i++)
		{
			EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), true);
		}

		GUI.enabled = true;
		serializedObject.ApplyModifiedProperties();
	}
}
