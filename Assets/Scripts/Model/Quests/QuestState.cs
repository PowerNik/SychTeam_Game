using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestState
{
	public QuestType questType;
	public QuestProgress questProgress;

	public QuestState(QuestType type, QuestProgress progress)
	{
		questType = type;
		questProgress = progress;
	}
}