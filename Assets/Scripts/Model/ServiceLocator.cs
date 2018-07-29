using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
	private static Dictionary<Type, object> serviceDict = new Dictionary<Type, object>();

	public static void Register<T>(T monobeh)
	{
		serviceDict[typeof(T)] = monobeh;
	}

	public static T GetService<T>()
	{
		return (T)serviceDict[typeof(T)];
	}
}
