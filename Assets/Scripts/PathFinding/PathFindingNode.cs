using System.Collections.Generic;

public class PathFindingNode
{
    
    public PlatformArea LinkedPlatformArea { private set; get; }
    public PathFindingNode ParentNode;
    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;

    private PathFinder _pathFinder;

    public PathFindingNode(PlatformArea platform, PathFinder pathFinder)
    {
        LinkedPlatformArea = platform;
        _pathFinder = pathFinder;
    }

    public List<PathFindingNode> GetNeighboringNodes()
    {
        var list = new List<PathFindingNode>();

        foreach (var destination in LinkedPlatformArea.PossibleDestinations)
            list.Add(_pathFinder.Graph[destination.DestinationPlatformArea]);

        return list;
    }

}
