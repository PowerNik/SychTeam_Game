using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestSystem", menuName = "QuestSystem")]
public class QuestSystem : ScriptableObject
{
	public List<QuestState> questList = new List<QuestState>();

	public void SetQuestProgress(QuestType type, QuestProgress progress)
	{
		QuestState questPair = questList.Find(pair => pair.questType == type);
		if (questPair == null)
		{
			AddNewQuest(type, progress);
		}
		else
		{
			UpdateQuestProgress(questPair, progress);
		}
	}

	public void SetQuestProgress(List<QuestState> list)
	{
		foreach(var state in list)
		{
			SetQuestProgress(state.questType, state.questProgress);
		}
	}

	public QuestProgress GetQuestProgress(QuestType type)
	{
		QuestState result = questList.Find(pair => pair.questType == type);
		if (result == null)
		{
			return QuestProgress.Not_started;
		}

		return result.questProgress;
	}

	public bool CheckQuestState(QuestState state)
	{
		QuestState result = questList.Find(pair => pair.questType == state.questType);
		if (result == null)
		{
			return QuestProgress.Not_started == state.questProgress;
		}

		return result.questProgress == state.questProgress;
	}

	private void AddNewQuest(QuestType type, QuestProgress progress)
	{
		if (progress == QuestProgress.Started || progress == QuestProgress.Failed)
		{
			questList.Add(new QuestState(type, progress));
		}
		else
		{
			Debug.Log(String.Format(
				"Try to ADD quest '{0}' with progress '{1}'",
				type, progress));
		}
	}

	private void UpdateQuestProgress(QuestState questPair, QuestProgress progress)
	{
		if (questPair.questProgress <= progress)
		{
			questPair.questProgress = progress;
		}
		else
		{
			Debug.Log(String.Format(
				"Tried to UPDATE quest '{0}' ({1}) by progress '{2}'",
				questPair.questType, questPair.questProgress, progress));
		}
	}

	[ContextMenu("RemoveAllQuests")]
	private void RemoveAllQuests()
	{
		questList.Clear();
	}
}
