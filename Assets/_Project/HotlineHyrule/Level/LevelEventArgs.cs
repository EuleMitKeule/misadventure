using System;
using HotlineHyrule.Entities;

namespace HotlineHyrule.Level
{
    public class LevelEventArgs : EventArgs
    {
        public LevelData LevelData { get; }
        public PlayerStateData PlayerStateData { get; }
        public bool IsMenu { get; }

        public LevelEventArgs(LevelData levelData, PlayerStateData playerStateData, bool isMenu) =>
            (LevelData, PlayerStateData, IsMenu) = (levelData, playerStateData, isMenu);

    }
}