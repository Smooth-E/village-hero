using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{

    public static PlatformArea CurrentPlatform { private set; get; }
    public static Vector2 Position { private set; get; }
    public static List<PlatformArea> ReachableFromPlatformAreas { private set; get; } = new List<PlatformArea>();

    [SerializeField] private CharacterGrounder _grounder;
    [SerializeField] private bool _drawDebugRays = true;

    private void Awake() =>
        _grounder.OnGrounded += OnGrounded;

    private void Update()
    {
        Position = transform.position;
        GetReachablePlatforms();
    }
    
    private void OnDestroy() =>
        _grounder.OnGrounded -= OnGrounded;

    private void OnGrounded(PlatformArea platformArea) =>
        CurrentPlatform = platformArea;

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        
        Gizmos.color = Color.cyan;
        foreach (var platform in ReachableFromPlatformAreas)
            Gizmos.DrawSphere(platform.transform.position, 0.3f);
    }

    private void GetReachablePlatforms()
    {
        ReachableFromPlatformAreas = new List<PlatformArea>();
        
        for (var angle = 0; angle < 360; angle += 5)
        {
            var layerMask = LayerMask.GetMask(new string[]{ "Platform Area", "Obstacle" });
            var rayDirection = Quaternion.Euler(0, 0, angle) * Vector2.up;

            if (_drawDebugRays && angle % 3 == 0)
                Debug.DrawRay(Position, rayDirection * 100f, Color.red);
            
            var hits = Physics2D.RaycastAll(Position, rayDirection, 100f, layerMask);

            for (var index = 0; index < hits.Length; index++)
            {
                if (hits[index].collider.CompareTag("Obstacle"))
                    break;
                
                PlatformArea platformArea = hits[index].collider.GetComponent<PlatformArea>();
                if (!ReachableFromPlatformAreas.Contains(platformArea))
                    ReachableFromPlatformAreas.Add(platformArea);
            }
        }

    }

}
