namespace Misadventure
{
    public static class PhysicsLayer
    {
        public const int PLAYER = 6;
        public const int ENEMY = 7;
        public const int PROJECTILE = 8;
        public const int WALL = 9;
        public const int ITEM = 10;
        public const int ENEMY_PROJECTILE = 11;

        /// <summary>
        /// Whether the layer is the wall layer.
        /// </summary>
        public static bool IsWall(this int layer) => layer == WALL;
        /// <summary>
        /// Whether the layer is the enemy layer.
        /// </summary>
        public static bool IsEnemy(this int layer) => layer == ENEMY;
        /// <summary>
        /// Whether the layer is the player layer;
        /// </summary>
        public static bool IsPlayer(this int layer) => layer == PLAYER;
        public static bool IsPlayerProjectile(this int layer) => layer == PROJECTILE;
    }
}