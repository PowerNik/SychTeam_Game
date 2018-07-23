using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(CustomEnum))]
public class CustomEnumEditor : Editor
{
	CustomEnum customEnum;

	HashSet<string> enumNamesSet = new HashSet<string>();
	HashSet<int> nonUniqueIndices = new HashSet<int>();

	bool isStringsValide = true;
	bool isStringsUnique = true;

	int capacity = 0;
	float labelWidth = 70;

	string filePath = "Assets/Scripts/Enums/";
	string fileName = "CustomEnum";

	private void OnEnable()
	{
		fileName = target.name;
		customEnum = (CustomEnum)target;
		capacity = customEnum.enumNames.Count;
	}

	public override void OnInspectorGUI()
	{
		isStringsValide = isStringsUnique = true;

		DrawCapacity();
		FixGroupSize();

		CheckStringsUnique();
		ValidateStrings();

		DrawGUI();
	}

    private void DrawCapacity()
    {
        EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Capacity: ", GUILayout.Width (labelWidth));

            int temp = EditorGUILayout.DelayedIntField(capacity);
            capacity = (temp < 0) ? 0 : temp;

            GUILayout.Space(10);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    private void FixGroupSize()
    {
        int oldCapacity = customEnum.enumNames.Count;
        if (capacity < oldCapacity)
        {
           customEnum.enumNames.RemoveRange(capacity, oldCapacity - capacity);
        }
        else
        {
            customEnum.enumNames.AddRange(new System.String[capacity - oldCapacity]);
        }
    }

	private void CheckStringsUnique()
	{
		enumNamesSet.Clear();
		nonUniqueIndices.Clear();

		for (int i = 0; i < capacity; i++)
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

	private void ValidateStrings()
	{
		for (int i = 0; i < capacity; i++)
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

			EditorGUILayout.BeginHorizontal();
				GUILayout.Label((i + 1) + ": ", GUILayout.Width (labelWidth));
				customEnum.enumNames[i] = EditorGUILayout.TextField(customEnum.enumNames[i]);
				GUILayout.Space(10);
			EditorGUILayout.EndHorizontal();

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

	private void DrawGUI()
	{
		GUILayout.Space(10);
		EditorGUILayout.BeginHorizontal();	
			if (GUILayout.Button("Add new", GUILayout.Height(25)))
			{
				customEnum.enumNames.Add(customEnum.enumNames.Last());
				capacity++;
			}

			GUILayout.Space(20);
			if (GUILayout.Button("Remove last", GUILayout.Height(25)))
			{
				customEnum.enumNames.Remove(customEnum.enumNames.Last());
				if(capacity > 0)
				{
					capacity--;
				}
			}
			GUILayout.Space(10);
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

