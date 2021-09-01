using System;

namespace Misadventure.Level
{
    public class LevelFinishedEventArgs : EventArgs
    {
        public bool FinishGame { get; }

        public LevelFinishedEventArgs(bool finishGame = false)
        {
            FinishGame = finishGame;
        }
    }
}