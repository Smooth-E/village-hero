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

    private void GetReachablePlatforms()
    {
        ReachableFromPlatforms = new List<Platform>();
        
        for (var angle = 0; angle < 360; angle += 15)
        {
            Debug.DrawRay(Position, Quaternion.Euler(0, 0, angle) * Vector2.up * 100f, Color.red);
            var hit = Physics2D.Raycast(Position, Quaternion.Euler(0, 0, angle) * Vector2.up, 100f, LayerMask.GetMask(new string[]{ "Obstacles" }));
            
            // if (hit.collider != null) Debug.Log($"{hit.collider} {hit.collider.TryGetComponent<Platform>(out _)}");

            Platform platform = null;
            var condition =
                hit.collider != null && 
                hit.collider.TryGetComponent<Platform>(out platform) && 
                !ReachableFromPlatforms.Contains(platform);
            
            if (condition)
                ReachableFromPlatforms.Add(platform);
        }

        // Debug.Log(ReachableFromPlatforms.Count);
    }

}
