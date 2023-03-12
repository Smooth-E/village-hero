using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{

    [SerializeField] private CharacterGrounder _grounder;
    public static Platform CurrentPlatform { private set; get; }
    public static Vector2 Position { private set; get; }
    public static List<Platform> ReachableFromPlatforms { private set; get; } = new List<Platform>();

    private void Awake() =>
        _grounder.OnGrounded += OnGrounded;

    private void Update()
    {
        Position = transform.position;
        GetReachablePlatforms();
    }
    
    private void OnDestroy() =>
        _grounder.OnGrounded -= OnGrounded;

    private void OnGrounded(Platform platform) =>
        CurrentPlatform = platform;

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        
        Gizmos.color = Color.cyan;
        foreach (var platform in ReachableFromPlatforms)
            Gizmos.DrawSphere(platform.transform.position, 1f);
    }

    private void GetReachablePlatforms()
    {
        ReachableFromPlatforms = new List<Platform>();
        
        for (var angle = 0; angle < 360; angle += 15)
        {
            var layerMask = LayerMask.GetMask(new string[]{ "Platform", "Platform Area" });
            var rayDirection = Quaternion.Euler(0, 0, angle) * Vector2.up;

            Debug.DrawRay(Position, rayDirection * 100f, Color.red);
            var hits = Physics2D.RaycastAll(Position, rayDirection, 100f, layerMask);

            for (var index = 0; index < hits.Length; index++)
            {
                Platform platform = null;
                
                if (!hits[index].collider.TryGetComponent<Platform>(out platform))
                    break;
                
                if (!ReachableFromPlatforms.Contains(platform))
                    ReachableFromPlatforms.Add(platform);
            }
        }

    }

}
