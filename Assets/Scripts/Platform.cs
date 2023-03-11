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

    private void Awake()
    {
        AreaCollider = GetComponent<BoxCollider2D>();
        var colliderSize = AreaCollider.size;
        var areaWidth = colliderSize.x * transform.lossyScale.x;
        LeftEdge = transform.position.x - areaWidth / 2f;
        RightEdge = transform.position.x + areaWidth / 2f;
    }

}
