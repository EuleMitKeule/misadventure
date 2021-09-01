using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HotlineHyrule.Pathfinding
{
    public static class Pathfinder
    {
        public static Dictionary<Vector3Int, NavNode> NavMap { get; set; }

        /// <summary>
        /// Initializes the pathfinder with a nav map.
        /// </summary>
        /// <param name="navMap">The nav map to intialize with.</param>
        public static void InitializeNavMap(List<Vector3Int> navMap)
        {
            NavMap = new Dictionary<Vector3Int, NavNode>();
            
            foreach (var position in navMap)
            {
                var navNode = new NavNode(position);
                
                NavMap.Add(position, navNode);
            }
        }

        /// <summary>
        /// Calculates a traversable path from the start cell to the end cell.
        /// </summary>
        /// <param name="startPosition">The cell position to start the path from.</param>
        /// <param name="endPosition">The cell position to end the path at.</param>
        /// <returns>A list of cell positions to be traversed.</returns>
        public static List<Vector3Int> FindPath(Vector3Int startPosition, Vector3Int endPosition)
        {
            NavMap.TryGetValue(startPosition, out var startNode);
            NavMap.TryGetValue(endPosition, out var endNode);

            if (startNode == null || endNode == null) return new List<Vector3Int>();

            var openNodes = new HashSet<NavNode>();
            var closedNodes = new HashSet<NavNode>();

            var currentNode = startNode;
            
            openNodes.Add(currentNode);

            while (openNodes.Count > 0)
            {
                currentNode = FindLowestCostNode(openNodes);

                openNodes.Remove(currentNode);
                closedNodes.Add(currentNode);

                if (currentNode.Equals(endNode))
                {
                    var path = RetracePath(startNode, endNode);

                    foreach (var navNode in openNodes)
                    {
                        navNode.CostG = 0;
                        navNode.CostH = 0;
                        navNode.Parent = null;
                    }
                    foreach (var navNode in closedNodes)
                    {
                        navNode.CostG = 0;
                        navNode.CostH = 0;
                        navNode.Parent = null;
                    }

                    return path;
                }

                foreach (var neighbour in GetNeighbours(currentNode, NavMap))
                {
                    if (closedNodes.Contains(neighbour)) continue;

                    var newCostG = currentNode.CostG + GetDistance(currentNode, neighbour);
                    
                    if (newCostG < neighbour.CostG || !openNodes.Contains(neighbour))
                    {
                        neighbour.CostG = newCostG;
                        neighbour.CostH = GetDistance(neighbour, endNode);
                        neighbour.Parent = currentNode;

                        if (!openNodes.Contains(neighbour)) openNodes.Add(neighbour);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Transforms a path to a list of directions.
        /// </summary>
        /// <param name="from">The cell position to start from.</param>
        /// <param name="waypoints">The path to transform.</param>
        /// <returns>A list of direction vectors.</returns>
        public static List<Vector3Int> GetDirections(Vector3Int from, IEnumerable<Vector3Int> waypoints)
        {
            var directions = new List<Vector3Int>();
            var current = from;

            foreach (var waypoint in waypoints)
            {
                var direction = waypoint - current;
                directions.Add(direction);
                current += direction;
            }

            return directions;
        }

        /// <summary>
        /// Creates a path by traversing nodes via their parent node.
        /// </summary>
        /// <param name="startNavNode">The nav node the path starts from.</param>
        /// <param name="endNavNode">The nav the path ends at.</param>
        /// <returns>The retraced path.</returns>
        static List<Vector3Int> RetracePath(NavNode startNavNode, NavNode endNavNode)
        {
            var path = new List<Vector3Int>();
            var currentNode = endNavNode;

            while (!currentNode.Equals(startNavNode))
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.Parent;
            }

            path.Reverse();

            return path;
        }

        /// <summary>
        /// Finds the node with the lowest total cost value.
        /// </summary>
        /// <param name="nodes">The list of nodes to search.</param>
        /// <returns>The node with lowest total cost value.</returns>
        static NavNode FindLowestCostNode(IEnumerable<NavNode> nodes)
        {
            var nodeList = nodes.ToList();
            var lowestCostNode = nodeList[0];

            foreach (var node in nodeList)
            {
                if (node.CostF < lowestCostNode.CostF ||
                    node.CostF == lowestCostNode.CostF && node.CostH < lowestCostNode.CostH)
                {
                    lowestCostNode = node;
                }
            }

            return lowestCostNode;
        }

        /// <summary>
        /// Calculates the distance cost between to nodes.
        /// </summary>
        /// <param name="from">The node to start from.</param>
        /// <param name="to">The node to end at.</param>
        /// <returns>The distance cost value.</returns>
        static int GetDistance(NavNode from, NavNode to)
        {
            var connection = new Vector3Int(
                Mathf.Abs(to.Position.x - from.Position.x),
                Mathf.Abs(to.Position.y - from.Position.y), 0);

            var isLargerX = Mathf.Abs(connection.x) > Mathf.Abs(connection.y);

            var larger = isLargerX ? connection.x : connection.y;
            var smaller = isLargerX ? connection.y : connection.x;

            var distance = (larger - smaller) * 10 + smaller * 14;

            return distance;
        }

        /// <summary>
        /// Finds the nodes traversable neighbour nodes.
        /// </summary>
        /// <param name="from">The node to search.</param>
        /// <param name="nodes">A list of traversable nodes.</param>
        /// <returns>The list of neighbour nodes.</returns>
        static List<NavNode> GetNeighbours(NavNode from, Dictionary<Vector3Int, NavNode> nodes)
        {
            var neighbours = new List<NavNode>();
            var walkableNeighbours = new List<NavNode>();

            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    nodes.TryGetValue(from.Position + new Vector3Int(x, y, 0), out var neighbour);
                    neighbours.Add(neighbour);
                    
                    if (x == 0 && y == 0) continue;

                    if (neighbour != null) walkableNeighbours.Add(neighbour);
                }
            }

            for (var i = 0; i < neighbours.Count; i++)
            {
                var neighbour = neighbours[i];
                if (neighbour == null) continue;
                
                var x = -1 + i / 3;
                var y = -1 + i % 3;
                if (x == 0 || y == 0) continue;

                var a = new Vector3Int(x, 0, 0);
                var b = new Vector3Int(0, y, 0);
                var nodeA = walkableNeighbours.Find(node => node.Position == from.Position + a);
                var nodeB = walkableNeighbours.Find(node => node.Position == from.Position + b);
                
                if (nodeA == null || nodeB == null) walkableNeighbours.Remove(neighbour);
            }

            return walkableNeighbours;
        }
    }
}
