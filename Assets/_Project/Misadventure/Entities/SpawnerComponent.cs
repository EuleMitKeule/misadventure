using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Misadventure.Extensions;
using Misadventure.Weapons;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Misadventure.Entities
{
    public class SpawnerComponent : MonoBehaviour
    {
                
        [SerializeField] public List<GameObject> entities; 
        [SerializeField] public Tilemap spawnTilemap;       
        [SerializeField] public int maxEntities;
        [SerializeField] public bool spawnOnAwake;
        [SerializeField] GameObject parentObject;

        [SerializeField] bool useWaves;
        [SerializeField] int waveLimit;
        [SerializeField] float waveTime;
        [SerializeField] [Range(0, 1)] float waveTimeOffset;
        [SerializeField] int entitiesPerWave;
        [SerializeField] [Range(0, 1)] float entitiesPerWaveOffset;
        
        List<Vector3Int> SpawnTiles { get; set; }
        public int CurrentEntities { get; set; }
        int CurrentWave { get; set; }

        bool CanSpawn => CurrentEntities < maxEntities;
        
        void Start()
        {
            if (maxEntities <= 0)
            {
                Logging.LogWarning($"You need to set a maximum entity limit for the spawner {name}.");
                return;
            }

            if (entities == null || entities.Count == 0)
            {
                Logging.LogWarning($"You need to assign entity prefabs for the spawner {name}.");
                return;
            }

            SpawnTiles = GetSpawnTiles();
            
            if (!useWaves && spawnOnAwake)
            {
                SpawnEntitiesAtRandom(maxEntities);
            }
            else if(useWaves)
            {
                StartCoroutine(WaveRoutine());
            }                  
            
        }

        List<Vector3Int> GetSpawnTiles()
        {
            var tiles = new List<Vector3Int>();
            var bounds = spawnTilemap.cellBounds;

            for (var x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (var y = bounds.yMin; y < bounds.yMax; y++)
                {
                    var position = new Vector3Int(x, y, 0);
                    if (!spawnTilemap.HasTile(position)) continue;
                    
                    tiles.Add(position);
                }
            }

            return tiles;
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
                        SpawnEntitiesAtRandom();
                    }

                    CurrentWave += 1;
                }

                var maxTimeOffset = waveTime * waveTimeOffset;
                var timeOffset = Random.Range(-maxTimeOffset, maxTimeOffset);

                yield return new WaitForSeconds(waveTime + timeOffset);
            }
        }
        
        void SpawnEntitiesAtRandom(int count = 1)
        {
            var spawnPositions = new List<Vector3Int>();

            for (var i = 0; i < SpawnTiles.Count; i++)
            {
                var index = Random.Range(0, SpawnTiles.Count);
                var position = SpawnTiles[index];
                spawnPositions.Add(position);
            }

            for (var i = 0; i < count; i++)
            {
                var index = i % spawnPositions.Count;
                
                SpawnEntityAt(spawnPositions[index]);
            }
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
            if (parentObject) entity.transform.SetParent(parentObject.transform);
            
            var healthComponent = entity.GetComponent<HealthComponent>();
            if (healthComponent) healthComponent.HealthChanged += OnHealthChanged;
            
            CurrentEntities += 1;
        }

        public void SpawnAround(Vector3 position, int radius, int amount)
        {
            var cellposition = spawnTilemap.WorldToCell(position);
            var tiles = new List<Vector3Int>();

            for (int x = (-radius); x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    Vector3Int tempPosition = new Vector3Int(x, y, 0);

                    if (tempPosition == Vector3Int.zero) continue;
                    
                    float distance = Mathf.Sqrt(Mathf.Pow(x,2) + Mathf.Pow(y,2));

                    if (distance > radius) continue;

                    var actuallPostion = cellposition + tempPosition;

                    if (!spawnTilemap.HasTile(actuallPostion)) continue;

                    tiles.Add(actuallPostion);                    
                }
            }
            tiles = tiles.OrderBy(e => Guid.NewGuid()).ToList();

            int spawnAmount = Mathf.Min(tiles.Count, amount);

            for(int i=0; i < spawnAmount; i++)
            {
                if (!CanSpawn) break;

                var spawnPosition = tiles[i];

                var entityIndex = Random.Range(0, entities.Count);
                var rotation = Random.Range(0, 360);

                var entity = Instantiate(
                    entities[entityIndex],
                    spawnPosition.ToWorld(),
                    Quaternion.Euler(0, 0, rotation)
                );
                if (parentObject) entity.transform.SetParent(parentObject.transform);

                var weaponComponent = GetComponentInParent<WeaponComponent>();
                if (weaponComponent)
                {
                    var healthComponent = entity.GetComponent<HealthComponent>();
                    if (healthComponent) weaponComponent.RegisterConjuringCallback(healthComponent);
                }
                
                CurrentEntities += 1;

            }

        }

        void OnHealthChanged(object sender, HealthEventArgs e)
        {
            if (e.NewHealth != 0) return;
            
            CurrentEntities -= 1;
        }
    }
}


