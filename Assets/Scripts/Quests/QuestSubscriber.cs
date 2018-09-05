using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestSubscriber : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    private QuestCondition condition;

    [SerializeField]
    private UnityEvent unityEvent;

    private void Start()
    {
        ServiceLocator.QuestSystem.Subscribe(this);
    }

    public void QuestsUpdated()
    {
        if(condition.IsTrue)
        {
            ServiceLocator.QuestSystem.Unsubscribe(this);
            unityEvent?.Invoke();
        }
    }
}
