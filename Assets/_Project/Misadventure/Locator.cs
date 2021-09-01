using HotlineHyrule.Entities;
using HotlineHyrule.Graphics;
using HotlineHyrule.Level;
using HotlineHyrule.Pathfinding;
using HotlineHyrule.Quests;
using HotlineHyrule.Sound;

namespace HotlineHyrule
{
    /// <summary>
    /// Provides access to singleton objects and services.
    /// </summary>
    public static class Locator
    {
        /// <summary>
        /// The current game component.
        /// </summary>
        public static GameComponent GameComponent { get; set; }

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

        /// <summary>
        /// The current level's sound component.
        /// </summary>
        public static SoundComponent SoundComponent { get; set; }

        /// <summary>
        /// The current level's quest component.
        /// </summary>
        public static QuestComponent QuestComponent { get; set; }
    }
}
