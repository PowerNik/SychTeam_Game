using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ServiceType
{
	DialogSystem = 0,
}

public static class ServiceLocator
{
	private static Dictionary<ServiceType, MonoBehaviour> serviceDict = new Dictionary<ServiceType, MonoBehaviour>();

	public static void SetService(ServiceType type, MonoBehaviour monobeh)
	{
		serviceDict[type] = monobeh;
	}

	public static MonoBehaviour GetService(ServiceType type)
	{
		return serviceDict[type];
	}

}
