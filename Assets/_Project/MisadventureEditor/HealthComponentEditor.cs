using HotlineHyrule.Entities;
using UnityEditor;
using UnityEngine;

namespace HotlineHyruleEditor
{
    [CustomEditor(typeof(HealthComponent))]
    public class HealthComponentEditor : Editor
    {
        int _healthToSet;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            base.OnInspectorGUI();
            
            var healthComponent = (HealthComponent)target;
            var health = healthComponent.Health;
            
            GUILayout.Label($"Current Health: {health}");
            
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Kill")) healthComponent.Health = 0;
            if (GUILayout.Button("-20")) healthComponent.Health -= 20;
            if (GUILayout.Button("+20")) healthComponent.Health += 20;
            if (GUILayout.Button("Heal Full")) healthComponent.Health = healthComponent.maxHealth;
            
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Set Health")) healthComponent.Health = _healthToSet;
            _healthToSet = EditorGUILayout.IntField(_healthToSet);
            
            GUILayout.EndHorizontal();
        }
    }
}