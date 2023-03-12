using UnityEngine;
using System.Collections.Generic;

public class PathFindingGraph : MonoBehaviour
{

    public static Dictionary<PlatformArea, PathFindingNode> Nodes { private set; get; }

    public static void Initialize()
    {
        Nodes = new Dictionary<PlatformArea, PathFindingNode>();

        foreach (var platform in FindObjectsOfType<PlatformArea>())
            Nodes.Add(platform, new PathFindingNode(platform));
    }

}
