using System;
using HotlineHyrule.Entities;

namespace HotlineHyrule.Level
{
    public class LevelEventArgs : EventArgs
    {
        public LevelData LevelData { get; }
        public PlayerStateData PlayerStateData { get; }

        public LevelEventArgs(LevelData levelData, PlayerStateData playerStateData) =>
            (LevelData, PlayerStateData) = (levelData, playerStateData);
    }
}