using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider2D))]
public class PlatformArea : MonoBehaviour
{
    
    [FormerlySerializedAs("_possibleDestinations")] public List<PathFindingDestination> PossibleDestinations;

    public BoxCollider2D AreaCollider { private set; get; }
    public float LeftEdge { private set; get; }
    public float RightEdge { private set; get; }
    public float Width => RightEdge - LeftEdge;
    public Vector3 LeftEdgePosition => new Vector3(LeftEdge, transform.position.y, transform.position.z);
    public Vector3 RightEdgePosition => new Vector3(RightEdge, transform.position.y, transform.position.z);

    private void Awake()
    {
        AreaCollider = GetComponent<BoxCollider2D>();
        var colliderSize = AreaCollider.size;
        var areaWidth = colliderSize.x * transform.localScale.x;
        var areaOffset = AreaCollider.offset;
        LeftEdge = transform.position.x + areaOffset.x - areaWidth / 2f;
        RightEdge = transform.position.x + areaOffset.x + areaWidth / 2f;
    }

    private void OnDrawGizmos()
    {
        foreach (var destination in PossibleDestinations)
        {
            var destinationPlatform = destination.DestinationPlatformArea;

            if (destinationPlatform == null)
                continue;
            
            var destinationPosition = destination.DestinationPlatformArea.transform.position;
            var position = transform.position;

            Vector3 temporaryValue;
            switch (destination.Action)
            {
                case PathFindingAction.JumpAnywhereUnder:
                    DrawArrow.ForGizmos(position, destinationPosition, Color.blue);
                    break;
                
                case PathFindingAction.FallFromAnyEdge:
                    temporaryValue = new Vector3(RightEdge, destinationPosition.y, destinationPosition.z);
                    DrawArrow.ForGizmos(RightEdgePosition, temporaryValue, Color.yellow);
                    
                    temporaryValue = new Vector3(LeftEdge, destinationPosition.y, destinationPosition.z);
                    DrawArrow.ForGizmos(LeftEdgePosition, temporaryValue, Color.yellow);
                    
                    break;
                
                case PathFindingAction.FallFromLeftEdge:
                    DrawArrow.ForGizmos(LeftEdgePosition, destinationPlatform.RightEdgePosition, Color.green);
                    break;
                
                case PathFindingAction.FallFromRightEdge:
                    DrawArrow.ForGizmos(RightEdgePosition, destinationPlatform.LeftEdgePosition, Color.green);
                    break;
                
                case PathFindingAction.JumpFromAnyEdge:
                    temporaryValue = new Vector3(RightEdge, destinationPosition.y, destinationPosition.z);
                    DrawArrow.ForGizmos(RightEdgePosition, temporaryValue, Color.magenta);
                    
                    temporaryValue = new Vector3(LeftEdge, destinationPosition.y, destinationPosition.z);
                    DrawArrow.ForGizmos(LeftEdgePosition, temporaryValue, Color.magenta);
                    
                    break;
                
                case PathFindingAction.JumpFromLeftEdge:
                    DrawArrow.ForGizmos(LeftEdgePosition, destinationPlatform.RightEdgePosition, Color.cyan);
                    break;
                
                case PathFindingAction.JumpFromRightEdge:
                    DrawArrow.ForGizmos(RightEdgePosition, destinationPlatform.LeftEdgePosition, Color.cyan);
                    break;
            }
        }
    }

    public PathFindingAction GetActionForDestination(PlatformArea destinationPlatformArea)
    {
        PathFindingAction action = 0;
        bool platformFound = false;

        foreach (var destination in PossibleDestinations)
        {
            if (destination.DestinationPlatformArea != destinationPlatformArea)
                continue;
            
            action = destination.Action;
            platformFound = true;
            break;
        }

        if (!platformFound)
            Debug.LogError("GetActionForDestination: Platform not found!");

        return action;
    }

}
