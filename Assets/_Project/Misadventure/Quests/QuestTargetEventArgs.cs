using System;

namespace HotlineHyrule.Quests
{
    public class QuestTargetEventArgs : EventArgs
    {
        public QuestTarget QuestTarget { get; }

        public QuestTargetEventArgs(QuestTarget questTarget) => QuestTarget = questTarget;
    }
}