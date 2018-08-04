using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomTypeSO", menuName = "Utils/Generate enum")]
public class CustomEnum : ScriptableObject 
{
	public List<string> enumNames = new List<string>();
}
