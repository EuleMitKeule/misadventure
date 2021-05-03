using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using HotlineHyrule.Level;

namespace HotlineHyrule.Entities
{
    public class RespawnEnemy : MonoBehaviour
    {
        [SerializeField] Vector3 spawnPos;
        [SerializeField] public GameObject enemyPrefab_rnd;
        [SerializeField] public int intEnemyMaxCount; //Enemy Prefabs-Anzahl muss angegeben werden     
        [SerializeField] public bool useLevelBounds;
        [SerializeField] public BoundsInt spawnBounds;
       
        // Start is called before the first frame update
        void Start()
        {
            /* Static Spawn
            spawnPos = new Vector3(7, 3, 0);
            Debug.Log("SpawnPos: " + spawnPos.x + ", " + spawnPos.y);
            Instantiate(enemyPrefab_rnd, spawnPos, Quaternion.identity);*/

           

            //
            if (intEnemyMaxCount<= 0)
            {
                Debug.Log("Error Anazahl Gegner nicht angegeben! Gegneranzahl: " + intEnemyMaxCount);
            }
            else
            {

                BoundsInt bounds = useLevelBounds ? Locator.LevelComponent.LevelBounds() : spawnBounds;

                for (int i = 0; i <= intEnemyMaxCount; i++)
                {
                    SpawnRandomEnemyPrefeb(bounds);
                }
            }
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        void SpawnRandomEnemyPrefeb(BoundsInt spawnBounds)
        {
            int intSpawnPointX;
            int intSpawnPointY;
            int errorCount = 0;
            float rotation;

            LevelComponent levelComponent = Locator.LevelComponent;

            intSpawnPointX = Random.Range(spawnBounds.min.x, spawnBounds.max.x);
            intSpawnPointY = Random.Range(spawnBounds.min.y, spawnBounds.max.y);

            Vector3Int spawnPosition = new Vector3Int(intSpawnPointX, intSpawnPointY, 0);

            while (levelComponent.IsWall(spawnPosition))
            {                
                intSpawnPointX = Random.Range(spawnBounds.min.x, spawnBounds.max.x);
                intSpawnPointY = Random.Range(spawnBounds.min.y, spawnBounds.max.y);
                spawnPosition = new Vector3Int(intSpawnPointX, intSpawnPointY, 0);
                errorCount += 1;

                if(errorCount > 15)
                {
                    break;
                }
            }

            // Instanz von Enemy-prefab an einer randomisierten Position spawnen ohne Rotation.
            // mit Rotation freischalten spawnPoints[spawnPointIndex].rotation
            rotation = Random.Range(0, 360);
            Instantiate(enemyPrefab_rnd, spawnPosition, Quaternion.Euler(0,0,rotation));
        }
    }
}


