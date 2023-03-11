using System.Collections.Generic;

public class PathFindingNode
{

    public Platform LinkedPlatform { private set; get; }
    public PathFindingNode ParentNode;
    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;

    public PathFindingNode(Platform platform) =>
        LinkedPlatform = platform;

    public List<PathFindingNode> GetNeighboringNodes()
    {
        var list = new List<PathFindingNode>();

        foreach (var destination in LinkedPlatform.PossibleDestinations)
            list.Add(new PathFindingNode(destination.DestinationPlatform));

        return list;
    }

}
