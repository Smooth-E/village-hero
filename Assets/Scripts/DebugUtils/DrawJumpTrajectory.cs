using PathFinding;
using UnityEngine;

namespace DebugUtils
{
    public class DrawJumpTrajectory : MonoBehaviour
    {
    
        private readonly float _entityMass = 1f;
        private readonly float _entityJumpForce = 30f;
        private readonly float _entityMovementVelocity = 10f;
    
        private float InitialVerticalVelocity => _entityJumpForce / _entityMass;
    
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
        
        
            var platform = GetComponent<Platform>();
            Gizmos.DrawSphere(platform.RightEdgePosition, 0.1f);

            var point = CalculateForTime(0) + platform.RightEdgePosition;
            for (float x = -100; x < 1000; x += 0.1f)
            {
                var newPoint = CalculateForTime(x) + platform.RightEdgePosition;
                Gizmos.DrawLine(point, newPoint);
                point = newPoint;
            }

            Gizmos.color = (Color.blue);
            point = CalculateForTime2(0) + platform.LeftEdgePosition;
            for (float x = -100; x < 1000; x += 0.1f)
            {
                var newPoint = CalculateForTime2(x) + platform.LeftEdgePosition;
                Gizmos.DrawLine(point, newPoint);
                point = newPoint;
            }
        }

        private Vector3 CalculateForTime(float time) =>
            new(_entityMovementVelocity * time,
                InitialVerticalVelocity * time - -Physics2D.gravity.y * 10 * time * time / 2, 0);
    
        private Vector3 CalculateForTime2(float time) =>
            new(-_entityMovementVelocity * time,
                InitialVerticalVelocity * time - -Physics2D.gravity.y * 10 * time * time / 2, 0);
    }
}
