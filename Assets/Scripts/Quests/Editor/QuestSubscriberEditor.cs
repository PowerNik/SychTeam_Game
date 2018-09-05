using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuestSubscriber))]
public class QuestSubscriberEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10);
        if (GUILayout.Button("Edit quest condition", GUILayout.Height(22)))
        {
            SerializedObject obj = serializedObject;
            var questProp = obj.FindProperty("condition");

            string label = "Subscriber name: " + target.name;
            QuestWindow.ShowQuestWindow(obj, questProp, label, SaveChanges, true);
        }
        GUILayout.Space(10);
    }

    private void SaveChanges()
    {
        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssets();
    }
}
