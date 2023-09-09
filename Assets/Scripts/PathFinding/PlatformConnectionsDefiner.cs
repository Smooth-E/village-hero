using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlatformConnectionsDefiner : MonoBehaviour
{
    
    // TODO: Define following constants inside a Singleton Scriptable Object
    private readonly float _entityMass = 1f;
    private readonly float _entityJumpForce = 30f;
    private readonly float _entityMovementVelocity = 10f;
    private readonly float _entityGravityScale = 10f;

    /// <summary>
    /// <para>
    /// Note regarding the initial velocity of a jump:
    /// Impulse is a change in momentum.
    /// Momentum is defined as m * u, where m is a mass and u is a velocity.
    /// </para>
    /// <para>
    /// Since when jumping the horizontal velocity of an entity is considered 0,
    /// the initial velocity for calculating the maximum jump height and
    /// all of the trajectories needed to find connections is force in Impulse mode divided by mass.
    /// </para>
    /// </summary>
    private float InitialVerticalVelocity => _entityJumpForce / _entityMass;

    private float VerticalGravityAcceleration => Mathf.Abs(Physics2D.gravity.y) * _entityGravityScale;
    
    private Platform[] _platforms;

    private void Start() =>
        UpdatePlatformList();

#if UNITY_EDITOR
    private void Update() =>
        UpdatePlatformList();
#endif
    
    private void UpdatePlatformList()
    {
        _platforms = FindObjectsOfType<Platform>(true);
        DefineConnections();
    }

    /// <summary>Assigns a list of all possible destinations (connections) for each platform.</summary>
    private void DefineConnections()
    {
        foreach (var platform in _platforms)
        {
            var connections = new List<PathFindingDestination>();
            var right = platform.RightEdgePosition;
            var left = platform.LeftEdgePosition;
            var horizontal = _entityMovementVelocity;
            var vertical = InitialVerticalVelocity;
            
            var fallFromRightEdge = JumpFromEdge(platform, right, horizontal, 0, PathFindingAction.FallFromRightEdge);
            var fallFromLeftEdge = JumpFromEdge(platform, left, -horizontal, 0, PathFindingAction.FallFromLeftEdge);

            var fallFromAnyEdge = new List<PathFindingDestination>();
            foreach (var rightConnection in fallFromRightEdge)
            {
                foreach (var leftConnection in fallFromLeftEdge)
                {
                    var rightPlatform = rightConnection.DestinationPlatform;
                    var leftPlatform = leftConnection.DestinationPlatform;
                    
                    if (rightPlatform == leftPlatform)
                        fallFromAnyEdge.Add(new(rightPlatform, PathFindingAction.JumpFromAnyEdge));
                }
            }
            
            connections.AddRange(fallFromRightEdge);
            connections.AddRange(fallFromLeftEdge);
            connections.AddRange(fallFromAnyEdge);
            
            connections.AddRange(JumpFromEdge(platform, right, horizontal, vertical, PathFindingAction.JumpFromRightEdge));
            connections.AddRange(JumpFromEdge(platform, left, -horizontal, vertical, PathFindingAction.JumpFromLeftEdge));
            connections.AddRange(JumpAnywhereUnder(platform));

            platform.PossibleDestinations = connections;
        }
    }
    
    private List<PathFindingDestination> JumpFromEdge(
        Platform platform,
        Vector3 edgePosition,
        float horizontalVelocity, 
        float initialVerticalVelocity, 
        PathFindingAction action)
    {
        var destinations = new List<PathFindingDestination>();

        foreach (var otherPlatform in _platforms)
        {
            if (platform == otherPlatform)
                continue;
            
            var otherRightEdge = otherPlatform.RightEdgePosition;
            var otherLeftEdge = otherPlatform.LeftEdgePosition;
            var x = GetXForAttitude(horizontalVelocity, initialVerticalVelocity, edgePosition, otherRightEdge.y);

            if (x <= float.NegativeInfinity)
                continue;
            
            if (otherLeftEdge.x < x && otherRightEdge.x > x)
                destinations.Add(new(otherPlatform, action));
        }
        
        return destinations;
    }
    
    private List<PathFindingDestination> JumpAnywhereUnder(Platform platform)
    {
        var destinations = new List<PathFindingDestination>();
        var maximumHeight = Mathf.Pow(InitialVerticalVelocity, 2) / (VerticalGravityAcceleration * 2);
        var rightEdge = platform.RightEdgePosition;
        var leftEdge = platform.LeftEdgePosition;
        
        foreach (var otherPlatform in _platforms)
        {
            if (platform == otherPlatform)
                continue;

            var otherRightEdge = otherPlatform.RightEdgePosition;
            var otherLeftEdge = otherPlatform.LeftEdgePosition;
            
            var isHigher = otherRightEdge.y < rightEdge.y + maximumHeight && otherRightEdge.y > rightEdge.y;
            var intersects = (otherRightEdge.x < rightEdge.x && otherRightEdge.x > leftEdge.x)
                             || (otherLeftEdge.x < rightEdge.x && otherLeftEdge.x > leftEdge.x);
            
            if (isHigher && intersects)
                destinations.Add(new(otherPlatform, PathFindingAction.JumpAnywhereUnder));
        }
        
        return destinations;
    }

    private float GetXForAttitude(
        float horizontalVelocity,
        float verticalVelocity,
        Vector3 startPosition,
        float desiredY) 
    {
        // y = vertical * time - g * t * t / 2
        // Finding the time means solving a quadratic polynomial
        // (g / 2) * t^2 - v * t + y = 0
        
        var a = VerticalGravityAcceleration / 2;
        var b =  verticalVelocity;
        var discriminant = b * b - 4 * a * desiredY;

        // Object will never reach the desired attitude
        if (discriminant < 0)
            return float.NegativeInfinity;

        var root = (float)Math.Sqrt(discriminant);
        var time = Mathf.Max((-b + root) / 2 / a, (-b - root) / 2 / a);

        return startPosition.x + horizontalVelocity * time;
    }
    
}
