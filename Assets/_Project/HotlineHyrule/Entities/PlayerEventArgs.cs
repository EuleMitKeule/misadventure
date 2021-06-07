using System;

namespace HotlineHyrule.Entities
{
    public class PlayerEventArgs : EventArgs
    {
        public PlayerComponent PlayerComponent { get; }
        
        public PlayerEventArgs(PlayerComponent playerComponent)
        {
            PlayerComponent = playerComponent;
        }
    }
}