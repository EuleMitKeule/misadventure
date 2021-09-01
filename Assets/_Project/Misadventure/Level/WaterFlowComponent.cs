using System.Collections;
using System.Collections.Generic;
using Misadventure.Entities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Misadventure.Level
{
    public class WaterFlowComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        float FlowTick { get; set; }
        [OdinSerialize]
        float DamageTick { get; set; }
        [OdinSerialize]
        int Damage { get; set; }
        [OdinSerialize]
        TileBase WaterTile { get; set; }
        [OdinSerialize]
        TileBase FlowTile { get; set; }
        [OdinSerialize]
        Tilemap DamTilemap { get; set; }
        [OdinSerialize]
        Tilemap WaterTilemap { get; set; }
        [OdinSerialize]
        Tilemap FlowTilemap { get; set; }

        List<Vector3Int> FlowPositions { get; } = new List<Vector3Int>();

        Coroutine FlowCoroutine { get; set; }

        float NextDamageTime { get; set; }

        void Awake()
        {
            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;

            foreach (var cellPosition in FlowTilemap.cellBounds.allPositionsWithin)
            {
                var tile = FlowTilemap.GetTile(cellPosition);
                if (!tile) continue;

                FlowPositions.Add(cellPosition);
            }

            StartFlowRoutine();
        }

        void StartFlowRoutine() => FlowCoroutine ??= StartCoroutine(FlowRoutine());

        IEnumerator FlowRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(FlowTick);

                var newFlowPositions = new List<Vector3Int>();

                foreach (var flowPosition in FlowPositions)
                {
                    if (HasDam(flowPosition)) continue;

                    var leftPosition = flowPosition + new Vector3Int(-1, 0, 0);
                    var rightPosition = flowPosition + new Vector3Int(1, 0,0);
                    var upPosition = flowPosition + new Vector3Int(0, 1, 0);
                    var downPosition = flowPosition + new Vector3Int(0, -1, 0);

                    var positions = new List<Vector3Int>() {leftPosition, rightPosition, upPosition, downPosition};

                    foreach (var position in positions)
                    {
                        if (IsObstructed(position)) continue;

                        WaterTilemap.SetTile(position, WaterTile);
                        FlowTilemap.SetTile(position, FlowTile);

                        newFlowPositions.Add(position);
                    }
                }

                FlowPositions.AddRange(newFlowPositions);
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (Time.time < NextDamageTime) return;
            NextDamageTime = Time.time + DamageTick;

            var healthComponent = other.GetComponent<HealthComponent>();
            if (!healthComponent) return;

            healthComponent.Health -= Damage;
        }

        bool IsObstructed(Vector3Int cellPosition)
        {
            var hasWater = WaterTilemap.GetTile(cellPosition);

            return HasDam(cellPosition) || hasWater || HasWall(cellPosition);
        }

        bool HasDam(Vector3Int cellPosition) => DamTilemap.GetTile(cellPosition);

        bool HasWall(Vector3Int cellPosition) => !Locator.NavComponent.NavMap.Contains(cellPosition);

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            GameComponent.LevelLoaded -= OnLevelLoaded;
            GameComponent.LevelUnloaded -= OnLevelUnloaded;
        }
    }
}