using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuestCondition))]
public class QuestConditionEditor : Editor
{
	public override void OnInspectorGUI()
	{
		GUILayout.Space(10);
		if (GUILayout.Button("Edit quest condition", GUILayout.Height(22)))
		{
			QuestConditionWindow.ShowWindow();
		}
		GUILayout.Space(10);
	}
}
