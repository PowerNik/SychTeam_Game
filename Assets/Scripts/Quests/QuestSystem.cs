using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "QuestSystem", menuName = "QuestSystem")]
public class QuestSystem : ScriptableObject
{
    [SerializeField]
    private List<QuestState> questList = new List<QuestState>();

    private List<QuestSubscriber> subscribers = new List<QuestSubscriber>();

    public void Subscribe(QuestSubscriber subscriber)
    {
        subscribers.Add(subscriber);
    }

    public void Unsubscribe(QuestSubscriber subscriber)
    {
        subscribers.Remove(subscriber);
    }


    public bool CheckQuestState(QuestState state)
    {
        QuestState result = questList.Find(pair => pair.questType == state.questType);
        if (result == null)
            return QuestProgress.Not_started == state.questProgress;

        return result.questProgress == state.questProgress;
    }

    public List<bool> CheckQuestState(List<QuestState> states)
    {
        List<bool> result = new List<bool>();
        foreach (var state in states)
            result.Add(CheckQuestState(state));

        return result;
    }


    public void SetQuestProgress(List<QuestState> list)
    {
        foreach (var state in list)
            SetQuestProgress(state.questType, state.questProgress);

        for (int i = subscribers.Count - 1; i >= 0; i--)
            subscribers[i].QuestsUpdated();
    }

    private void SetQuestProgress(QuestType type, QuestProgress progress)
    {
        QuestState questPair = questList.Find(pair => pair.questType == type);
        if (questPair == null)
            AddNewQuest(type, progress);
        else
            UpdateQuest(questPair, progress);
    }

    private void AddNewQuest(QuestType type, QuestProgress progress)
    {
        if (progress == QuestProgress.Started || progress == QuestProgress.Failed)
            questList.Add(new QuestState(type, progress));
        else
        {
            Debug.Log(String.Format(
                "Try to ADD quest '{0}' with progress '{1}'",
                type, progress));
        }
    }

    private void UpdateQuest(QuestState questPair, QuestProgress progress)
    {
        if (questPair.questProgress <= progress)
            questPair.questProgress = progress;
        else
        {
            Debug.Log(String.Format(
                "Tried to UPDATE quest '{0}' ({1}) by progress '{2}'",
                questPair.questType, questPair.questProgress, progress));
        }
    }


    [ContextMenu("ResetQuests")]
    private void ResetQuests()
    {
        questList.Clear();
    }
}
