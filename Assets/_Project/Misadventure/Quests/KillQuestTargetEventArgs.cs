using System;

namespace Misadventure.Quests
{
    public class KillQuestTargetEventArgs : EventArgs
    {
        public QuestTarget QuestTarget { get; }
        public int KillCount { get; }

        public KillQuestTargetEventArgs(QuestTarget questTarget, int killCount) =>
            (QuestTarget, KillCount) = (questTarget, killCount);
    }
}