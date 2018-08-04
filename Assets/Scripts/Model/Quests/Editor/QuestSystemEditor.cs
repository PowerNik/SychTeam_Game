using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuestSystem))]
public class QuestSystemEditor : Editor
{
	QuestSystem questSystem;
	float labelWidth = 120;

	private void OnEnable()
	{
		questSystem = (QuestSystem)target;
	}

	public override void OnInspectorGUI()
	{
		GUIStyle style = new GUIStyle();
		style.fontSize = 12;

		EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Quest ", style, GUILayout.Width(labelWidth));
			GUILayout.Label("Progress", style);
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(10);

		for (int i = 0; i < questSystem.questList.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(questSystem.questList[i].questType + ": ", style, GUILayout.Width(labelWidth));
				GUILayout.Label(questSystem.questList[i].questProgress.ToString(), style);
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(5);
		}
	}
}
