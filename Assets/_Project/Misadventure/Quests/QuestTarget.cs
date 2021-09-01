using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace HotlineHyrule.Quests
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class QuestTarget
    {
        [BoxGroup("General")]
        [HideLabel]
        [ShowInInspector]
        [EnumToggleButtons]
        [OdinSerialize]
        RequiredType requiredType = RequiredType.Required;
        public bool IsRequired => requiredType == RequiredType.Required;
        [BoxGroup("General")]
        [LabelText("Short description")]
        [ShowInInspector]
        public string shortTargetText = "Do Something";
        [BoxGroup("General")]
        [TextArea]
        [LabelText("$TargetReachedTextLabel")]
        [ShowInInspector]
        public string targetReachedText = "We did something.";
        [HideIf("IsRequired")]
        [BoxGroup("General")]
        [TextArea]
        [LabelText("Outro Text (Unreached)")]
        [ShowInInspector]
        public string targetNotReachedText = "We didn't do something.";

        string TargetReachedTextLabel => $"Outro Text {(IsRequired ? "" : "(Reached)")}";

        enum RequiredType
        {
            Required = 1 << 1,
            Optional = 1 << 2,
        }
    }
}