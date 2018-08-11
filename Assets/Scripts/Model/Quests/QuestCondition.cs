using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LogicOperation { And, Or }

public class QuestCondition : MonoBehaviour
{
	public QuestState[] states;
	public LogicOperation[] logics;

	private List<int> orConditionList = new List<int>();
	private List<List<int>> andConditionList = new List<List<int>>();

	[ContextMenu("GetConditionValue")]
	public bool GetConditionValue()
	{
		if(states.Length == 1)
		{
			return ServiceLocator.QuestSystem.CheckQuestState(states[0]);
		}

		orConditionList.Clear();
		andConditionList.Clear();
		andConditionList.Add(new List<int>());

		ProcessFirstValue();
		ProcessMiddleValues();
		ProcessLastValue();

		if(CheckOrConditions())
		{
			return true;
		}
		else
		{
			return CheckAndConditions();
		}
	}

	private void ProcessFirstValue()
	{
		if (logics.Length == 0)
		{
			return;
		}

		if (logics[0] == LogicOperation.And)
		{
			andConditionList[0].Add(0);
		}
		else
		{
			if (logics[0] == LogicOperation.Or)
			{
				orConditionList.Add(0);
			}
		}
	}

	private void ProcessMiddleValues()
	{
		for (int i = 1; i < logics.Length - 1; i++)
		{
			if (logics[i] == LogicOperation.And)
			{
				andConditionList[andConditionList.Count - 1].Add(i + 1);
			}

			if (logics[i] == LogicOperation.Or)
			{
				if (logics[i + 1] == LogicOperation.Or)
				{
					orConditionList.Add(i + 1);
				}
				if (logics[i + 1] == LogicOperation.And)
				{
					andConditionList.Add(new List<int>());
					andConditionList[andConditionList.Count - 1].Add(i + 1);
				}
			}
		}
	}

	private void ProcessLastValue()
	{
		if (logics.Length == 0)
		{
			return;
		}

		int lastLogic = logics.Length - 1;
		if (logics[lastLogic] == LogicOperation.And)
		{
			andConditionList[andConditionList.Count - 1].Add(lastLogic + 1);
		}
		else
		{
			if (logics[lastLogic] == LogicOperation.Or)
			{
				orConditionList.Add(lastLogic + 1);
			}
		}
	}

	private bool CheckOrConditions()
	{
		for (int index = 0; index < orConditionList.Count; index++)
		{
			if(ServiceLocator.QuestSystem.CheckQuestState(states[index]))
			{
				return true;
			}
		}

		return false;
	}

	private bool CheckAndConditions()
	{
		foreach(var andCond in andConditionList)
		{
			bool result = true;
			for (int i = 0; i < andCond.Count; i++)
			{
				if (ServiceLocator.QuestSystem.CheckQuestState(states[andCond[i]]) == false)
				{
					result = false;
					break;
				}
			}

			if(result)
			{
				return true;
			}
		}

		return false;
	}
}
