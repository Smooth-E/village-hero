using System;
using UnityEngine.Serialization;

[Serializable]
public class PathFindingDestination
{
    [FormerlySerializedAs("DestinationPlatformArea")] public Platform DestinationPlatform;
    public PathFindingAction Action;

    public PathFindingDestination() { }

    public PathFindingDestination(Platform platform, PathFindingAction action)
    {
        DestinationPlatform = platform;
        Action = action;
    }
    
}
