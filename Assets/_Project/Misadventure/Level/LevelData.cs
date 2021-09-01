using System;
using Misadventure.Quests;
using Misadventure.Sound;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Misadventure.Level
{
    [CreateAssetMenu(menuName = "Level/New Level")]
    [HideMonoScript]
    public class LevelData : SerializedScriptableObject
    {
        [BoxGroup("Story")]
        [ShowInInspector]
        [LabelText("Name")]
        [LabelWidth(50)]
        public string areaName;

        [BoxGroup("Story")]
        [TextArea]
        [ShowInInspector]
        [LabelText("Intro Text")]
        public string areaText;

        [BoxGroup("Story")]
        [TextArea]
        [ShowInInspector]
        [LabelText("Outro Text")]
        public string areaFinishedText;

        [BoxGroup("Story")]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        [ShowInInspector]
        public QuestData questData;

        [BoxGroup("General")]
        [LabelText("Spawn Player At", true)]
        [ShowInInspector]
        public Vector3Int playerSpawnPosition;

        [BoxGroup("Effects")]
        [EnumToggleButtons]
        [HideLabel]
        [ShowInInspector]
        [OdinSerialize]
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

        [BoxGroup("Background Music")]
        [ShowInInspector]
        public BGMData bgmData;
    }
}