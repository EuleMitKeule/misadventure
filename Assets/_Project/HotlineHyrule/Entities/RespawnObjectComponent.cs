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
    public class RespawnObjectComponent : MonoBehaviour
    {
                
        [SerializeField] public List<GameObject> spawnObjectList; // Prefabs sind als liste und randomisiert aussuchen, 
        [SerializeField] public Tilemap spawnTilemap;
        [SerializeField] private bool useManualPosition; //useLevelBounds
        [SerializeField] Vector3Int spawnManualPosition;
        //[SerializeField] public BoundsInt spawnBounds;       
        [SerializeField] public int spawnObjectsMaxCount;
        
        [SerializeField] private bool useSpawnWaves;
        [SerializeField] private bool randomSpawnPositionPerWaves;

        [SerializeField] private int spawnWavesCount;
        [SerializeField] private float waveIntervallTime;
        [SerializeField] [Range(0, 1)] private float intervallRandomnes;
        [SerializeField] private int spawnObjectsPerWave;
        [SerializeField] [Range(0, 1)] private float spawnCounterRandomnes; //Gegner dürfen nicht an der selben Stelle innerhalb einer Welle gespawnd werden

        private int spawnedObjectsCount;
        private int wave;

        //Spawn per wave
        //spawn at time
        //waveintrvall mit %-Abweichung (randomisiert intervall +/- Abweichung als Grenze)
        //

        // Start is called before the first frame update
        void Start()
        {

            //Grenzen zum Spawnen festlegen:
            if (!useManualPosition)
            {
                StartCoroutine(SpawnMaster(spawnTilemap.cellBounds)); //
            }
            else
            {
                SpawnPrefebsOnPosition(spawnManualPosition);
            }

            //BoundsInt bounds = useManualPosition ? Locator.LevelComponent.LevelBounds() : spawnTilemap.cellBounds;
            //StartCoroutine(SpawnMaster(bounds));

        }

        // Update is called once per frame
        void Update()
        {
           
        }

        IEnumerator SpawnMaster(BoundsInt bounds)
        {
            float waveTime, waveTimeWeight;
            int spawnRandomCountPerWave;
            int randomSpawnWeight;

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
                        SpawnPrefebsOnRandomPosition(bounds);
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
                             
                            randomSpawnWeight = Convert.ToInt32(spawnObjectsPerWave * spawnCounterRandomnes);

                            spawnRandomCountPerWave = spawnCounterRandomnes != 0 ? Random.Range(spawnObjectsPerWave - randomSpawnWeight, spawnObjectsPerWave + randomSpawnWeight) : spawnObjectsPerWave;
                            Debug.Log("RandomCount to spawn: " + spawnRandomCountPerWave);

                            if ((spawnedObjectsCount+ spawnRandomCountPerWave) <= spawnObjectsMaxCount)
                            {
                                for (int i = 0; i < spawnRandomCountPerWave; i++)
                                {
                                    SpawnPrefebsOnRandomPosition(bounds);
                                    //Debug.Log("Spawn enemy: " + spwanedObjectsCount);
                                }
                                wave += 1;
                            }                            
                            Debug.Log("Wave: " + wave);
                                                        
                            waveTimeWeight = waveIntervallTime * intervallRandomnes;

                            waveTime = intervallRandomnes != 0 ? Random.Range(waveIntervallTime - waveTimeWeight, waveIntervallTime + waveTimeWeight) : waveIntervallTime;
                            Debug.Log("Spawn time: " + waveTime);

                            yield return new WaitForSeconds(waveTime);
                            
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

    

        void SpawnPrefebsOnRandomPosition(BoundsInt spawnBounds)
        {
            int intSpawnPointX;
            int intSpawnPointY;
            int errorCount = 0;
            float rotation;
            int prefabIndex;
                        

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

            // Instanz von Object-prefab an einer randomisierten Position spawnen ohne Rotation.
            // mit Rotation freischalten spawnPoints[spawnPointIndex].rotation
            prefabIndex = Random.Range(0, spawnObjectList.Count);
            rotation = Random.Range(0, 360);

            var spawnObj = Instantiate(spawnObjectList[prefabIndex], spawnPosition.ToWorld(), Quaternion.Euler(0,0,rotation));
            spawnedObjectsCount += 1;
            var healthComponent = spawnObj.GetComponent<HealthComponent>();
            healthComponent.HealthChanged += onHealthChanged;
        }


        void SpawnPrefebsOnPosition(Vector3Int spawnManualPostion)
        {
            /*int intSpawnPointX;
            int intSpawnPointY;
            int errorCount = 0;*/
            float rotation;

            LevelComponent levelComponent = Locator.LevelComponent;

            /*
            intSpawnPointX = Random.Range(spawnManualPostion.x, spawnManualPostion.x);
            intSpawnPointY = Random.Range(spawnManualPostion.y, spawnManualPostion.y);

            Vector3Int spawnPosition = new Vector3Int(intSpawnPointX, intSpawnPointY, 0);

            while (!spawnTilemap.HasTile(spawnPosition)) //levelComponent.IsWall(spawnPosition)
            {
                intSpawnPointX = spawnManualPostion.x; //Random.Range(spawnManualPostion.x, spawnManualPostion.x);
                intSpawnPointY = spawnManualPostion.y; //Random.Range(spawnManualPostion.y, spawnManualPostion.y);
                spawnPosition = new Vector3Int(intSpawnPointX, intSpawnPointY, 0);

                errorCount += 1;

                if (errorCount > 50)
                {
                    return;
                }
            }*/

            // Instanz von Enemy-prefab an einer randomisierten Position spawnen ohne Rotation.
            // mit Rotation freischalten spawnPoints[spawnPointIndex].rotation
            rotation = Random.Range(0, 360);
            var spawnObj = Instantiate(spawnObjectList[0], spawnManualPostion.ToWorld(), Quaternion.Euler(0, 0, rotation));
            spawnedObjectsCount += 1;
            var healthComponent = spawnObj.GetComponent<HealthComponent>();
            healthComponent.HealthChanged += onHealthChanged;
        }

        private void onHealthChanged(object sender, HealthEventArgs e)
        {
            if(e.NewHealth == 0){
                spawnedObjectsCount -= 1;
                Debug.Log("Kill");
                Debug.Log("Enemy count: " + spawnedObjectsCount);
            }
        }
    }
}


