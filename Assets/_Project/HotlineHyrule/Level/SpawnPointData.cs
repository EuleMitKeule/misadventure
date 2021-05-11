using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HotlineHyrule.Level
{
    [CreateAssetMenu(fileName = "SpawnPointData", menuName = "SpawnPointData", order = 0)]
    public class SpawnPointData : ScriptableObject
    {
        /// <summary>
        /// Last spawn point that was stored in this vector.
        /// Will be used to determine the Player position on scene switch
        /// </summary>
        public Vector3 lastSpawnPoint;
    }
}