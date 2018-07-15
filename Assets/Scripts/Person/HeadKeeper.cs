using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CreateAssetMenu(fileName = "HeadKeeper", menuName = "HeadKeeper")]
public class HeadKeeper : ScriptableObject 
{
	public List<SpeakerHead> heads = new List<SpeakerHead>();

	public Sprite GetHead(Speaker speaker)
	{
		return heads.Find(head => head.speaker == speaker).head;
	}

	/// <summary>
	/// This function is called when the behaviour becomes disabled or inactive.
	/// </summary>
	void OnDisable()
	{
		EditorUtility.SetDirty(this);
	}
}
