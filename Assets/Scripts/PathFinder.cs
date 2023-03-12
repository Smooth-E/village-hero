using UnityEngine;
using System.Collections.Generic;

public static class PathFinder
{
    
    public static List<PathFindingNode> FindPath(PlatformArea start, PlatformArea destination)
    {
        PathFindingGraph.Initialize();

        var startNode = PathFindingGraph.Nodes[start];
        var destinationNode = PathFindingGraph.Nodes[destination];

        var openList = new List<PathFindingNode>();
        var visitedNodes = new HashSet<PathFindingNode>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            var currentNode = openList[0];

            for (var i = 1; i < openList.Count; i++)
            {
                var condition = 
                    openList[i].FCost < currentNode.FCost || 
                    openList[i].FCost == currentNode.FCost && 
                    openList[i].HCost < currentNode.HCost;
                
                if (condition)
                    currentNode = openList[i];
            }

            openList.Remove(currentNode);
            visitedNodes.Add(currentNode);

            if (currentNode == destinationNode)
                return CreateFinalPath(startNode, destinationNode);

            var neighboringNodes = currentNode.GetNeighboringNodes();
            Debug.Log($"Found neighboring nodes: {neighboringNodes.Count}");

            foreach (var neighboringNode in neighboringNodes)
            {
                if (visitedNodes.Contains(neighboringNode))
                    continue;
                
                // Мы не будем использовать вычисление Манхеттенской длины, 
                // так как нам не важно фактическое расстояние между нодами
                var moveCost = currentNode.GCost + 1;

                if (moveCost < neighboringNode.GCost || !openList.Contains(neighboringNode))
                {
                    neighboringNode.GCost = moveCost;
                    neighboringNode.HCost = 1;
                    neighboringNode.ParentNode = currentNode;

                    if (!openList.Contains(neighboringNode))
                        openList.Add(neighboringNode);
                }
            }
        }

        return null;
    }

    private static List<PathFindingNode> CreateFinalPath(PathFindingNode startNode, PathFindingNode destinationNode)
    {
        var path = new List<PathFindingNode>();
        var currentNode = destinationNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.ParentNode;
        }

        path.Add(startNode);
        path.Reverse();
        Debug.Log($"Final path length: {path.Count}");
        return path;
    }

}
