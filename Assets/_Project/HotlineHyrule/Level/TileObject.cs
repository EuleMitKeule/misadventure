using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HotlineHyrule.Level
{
    [Serializable]
    public struct TileObject
    {
        public Tile Tile;
        public GameObject Prefab;
    }
}