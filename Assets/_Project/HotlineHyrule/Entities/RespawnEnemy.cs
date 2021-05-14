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
        [SerializeField] public int spawnObjectsMaxCount;
        
        [SerializeField] public bool useSpawnWaves;
        [SerializeField] public bool randomSpawnPositionPerWaves;

        [SerializeField] public int spawnWavesCount;
        [SerializeField] public int waveIntervallTime;
        [SerializeField] [Range(0, 1)] public float intervallRandomnes;
        [SerializeField] public int spawnObjectsPerWave;
        [SerializeField] [Range(0, 1)] public float spawnCounterRandomnes; //Gegner dürfen nicht an der selben Stelle innerhalb einer Welle gespawnd werden

        private int spwanedObjectsCount;
        private int wave;

        //Spawn per wave
        //spawn at time
        //waveintrvall mit %-Abweichung (randomisiert intervall +/- Abweichung als Grenze)
        //

        // Start is called before the first frame update
        void Start()
        {

            //Grenzen zum Spawnen festlegen:
            BoundsInt bounds = useLevelBounds ? Locator.LevelComponent.LevelBounds() : spawnTilemap.cellBounds;
            
            StartCoroutine(SpawnMaster(bounds));
        }

        // Update is called once per frame
        void Update()
        {
           
        }

        IEnumerator SpawnMaster(BoundsInt bounds)
        {
            //Wenn kein Max-Anzahl an Spawn-Objekte definiert
            if (spawnObjectsMaxCount <= 0)
            {
                yield return new WaitForSeconds(0.5f);
                Debug.LogWarning("Warning: Max-Anazahl Spawn-Objekte nicht angegeben! " + spawnObjectsMaxCount);
                
            }
            else
            {
                //Art von Spawnen waehlen
                if (!useSpawnWaves)
                {
                    //Randomisierte spawnen von maximaler Anzahl an Elementen-Objekten
                    Debug.Log("Spawn begint in 2 sec.");
                    yield return new WaitForSeconds(2);

                    for (int i = 0; i <= spawnObjectsMaxCount; i++)
                    {                       
                        SpawnRandomEnemyPrefeb(bounds);
                        Debug.Log("Spawn enemy: " + i);

                    }
                }
                else
                {
                    //Wellenartige Spawnen
                    if (spawnWavesCount > 0)
                    {
                        Debug.Log("Wavespawn begint in 2 sec.");
                        yield return new WaitForSeconds(2);

                        while (wave < spawnWavesCount)
                        {
                            if ((spwanedObjectsCount+ spawnObjectsPerWave) <= spawnObjectsMaxCount)
                            {
                                for (int i = 0; i < spawnObjectsPerWave; i++)
                                {

                                    SpawnRandomEnemyPrefeb(bounds);
                                    Debug.Log("Spawn enemy: " + spwanedObjectsCount);
                                }
                                wave += 1;
                            }                            
                            Debug.Log("Wave: " + wave);
                            yield return new WaitForSeconds(waveIntervallTime);
                            
                        }
                        yield return null;
                    }
                    else
                    {
                        Debug.LogWarning("Warning: Spawn Wavescounter ist nicht gesetzt!");
                    }
                    

                }
                                                              
            }
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
            spwanedObjectsCount += 1;
            var healthComponent = spawnObj.GetComponent<HealthComponent>();
            healthComponent.HealthChanged += onHealthChanged;
        }

        private void onHealthChanged(object sender, HealthEventArgs e)
        {
            if(e.NewHealth == 0){
                spwanedObjectsCount -= 1;
                Debug.Log("Kill");
                Debug.Log("Enemy count: " + spwanedObjectsCount);
            }
        }
    }
}


