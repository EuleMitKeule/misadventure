using UnityEngine;

namespace HotlineHyrule.Entities
{
    public class CaterpillarNode
    {
        public int Index { get; }
        public Vector2 Position { get; }
        public float Rotation { get; }

        public CaterpillarNode(int index, Vector2 position, float rotation)
        {
            Index = index;
            Position = position;
            Rotation = rotation;
        }
    }
}