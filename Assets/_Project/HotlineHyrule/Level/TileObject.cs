using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HotlineHyrule.Level
{
    [Serializable]
    public struct TileObject
    {
        [SerializeField] public Tile tile;
        [SerializeField] public GameObject prefab;
    }
}