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
        [BoxGroup("General")]
        [ShowInInspector]
        [LabelText("Auto Complete")]
        public bool finishLevelOnCompletion;
        [BoxGroup("General")]
        [ShowInInspector]
        [LabelText("Finishes Game")]
        public bool finishGameOnCompletion;
        [BoxGroup("General")]
        [TextArea]
        [ShowInInspector]
        [LabelText("Intro Text")]
        public string questText;
        [BoxGroup("General")]
        [TextArea]
        [ShowInInspector]
        [LabelText("Additional Intro Text")]
        public string questTargetText;
        [BoxGroup("General")]
        [ShowInInspector]
        [OdinSerialize]
        [NonSerialized]
        [ListDrawerSettings(ListElementLabelName = "shortTargetText", Expanded = true)]
        public List<QuestTarget> questTargets = new List<QuestTarget>();
        [BoxGroup("Rewards")]
        [ShowInInspector]
        public List<ConsumableItemData> questRewards;
        [LabelText("How Many")]
        [BoxGroup("Rewards")]
        [Range(0, 2)]
        [ShowInInspector]
        public int questRewardCount;

        public static QuestData Empty => CreateInstance<QuestData>();
    }
}