using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using HotlineHyrule.Level;
using HotlineHyrule.Extensions;
using UnityEngine.Tilemaps;

namespace HotlineHyrule.Entities
{
    public class RespawnEnemy : MonoBehaviour
    {
                
        [SerializeField] public List<GameObject> enemyPrefab_rnd; // Prefabs sind als liste und randomisiert aussuchen, 
        [SerializeField] public Tilemap spawnTilemap;
        [SerializeField] public bool useLevelBounds;
        [SerializeField] Vector3 spawnPos;
        //[SerializeField] public BoundsInt spawnBounds;       
        [SerializeField] public int enemyMaxCount;
        
        [SerializeField] public bool useSpawnWaves;
        [SerializeField] public bool randomSpawnPositionPerWaves;

        [SerializeField] public int spawnWavesCount;
        [SerializeField] [Range(0, 1)] public float intervallRandomnes;
        [SerializeField] public int spawnCountPerWave;
        [SerializeField] [Range(0, 1)] public float spawnCounterRandomnes; //Gegner dürfen nicht an der selben Stelle innerhalb einer Welle gespawnd werden

        private int enemyCount;

        //Spawn per wave
        //spawn at time
        //waveintrvall mit %-Abweichung (randomisiert intervall +/- Abweichung als Grenze)
        //

        // Start is called before the first frame update
        void Start()
        {
          
            //
            if (enemyMaxCount <= 0)
            {
                Debug.Log("Error Anazahl Gegner nicht angegeben! Gegneranzahl: " + enemyMaxCount);
            }
            else
            {

                // BoundsInt bounds = useLevelBounds ? Locator.LevelComponent.LevelBounds() : spawnBounds;

                BoundsInt bounds = useLevelBounds ? Locator.LevelComponent.LevelBounds() : spawnTilemap.cellBounds;

                for (int i = 0; i <= enemyMaxCount; i++)
                {
                    SpawnRandomEnemyPrefeb(bounds);
                }

                //Coroutine für die Wellen implementieren
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

            while (!spawnTilemap.HasTile(spawnPosition)) //levelComponent.IsWall(spawnPosition)
            {                
                intSpawnPointX = Random.Range(spawnBounds.min.x, spawnBounds.max.x);
                intSpawnPointY = Random.Range(spawnBounds.min.y, spawnBounds.max.y);
                spawnPosition = new Vector3Int(intSpawnPointX, intSpawnPointY, 0);
                

                errorCount += 1;

                if(errorCount > 50)
                {
                    return;
                }
            }

            // Instanz von Enemy-prefab an einer randomisierten Position spawnen ohne Rotation.
            // mit Rotation freischalten spawnPoints[spawnPointIndex].rotation
            rotation = Random.Range(0, 360);
            var spawnObj = Instantiate(enemyPrefab_rnd[0], spawnPosition.ToWorld(), Quaternion.Euler(0,0,rotation));
            enemyCount += 1;
            var healthComponent = spawnObj.GetComponent<HealthComponent>();
            healthComponent.HealthChanged += onHealthChanged;
        }

        private void onHealthChanged(object sender, HealthEventArgs e)
        {
            if(e.NewHealth == 0){
                enemyCount -= 1;
            }
        }
    }
}


