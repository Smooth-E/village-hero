using UnityEngine;
using System.Collections.Generic;

public class PathFindingGraph : MonoBehaviour
{

    public static Dictionary<Platform, PathFindingNode> Nodes { private set; get; }

    public static void Initialize()
    {
        Nodes = new Dictionary<Platform, PathFindingNode>();

        foreach (var platform in FindObjectsOfType<Platform>())
            Nodes.Add(platform, new PathFindingNode(platform));
    }

}
