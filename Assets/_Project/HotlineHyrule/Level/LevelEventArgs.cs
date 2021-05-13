using System;

namespace HotlineHyrule.Level
{
    public class LevelEventArgs : EventArgs
    {
        public LevelData LevelData { get; }

        public LevelEventArgs(LevelData levelData) => LevelData = levelData;
    }
}