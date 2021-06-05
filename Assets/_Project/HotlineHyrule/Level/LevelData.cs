using System;
using HotlineHyrule.Quests;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HotlineHyrule.Level
{
    [CreateAssetMenu(menuName = "Level/New Level")]
    [HideMonoScript]
    public class LevelData : ScriptableObject
    {
        [TitleGroup("Settings")]
        [BoxGroup("Settings/Story")]
        [ShowInInspector]
        [LabelText("Name")]
        [LabelWidth(50)]
        public string areaName;

        [BoxGroup("Settings/Story")]
        [TextArea]
        [ShowInInspector]
        [LabelText("Intro Text")]
        public string areaText;

        [BoxGroup("Settings/Story")]
        [TextArea]
        [ShowInInspector]
        [LabelText("Outro Text")]
        public string areaFinishedText;

        [BoxGroup("Settings/Story")]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        [ShowInInspector]
        public QuestData questData;

        [BoxGroup("Settings/General")]
        [LabelText("Spawn Player At", true)]
        [ShowInInspector]
        public Vector3Int playerSpawnPosition;

        [BoxGroup("Settings/Effects")]
        [EnumToggleButtons]
        [HideLabel]
        [ShowInInspector]
        EffectType effectType;

        /// <summary>
        /// Whether to enable the rain effect.
        /// </summary>
        public bool IsRaining => effectType == EffectType.Rain;

        /// /// <summary>
        /// Whether to enable the snow effect.
        /// </summary>
        public bool IsSnowing => effectType == EffectType.Snow;

        [Flags]
        enum EffectType
        {
            Rain = 1 << 1,
            Snow = 1 << 2,
            All = Rain | Snow,
        }
    }
}