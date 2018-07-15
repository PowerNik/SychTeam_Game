using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomEnum))]
public class CustomEnumEditor : Editor
{
	CustomEnum customEnum;

	string filePath = "Assets/Scripts/Enums/";
	string fileName = "CustomEnum";

	private void OnEnable()
	{
		customEnum = (CustomEnum)target;
		fileName = target.name;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		 
		GUI.enabled = false;
		filePath = EditorGUILayout.TextField("Path", filePath);
		fileName = EditorGUILayout.TextField("Name", fileName);
		GUI.enabled = true;

		if (GUILayout.Button("Save"))
		{
			EditorUtility.SetDirty(customEnum);
			EdiorMethods.WriteToEnum(filePath, fileName, customEnum.enumNames);
		}
	}
}

