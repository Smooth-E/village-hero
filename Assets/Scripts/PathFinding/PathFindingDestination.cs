using System;

[Serializable]
public class PathFindingDestination
{
    public PlatformArea DestinationPlatformArea;
    public PathFindingAction Action;

    public PathFindingDestination() { }

    public PathFindingDestination(PlatformArea platformArea, PathFindingAction action)
    {
        DestinationPlatformArea = platformArea;
        Action = action;
    }
    
}
