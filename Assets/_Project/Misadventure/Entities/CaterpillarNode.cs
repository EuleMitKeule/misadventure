using UnityEngine;

namespace Misadventure.Entities
{
    public class CaterpillarNode
    {
        public int Index { get; }
        public Vector2 Position { get; }
        public Quaternion Rotation { get; }

        public CaterpillarNode(int index, Vector2 position, Quaternion rotation)
        {
            Index = index;
            Position = position;
            Rotation = rotation;
        }
    }
}