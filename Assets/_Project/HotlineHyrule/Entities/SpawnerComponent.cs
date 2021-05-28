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
    public class SpawnerComponent : MonoBehaviour
    {
                
        [SerializeField] public List<GameObject> entities; 
        [SerializeField] public Tilemap spawnTilemap;       
        [SerializeField] public int maxEntities;

        [SerializeField] bool useWaves;
        [SerializeField] int waveLimit;
        [SerializeField] float waveTime;
        [SerializeField] [Range(0, 1)] float waveTimeOffset;
        [SerializeField] int entitiesPerWave;
        [SerializeField] [Range(0, 1)] float entitiesPerWaveOffset;
        
        int CurrentEntities { get; set; }
        int CurrentWave { get; set; }

        bool CanSpawn => CurrentEntities < maxEntities;
        
        void Start()
        {
            if (maxEntities <= 0)
            {
                Debug.LogWarning($"You need to set a maximum entity limit for the spawner {name}.");
                return;
            }

            if (entities == null || entities.Count == 0)
            {
                Debug.LogWarning($"You need to assign entity prefabs for the spawner {name}.");
                return;
            }

            if (!useWaves)
            {
                for (var i = 0; i <= maxEntities; i++)
                {
                    SpawnEntityAtRandom();
                }

                return;
            }
            
            StartCoroutine(WaveRoutine());
        }

        IEnumerator WaveRoutine()
        {
            while (CurrentWave < waveLimit || waveLimit == 0)
            {
                var maxOffset = (int)(entitiesPerWave * entitiesPerWaveOffset);
                var offset = Random.Range(-maxOffset, maxOffset + 1);

                if ((CurrentEntities + entitiesPerWave + offset) <= maxEntities)
                {
                    for (var i = 0; i < entitiesPerWave + offset; i++)
                    {
                        if (!CanSpawn) break;
                        SpawnEntityAtRandom();
                    }

                    CurrentWave += 1;
                }
                Debug.Log("Wave: " + CurrentWave);
                var maxTimeOffset = waveTime * waveTimeOffset;
                var timeOffset = Random.Range(-maxTimeOffset, maxTimeOffset);

                yield return new WaitForSeconds(waveTime + timeOffset);
            }
        }
        
        void SpawnEntityAtRandom()
        {
            var errors = 0;
            var bounds = spawnTilemap.cellBounds;
            
            var spawnPositionX = Random.Range(bounds.min.x, bounds.max.x);
            var spawnPositionY = Random.Range(bounds.min.y, bounds.max.y);
            var spawnPosition = new Vector3Int(spawnPositionX, spawnPositionY, 0);

            while (!spawnTilemap.HasTile(spawnPosition))
            {                
                errors += 1;
                
                spawnPositionX = Random.Range(bounds.min.x, bounds.max.x);
                spawnPositionY = Random.Range(bounds.min.y, bounds.max.y);
                spawnPosition = new Vector3Int(spawnPositionX, spawnPositionY, 0);

                if (errors > 50) return;
            }

            SpawnEntityAt(spawnPosition);
        }
        
        void SpawnEntityAt(Vector3Int position)
        {
            var entityIndex = Random.Range(0, entities.Count);
            var rotation = Random.Range(0, 360);
            
            var entity = Instantiate(
                entities[entityIndex],
                position.ToWorld(),
                Quaternion.Euler(0, 0, rotation)
            );
            
            var healthComponent = entity.GetComponent<HealthComponent>();
            if (healthComponent) healthComponent.HealthChanged += OnHealthChanged;
            
            CurrentEntities += 1;
        }

        void OnHealthChanged(object sender, HealthEventArgs e)
        {
            if (e.NewHealth != 0) return;
            
            CurrentEntities -= 1;
        }
    }
}


