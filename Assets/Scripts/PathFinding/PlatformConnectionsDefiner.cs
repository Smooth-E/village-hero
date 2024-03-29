﻿using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace PathFinding
{
    [ExecuteInEditMode]
    public class PlatformConnectionsDefiner : MonoBehaviour
    {
    
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
        private float InitialVerticalVelocity => EntityParameters.JumpForce / EntityParameters.Mass;

        private float VerticalGravityAcceleration => Mathf.Abs(Physics2D.gravity.y) * EntityParameters.GravityScale;
    
        private Platform[] _platforms;

        private void Start() =>
            UpdatePlatformList();

#if UNITY_EDITOR
        private void Update() 
        {
            if (DebuggingFlags.ContinuouslyRefreshPlatformList)
                UpdatePlatformList();
        }
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
                var horizontal = EntityParameters.MovementVelocity;
                var vertical = InitialVerticalVelocity;
            
                var fallFromRightEdge = 
                    JumpFromEdge(platform, right, horizontal, 0, PathFindingAction.FallFromRightEdge);
            
                var fallFromLeftEdge = 
                    JumpFromEdge(platform, left, -horizontal, 0, PathFindingAction.FallFromLeftEdge);
            
                if (fallFromLeftEdge != null && fallFromRightEdge == fallFromLeftEdge)
                    connections.Add(new(platform, PathFindingAction.FallFromAnyEdge));
            
                AddToListIfNotNull(connections, fallFromRightEdge);
                AddToListIfNotNull(connections, fallFromLeftEdge);
            
                var jumpFromRightEdge = 
                    JumpFromEdge(platform, right, horizontal, vertical, PathFindingAction.JumpFromRightEdge);
            
                var jumpFromLeftEdge = 
                    JumpFromEdge(platform, left, -horizontal, vertical, PathFindingAction.JumpFromLeftEdge);
            
                if (jumpFromLeftEdge != null && jumpFromLeftEdge == jumpFromRightEdge)
                    connections.Add(new(platform, PathFindingAction.JumpFromAnyEdge));
            
                AddToListIfNotNull(connections, jumpFromRightEdge);
                AddToListIfNotNull(connections, jumpFromLeftEdge);
            
                connections.AddRange(JumpAnywhereUnder(platform));

                platform.PossibleDestinations = connections;
            }
        }
    
        private PathFindingDestination JumpFromEdge(
            Platform platform,
            Vector3 edgePosition,
            float horizontalVelocity, 
            float initialVerticalVelocity, 
            PathFindingAction action)
        {
            PathFindingDestination destination = null;
            var highestPlatformY = float.NegativeInfinity;

            foreach (var otherPlatform in _platforms)
            {
                if (platform == otherPlatform)
                    continue;
            
                var otherRightEdge = otherPlatform.RightEdgePosition;
                var otherLeftEdge = otherPlatform.LeftEdgePosition;
                var x = GetXForAttitude(horizontalVelocity, initialVerticalVelocity, edgePosition, otherRightEdge.y);

                if (x <= float.NegativeInfinity)
                    continue;

                if (otherLeftEdge.x < x && otherRightEdge.x > x && otherRightEdge.y > highestPlatformY)
                {
                    destination = new(otherPlatform, action);
                    highestPlatformY = otherLeftEdge.y;
                }
            }
        
            return destination;
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

        private void AddToListIfNotNull(List<PathFindingDestination> list, PathFindingDestination destination)
        {
            if (destination != null)
                list.Add(destination);
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
            var b =  -verticalVelocity;
            var c = desiredY - startPosition.y;
            var discriminant = b * b - 4 * a * c;

            // Object will never reach the desired attitude
            if (discriminant < 0)
                return float.NegativeInfinity;

            var root = (float)Math.Sqrt(discriminant);
            var time = Mathf.Max((-b + root) / 2 / a, (-b - root) / 2 / a);

            return startPosition.x + horizontalVelocity * time;
        }
    
    }
}
