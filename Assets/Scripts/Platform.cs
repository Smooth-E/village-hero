using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(BoxCollider2D))]
public class Platform : MonoBehaviour
{

    [SerializeField] private List<PathFindingDestination> _possibleDestinations;

    public BoxCollider2D AreaCollider { private set; get; }
    public List<PathFindingDestination> PossibleDestinations => _possibleDestinations;
    public float LeftEdge { private set; get; }
    public float RightEdge { private set; get; }
    public float Width => RightEdge - LeftEdge;

    private void Awake()
    {
        AreaCollider = GetComponent<BoxCollider2D>();
        var colliderSize = AreaCollider.size;
        var areaWidth = colliderSize.x * transform.localScale.x;
        LeftEdge = transform.position.x - areaWidth / 2f;
        RightEdge = transform.position.x + areaWidth / 2f;
    }

    public PathFindingAction GetActionForDestination(Platform destinationPlatform)
    {
        PathFindingAction action = 0;
        bool platformFound = false;

        foreach (var destination in _possibleDestinations)
        {
            if (destination.DestinationPlatform != destinationPlatform)
                continue;
            
            action = destination.Action;
            platformFound = true;
            break;
        }

        if (!platformFound)
            Debug.LogError("GetActionForDestination: Платформа не найдена!");

        return action;
    }

}
