using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(QuestState))]
public class QuestStateDrawer : PropertyDrawer
{
	float space = 5;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		Rect pos = position;
		var prefixRect = new Rect(pos.x, pos.y, pos.width / 3 - space, pos.height);
		var typeRect = new Rect(pos.x + pos.width / 3, pos.y, pos.width / 3 - space, pos.height);
		var progressRect = new Rect(pos.x + 2 * pos.width / 3, pos.y, pos.width / 3, pos.height);

		label.text = label.text.Replace("Element ", "") + ":";
		EditorGUI.PrefixLabel(prefixRect, GUIUtility.GetControlID(FocusType.Passive), label);
		EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("questType"), GUIContent.none);
		EditorGUI.PropertyField(progressRect, property.FindPropertyRelative("questProgress"), GUIContent.none);

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}
