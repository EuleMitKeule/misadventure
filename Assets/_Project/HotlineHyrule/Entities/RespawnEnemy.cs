using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Random = UnityEngine.Random;

namespace HotlineHyrule.Entities
{
    public class RespawnEnemy : MonoBehaviour
    {
        [SerializeField] Vector3 spawnPos;
        [SerializeField] GameObject enemyPrefab_rnd;
        public int intEnemyMaxCount; //Enemy Prefabs-Anzahl muss angegeben werden
       
        // Start is called before the first frame update
        void Start()
        {
            /* Static Spawn
            spawnPos = new Vector3(7, 3, 0);
            Debug.Log("SpawnPos: " + spawnPos.x + ", " + spawnPos.y);
            Instantiate(enemyPrefab_rnd, spawnPos, Quaternion.identity);*/
            
            //
            if(intEnemyMaxCount<= 0)
            {
                Debug.Log("Error Anazahl Gegner nicht angegeben! Gegneranzahl: " + intEnemyMaxCount);
            }
            else
            {
                for (int i = 0; i <= intEnemyMaxCount; i++)
                {
                    SpawnRandomEnemyPrefeb();
                }
            }
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        void SpawnRandomEnemyPrefeb()
        {
            int intSpawnPointX = Random.Range(-10, 10);
            int intSpawnPointY = Random.Range(-15, 15);
            Vector3 spawnPosition = new Vector3(intSpawnPointX, intSpawnPointY, 0);

            // Instanz von Enemy-prefab an einer randomisierten Position spawnen ohne Rotation.
            // mit Rotation freischalten spawnPoints[spawnPointIndex].rotation
            Instantiate(enemyPrefab_rnd, spawnPosition, Quaternion.identity);
        }
    }
}


