using System;
using System.Collections.Generic;
using HotlineHyrule.Items;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace HotlineHyrule.Quests
{
    [CreateAssetMenu(menuName = "Quest/New Quest")]
    [HideMonoScript]
    public class QuestData : SerializedScriptableObject
    {
        [TitleGroup("Settings")]
        [BoxGroup("Settings/General")]
        [ShowInInspector]
        [LabelText("Auto Complete")]
        public bool finishLevelOnCompletion;
        [BoxGroup("Settings/General")]
        [TextArea]
        [ShowInInspector]
        [LabelText("Intro Text")]
        public string questText;
        [BoxGroup("Settings/General")]
        [TextArea]
        [ShowInInspector]
        [LabelText("Additional Intro Text")]
        public string questTargetText;
        [BoxGroup("Settings/General")]
        [ShowInInspector]
        [OdinSerialize]
        [NonSerialized]
        [ListDrawerSettings(ListElementLabelName = "shortTargetText", Expanded = true)]
        public List<QuestTarget> questTargets = new List<QuestTarget>();
        [BoxGroup("Settings/Rewards")]
        [ShowInInspector]
        public List<ItemData> questRewards;
        [LabelText("How Many")]
        [BoxGroup("Settings/Rewards")]
        [Range(0, 2)]
        [ShowInInspector]
        public int questRewardCount;

        public static QuestData Empty => CreateInstance<QuestData>();
    }
}