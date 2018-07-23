using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UI;

[CustomEditor(typeof(HeadKeeper))]
public class HeadKeeperEditor : Editor 
{
	float imageSize = 80;

	float verticalSpacing = 15;
	float labelWidth = 150;

	public override void OnInspectorGUI()
	{     
		HeadKeeper keeper = (HeadKeeper)target;

		FixGroupSize(keeper);
		DrawImages(keeper);

		EditorUtility.SetDirty(keeper);
	}

	private void FixGroupSize(HeadKeeper keeper)
	{
		int enumCount = Enum.GetValues (typeof(Speaker)).Length;
		int headCount = keeper.heads.Count;
		if (enumCount < headCount)
		{
			keeper.heads.RemoveRange(enumCount, headCount - enumCount);
		}
		else
		{
			keeper.heads.AddRange(new SpeakerHead[enumCount - headCount]);
		}
	}

	private void DrawImages(HeadKeeper keeper)
	{
		int i = 0;
		foreach(Speaker speaker in Enum.GetValues (typeof(Speaker)))
		{
			float aspect = 1;
			if (keeper.heads[i] != null && keeper.heads[i].head != null)
			{
				aspect = (float)keeper.heads[i].head.rect.width / keeper.heads[i].head.rect.height;
				keeper.heads[i].speaker = speaker;
			}
			
			EditorGUILayout.BeginHorizontal();	
				GUILayout.Label((i + 1) + ": " + speaker.ToString(), GUILayout.Width (labelWidth));
				
				if (keeper.heads [i] != null) 
				{					
					keeper.heads [i].head = (Sprite)EditorGUILayout.ObjectField (keeper.heads[i].head, typeof(Sprite), false, 
					new GUILayoutOption[] {GUILayout.Height(imageSize), GUILayout.Width(imageSize * aspect)});
				}
				GUILayout.Space(20);
            EditorGUILayout.EndHorizontal();
			GUILayout.Space(verticalSpacing);
			
			i++;
		}
	}
}