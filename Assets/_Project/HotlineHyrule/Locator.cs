using HotlineHyrule.Entities;
using HotlineHyrule.Graphics;
using HotlineHyrule.Level;
using HotlineHyrule.Pathfinding;

namespace HotlineHyrule
{
    /// <summary>
    /// Provides access to singleton objects and services.
    /// </summary>
    public static class Locator
    {
        /// <summary>
        /// The player's player component.
        /// </summary>
        public static PlayerComponent PlayerComponent { get; set; }

        /// <summary>
        /// The main camera's camera component.
        /// </summary>
        public static CameraComponent CameraComponent { get; set; }
        
        /// <summary>
        /// The current level's level component.
        /// </summary>
        public static LevelComponent LevelComponent { get; set; }

        /// <summary>
        /// The current level's nav component.
        /// </summary>
        public static NavComponent NavComponent { get; set; }
    }
}
