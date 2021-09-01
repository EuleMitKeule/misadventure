using System;
using System.Collections.Generic;
using Misadventure.Extensions;
using UnityEngine;

namespace Misadventure.Pathfinding
{
    public class PathfindingComponent : MonoBehaviour
    {
        /// <summary>
        /// The distance to cell center points that needs to be undershot for the cell to count as reached.
        /// </summary>
        [SerializeField] public float travelThreshold;

        /// <summary>
        /// Whether the entity currently has a waypoint to be reached.
        /// </summary>
        public bool HasWaypoint { get; private set; }
        /// <summary>
        /// The cell position the entity is currently located at.
        /// </summary>
        Vector3Int CurrentCell { get; set; }
        /// <summary>
        /// The next cell position that needs to be reached.
        /// </summary>
        Vector3Int CurrentWaypoint { get; set; }
        /// <summary>
        /// The path that is currently processed by the entity.
        /// </summary>
        List<Vector3Int> CurrentPath { get; set; }

        /// <summary>
        /// The direction vector towards the current waypoint.
        /// </summary>
        public Vector3 CurrentDirection =>
            HasWaypoint ? (CurrentWaypoint.ToWorld() - transform.position).normalized : Vector3.zero;
        /// <summary>
        /// Whether the entity is currently located at its next waypoint.
        /// </summary>
        bool IsAtWaypoint => (transform.position - CurrentWaypoint.ToWorld()).magnitude <= travelThreshold;

        /// <summary>
        /// Invoked when a new destination is assigned.
        /// </summary>
        public event EventHandler<CellEventArgs> DestinationChanged;
        /// <summary>
        /// Invoked when the current destination is reached.
        /// </summary>
        public event EventHandler<CellEventArgs> DestinationReached;
        /// <summary>
        /// Invoked when the entity's cell position has changed.
        /// </summary>
        public event EventHandler<CellEventArgs> CellPositionChanged;

        void Awake()
        {
            CellPositionChanged += OnCellPositionChanged;
        }

        void Start()
        {
            CurrentCell = Locator.LevelComponent.Grid.WorldToCell(transform.position);
        }

        void Update()
        {
            UpdateCell();
        }

        void OnCellPositionChanged(object sender, CellEventArgs e)
        {
            if (!HasWaypoint) return;
            if (!IsAtWaypoint) return;

            if (CurrentPath.Count == 0)
            {
                HasWaypoint = false;

                DestinationReached?.Invoke(this, new CellEventArgs(CurrentWaypoint));

                return;
            }

            CurrentWaypoint = CurrentPath[0];
            CurrentPath.RemoveAt(0);
        }

        /// <summary>
        /// Generates the path from current to given cell position.
        /// </summary>
        /// <param name="destinationCell">The path's destination cell position</param>
        public void SetDestination(Vector3Int destinationCell)
        {
            CurrentPath = Pathfinder.FindPath(CurrentCell, destinationCell);

            if (CurrentPath == null) return;

            if (CurrentPath.Count == 0)
            {
                DestinationReached?.Invoke(this, new CellEventArgs(destinationCell));
                return;
            }

            CurrentWaypoint = CurrentPath[0];
            CurrentPath.RemoveAt(0);
            HasWaypoint = true;

            OnCellPositionChanged(this, new CellEventArgs(CurrentCell));

            DestinationChanged?.Invoke(this, new CellEventArgs(destinationCell));
        }

        /// <summary>
        /// Stops the current path traversal process.
        /// </summary>
        public void ClearDestination()
        {
            HasWaypoint = false;
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