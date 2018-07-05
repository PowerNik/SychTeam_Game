using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SpeakerHeads", menuName = "SpeakerHeads")]
public class SpeakerHeads : ScriptableObject 
{
	[System.Serializable]
	public class Head
	{
		public Speaker speaker;
		public Sprite head;
	}

	public List<Head> heads;

	public Sprite GetHead(Speaker speaker)
	{
		return heads.Find(head => head.speaker == speaker).head;
	}
}
