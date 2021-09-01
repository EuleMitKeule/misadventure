using System;

namespace HotlineHyrule.Quests
{
    public class KillQuestTargetEventArgs : EventArgs
    {
        public QuestTarget QuestTarget { get; }
        public int KillCount { get; }

        public KillQuestTargetEventArgs(QuestTarget questTarget, int killCount) =>
            (QuestTarget, KillCount) = (questTarget, killCount);
    }
}