using System.Collections.Generic;

public class PathFindingNode
{
    
    public Platform LinkedPlatform { private set; get; }
    public PathFindingNode ParentNode;
    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;

    private PathFinder _pathFinder;

    public PathFindingNode(Platform platform, PathFinder pathFinder)
    {
        LinkedPlatform = platform;
        _pathFinder = pathFinder;
    }

    public List<PathFindingNode> GetNeighboringNodes()
    {
        var list = new List<PathFindingNode>();

        foreach (var destination in LinkedPlatform.PossibleDestinations)
            list.Add(_pathFinder.Graph[destination.DestinationPlatform]);

        return list;
    }

}
