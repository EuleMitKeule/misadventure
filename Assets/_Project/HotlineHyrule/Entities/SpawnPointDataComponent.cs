using System;
using HotlineHyrule.Level;
using UnityEngine;

namespace HotlineHyrule.Entities
{
    public class SpawnPointDataComponent : MonoBehaviour
    {
        /// <summary>
        /// Spawn Point Data object to overwrite the last spawn point there
        /// </summary>
        [SerializeField] SpawnPointData SpawnPointData;
        
        void Awake()
        {
            transform.position = SpawnPointData.lastSpawnPoint;
        }
    }
}