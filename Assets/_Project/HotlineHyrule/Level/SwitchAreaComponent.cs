using System;
using HotlineHyrule.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HotlineHyrule.Level
{
    public class SwitchAreaComponent : MonoBehaviour
    {
        /// <summary>
        /// Index of the target scene set in Build
        /// </summary>
        [SerializeField] int targetSceneIndex;
        /// <summary>
        /// The position that shall be stored in the Last spawn point data object.
        /// </summary>
        [SerializeField] Vector3 targetPosition;
        /// <summary>
        /// Data where the target position shall be stored
        /// </summary>
        [SerializeField] SpawnPointData spawnPointData;

        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("player")) return;

            SceneManager.LoadScene(targetSceneIndex);
            spawnPointData.lastSpawnPoint = targetPosition;
        }
    }
    
}