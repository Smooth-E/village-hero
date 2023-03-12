using System.Collections.Generic;

public class PathFindingNode
{

    public PlatformArea LinkedPlatformArea { private set; get; }
    public PathFindingNode ParentNode;
    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;

    public PathFindingNode(PlatformArea platform) =>
        LinkedPlatformArea = platform;

    public List<PathFindingNode> GetNeighboringNodes()
    {
        var list = new List<PathFindingNode>();

        foreach (var destination in LinkedPlatformArea.PossibleDestinations)
            list.Add(PathFindingGraph.Nodes[destination.DestinationPlatformArea]);

        return list;
    }

}
