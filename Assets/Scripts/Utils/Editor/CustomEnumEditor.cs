using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(CustomEnum))]
public class CustomEnumEditor : Editor
{
	CustomEnum customEnum;
	bool isStringsValide = true;
	bool isStringsUnique = true;

	HashSet<string> enumNamesSet = new HashSet<string>();
	HashSet<int> nonUniqueIndices = new HashSet<int>();

	string filePath = "Assets/Scripts/Enums/";
	string fileName = "CustomEnum";

	private void OnEnable()
	{
		customEnum = (CustomEnum)target;
		fileName = target.name;
	}

	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();

		isStringsValide = isStringsUnique = true;
		CheckStringsUnique();
		CheckAllStrings();

		DrawGUI();
	}

	private void CheckAllStrings()
	{
		for (int i = 0; i < customEnum.enumNames.Count; i++)
		{
			Color defaultColor = GUI.color;
			if(nonUniqueIndices.Contains(i))
			{
				GUI.color = new Color(0.9f, 0.8f, 0f);
			}

			if (CheckString(customEnum.enumNames[i]) == false)
			{	
				GUI.color = new Color(1.0f, 0.6f, 0f);
				isStringsValide = false;
			}

			customEnum.enumNames[i] = EditorGUILayout.TextField(customEnum.enumNames[i]);
			GUI.color = defaultColor;
		}
	}

	private bool CheckString(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return false;
		}

		if(System.Char.IsLetter(str, 0) == false)
		{
			return false;
		}

		return true;
	}

	private void CheckStringsUnique()
	{
		enumNamesSet.Clear();
		nonUniqueIndices.Clear();

		for (int i = 0; i < customEnum.enumNames.Count; i++)
		{
			if(enumNamesSet.Contains(customEnum.enumNames[i]))
			{
				nonUniqueIndices.Add(i);
			}
			else
			{
				enumNamesSet.Add(customEnum.enumNames[i]);
			}
		}
		isStringsUnique = nonUniqueIndices.Count == 0;
	}

	private void DrawGUI()
	{
		GUILayout.Space(10);
		EditorGUILayout.BeginHorizontal();	
			if (GUILayout.Button("Add new", GUILayout.Height(25)))
			{
				customEnum.enumNames.Add(customEnum.enumNames.Last());
			}

			GUILayout.Space(20);
			if (GUILayout.Button("Remove last", GUILayout.Height(25)))
			{
				customEnum.enumNames.Remove(customEnum.enumNames.Last());
			}
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(10);

		
		GUI.enabled = false;
		filePath = EditorGUILayout.TextField("Path", filePath);
		fileName = EditorGUILayout.TextField("Name", fileName);

		GUI.enabled = isStringsValide && isStringsUnique;
		if (GUILayout.Button("Save", GUILayout.Height(30)))
		{
			EdiorMethods.WriteToEnum(filePath, fileName, customEnum.enumNames);
			
			EditorUtility.SetDirty(customEnum);
			AssetDatabase.SaveAssets();
		}
		GUI.enabled = true;
	}
}

