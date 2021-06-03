using System.Linq;
using HotlineHyrule.Entities;
using UnityEditor;
using UnityEngine;

namespace HotlineHyruleEditor
{
    [CustomEditor(typeof(SegmentComponent))]
    [CanEditMultipleObjects]
    public class SegmentComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // var segmentComponent = (SegmentComponent)target;
            // var nodes = segmentComponent.Nodes.ToList();
            //
            // GUILayout.BeginVertical();
            //
            // foreach (var node in nodes)
            // {
            //     GUILayout.BeginHorizontal();
            //     
            //     EditorGUILayout.LabelField($"{node.Index}");
            //     EditorGUILayout.LabelField($"{node.Position}");
            //     
            //     GUILayout.EndHorizontal();
            // }
            //
            // GUILayout.EndVertical();
        }
    }
}