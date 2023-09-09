using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
[ExecuteInEditMode]
public class Platform : MonoBehaviour
{

    [SerializeField] private BoxCollider2D _areaCollider;
    [SerializeField] private Collider2D _groundCollider;
    
    [FormerlySerializedAs("_possibleDestinations")] public List<PathFindingDestination> PossibleDestinations;

    public BoxCollider2D AreaCollider => _areaCollider;
    
    public float LeftEdge => transform.position.x + AreaCollider.offset.x - GetAreaWidth() / 2f;
    public float RightEdge => transform.position.x + AreaCollider.offset.x + GetAreaWidth() / 2f;
    public float Width => RightEdge - LeftEdge;
    public Vector3 LeftEdgePosition => GetGroundPositionWithX(LeftEdge);
    public Vector3 RightEdgePosition => GetGroundPositionWithX(RightEdge);

    private void OnDrawGizmos()
    {
        foreach (var destination in PossibleDestinations)
        {
            var destinationPlatform = destination.DestinationPlatform;

            if (destinationPlatform == null)
                continue;
            
            var destinationPosition = destination.DestinationPlatform.transform.position;
            destinationPosition.y = destination.DestinationPlatform.RightEdgePosition.y;
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
                    DrawArrow.ForGizmos(LeftEdgePosition, destinationPosition, Color.green);
                    break;
                
                case PathFindingAction.FallFromRightEdge:
                    DrawArrow.ForGizmos(RightEdgePosition, destinationPosition, new Color32(117, 133, 185, 255));
                    break;
                
                case PathFindingAction.JumpFromAnyEdge:
                    temporaryValue = new Vector3(RightEdge, destinationPosition.y, destinationPosition.z);
                    DrawArrow.ForGizmos(RightEdgePosition, temporaryValue, Color.magenta);
                    
                    temporaryValue = new Vector3(LeftEdge, destinationPosition.y, destinationPosition.z);
                    DrawArrow.ForGizmos(LeftEdgePosition, temporaryValue, Color.magenta);
                    
                    break;
                
                case PathFindingAction.JumpFromLeftEdge:
                    DrawArrow.ForGizmos(LeftEdgePosition, destinationPosition, Color.cyan);
                    break;
                
                case PathFindingAction.JumpFromRightEdge:
                    DrawArrow.ForGizmos(RightEdgePosition, destinationPosition, new Color32(255, 165, 0, 255));
                    break;
            }
        }
    }

    public PathFindingAction GetActionForDestination(Platform destinationPlatform)
    {
        PathFindingAction action = 0;
        bool platformFound = false;

        foreach (var destination in PossibleDestinations)
        {
            if (destination.DestinationPlatform != destinationPlatform)
                continue;
            
            action = destination.Action;
            platformFound = true;
            break;
        }

        if (!platformFound)
            Debug.LogError("GetActionForDestination: Platform not found!");

        return action;
    }

    private float GetAreaWidth() =>
        _areaCollider.size.x * transform.localScale.x;

    private Vector3 GetGroundPositionWithX(float x)
    {
        var position = _groundCollider.transform.position + (Vector3)_groundCollider.offset;
        position.x = x;
        return position;
    }

}
