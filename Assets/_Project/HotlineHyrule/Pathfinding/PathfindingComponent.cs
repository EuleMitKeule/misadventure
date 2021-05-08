using System;
using System.Collections.Generic;
using HotlineHyrule.Entities;
using HotlineHyrule.Extensions;
using UnityEngine;

namespace HotlineHyrule.Pathfinding
{
    public class PathfindingComponent : MonoBehaviour
    {
        [SerializeField] public bool hasWaypoint;
        [SerializeField] public Vector3Int currentWaypoint;
        [SerializeField] public List<Vector3Int> currentPath;
        [SerializeField] public float travelThreshold;

        Vector3Int CurrentCell { get; set; }
        public Vector3 CurrentDirection => hasWaypoint ? (currentWaypoint.ToWorld() - transform.position).normalized : Vector3.zero;
        bool IsAtWaypoint => (transform.position - currentWaypoint.ToWorld()).magnitude <= travelThreshold;

        public event EventHandler<CellEventArgs> DestinationChanged;
        public event EventHandler<CellEventArgs> DestinationReached;
        public event EventHandler<CellEventArgs> CellPositionChanged;

        void Awake()
        {
            CellPositionChanged += OnCellPositionChanged;
        }

        void Update()
        {
            UpdateCell();
        }

        void OnCellPositionChanged(object sender, CellEventArgs e)
        {
            if (!hasWaypoint) return;
            if (!IsAtWaypoint) return;

            if (currentPath.Count == 0)
            {
                hasWaypoint = false;

                DestinationReached?.Invoke(this, new CellEventArgs(currentWaypoint));

                return;
            }

            currentWaypoint = currentPath[0];
            currentPath.RemoveAt(0);
        }

        /// <summary>
        /// Generates the path from current to given cell position.
        /// </summary>
        /// <param name="destinationCell">The path's destination cell position</param>
        public void SetDestination(Vector3Int destinationCell)
        {
            UpdateCell();
            
            var navMap = Locator.NavComponent.NavMap;
            
            currentPath = Pathfinder.FindPath(CurrentCell, destinationCell, navMap);

            if (currentPath.Count == 0)
            {
                DestinationReached?.Invoke(this, new CellEventArgs(destinationCell));
                return;
            }

            currentWaypoint = currentPath[0];
            currentPath.RemoveAt(0);
            hasWaypoint = true;

            OnCellPositionChanged(this, new CellEventArgs(CurrentCell));

            DestinationChanged?.Invoke(this, new CellEventArgs(destinationCell));
        }
        
        /// <summary>
        /// Checks if the cell position of the entity has changed.
        /// </summary>
        void UpdateCell()
        {
            var currentCellPosition = Locator.LevelComponent.Grid.WorldToCell(transform.position);
            
            if ((transform.position - currentCellPosition.ToWorld()).magnitude <= travelThreshold)
            {
                if (CurrentCell != currentCellPosition)
                {
                    CurrentCell = currentCellPosition;
                    CellPositionChanged?.Invoke(this, new CellEventArgs(CurrentCell));
                }
            }
        }
    }
}