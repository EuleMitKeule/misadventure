using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HotlineHyrule.Quests;
using UnityEditor;
using UnityEngine;

namespace HotlineHyruleEditor
{
    // [CustomEditor(typeof(QuestData))]
    public class QuestDataEditor : Editor
    {
        Type[] QuestTargetTypes { get; set; }
        string[] QuestTargetTypeNames { get; set; }

        int SelectedQuestTargetTypeIndex { get; set; }

        QuestData QuestData => (QuestData)serializedObject.targetObject;

        public void OnEnable()
        {
            QuestTargetTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.IsSubclassOf(typeof(QuestTarget))
                select type).ToArray();

            QuestTargetTypeNames = QuestTargetTypes.Select(e => e.Name).ToArray();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.BeginHorizontal();

            SelectedQuestTargetTypeIndex =
                EditorGUILayout.Popup("", SelectedQuestTargetTypeIndex, QuestTargetTypeNames);

            if (GUILayout.Button("Add Quest Target"))
            {
                var questTarget =
                    (QuestTarget)Activator.CreateInstance(QuestTargetTypes[SelectedQuestTargetTypeIndex]);

                QuestData.questTargets ??= new List<QuestTarget>();
                QuestData.questTargets.Add(questTarget);
            }

            GUILayout.EndHorizontal();
        }
    }
}