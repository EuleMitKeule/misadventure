using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HotlineHyrule.Pathfinding
{
    public static class Pathfinder
    {
        public static List<Vector3Int> FindPath(Vector3Int startPosition, Vector3Int endPosition, List<Vector3Int> navMap)
        {
            if (!navMap.Contains(startPosition))
            {
                return new List<Vector3Int>();
            }

            if (!navMap.Contains(endPosition))
            {
                return new List<Vector3Int>();
            }

            var nodes = (from position in navMap select new NavNode(position)).ToList();

            var startNode = nodes.Find(node => node.Position == startPosition);
            var endNode = nodes.Find(node => node.Position == endPosition);

            var openNodes = new List<NavNode>();
            var closedNodes = new List<NavNode>();

            var currentNode = startNode;

            openNodes.Add(startNode);

            while (openNodes.Count > 0)
            {
                currentNode = FindLowestCostNode(openNodes);

                openNodes.Remove(currentNode);
                closedNodes.Add(currentNode);

                if (currentNode.Equals(endNode)) return RetracePath(startNode, endNode);

                foreach (var neighbour in GetNeighbours(currentNode, nodes))
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

        public static List<Vector3Int> RetracePath(NavNode startNavNode, NavNode endNavNode)
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

        public static NavNode FindLowestCostNode(IEnumerable<NavNode> nodes)
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

        public static int GetDistance(NavNode from, NavNode to)
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

        public static List<NavNode> GetNeighbours(NavNode from, IEnumerable<NavNode> nodes)
        {
            var nodesList = nodes.ToList();
            var neighbours = new List<NavNode>();
            var walkableNeighbours = new List<NavNode>();

            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    var neighbour = nodesList.Find(node => node.Position == from.Position + new Vector3Int(x, y, 0));
                    neighbours.Add(neighbour);
                    
                    if (x == 0 && y == 0) continue;

                    if (nodesList.Contains(neighbour)) walkableNeighbours.Add(neighbour);
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
                var nodeA = nodesList.Find(node => node.Position == from.Position + a);
                var nodeB = nodesList.Find(node => node.Position == from.Position + b);
                
                if (nodeA == null || nodeB == null) walkableNeighbours.Remove(neighbour);
            }

            return walkableNeighbours;
        }
    }
}
