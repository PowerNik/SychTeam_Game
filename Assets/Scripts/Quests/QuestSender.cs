using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSender : MonoBehaviour
{
    [SerializeField]
    private List<QuestState> states = new List<QuestState>();

    [ContextMenu("SendQuests")]
    public void SendQuests()
    {
        ServiceLocator.QuestSystem.SetQuestProgress(states);
        gameObject.SetActive(false);
    }
}
