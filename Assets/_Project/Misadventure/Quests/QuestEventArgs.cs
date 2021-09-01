using System;

namespace HotlineHyrule.Quests
{
    public class QuestEventArgs : EventArgs
    {
        public QuestData QuestData { get; }

        public QuestEventArgs(QuestData questData) => QuestData = questData;
    }
}