using UnityEngine;
using UnityEngine.InputSystem;

public class PathToClick : MonoBehaviour
{

    [SerializeField] private ReimplementedMap _map;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private MapBuilder _mapBuilder;

    private void Start()
    {
        _mapBuilder.InitializeMap(_map);
        PathFinding.Initialize(_map);
    }

    private void Update()
    {
        // if (Mouse.current.clickCount.ReadValue() == 0)
        //     return;
        
        var mousePosition = Mouse.current.position.ReadValue();
        var worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
        var tileX = 0;
        var tileY = 0;
        _map.GetMapTileAtPoint(new Vector2(worldPosition.x, worldPosition.y), out tileX, out tileY);

        var path = PathFinding.PathFinder.FindPath(
            new Vector2Int(tileX, tileY),
            new Vector2Int((int) _playerTransform.position.x, (int) _playerTransform.position.y),
            1, 
            1,
            5
        );

        if (path == null || path.Count == 0)
            return;

        for (var i = 0; i < path.Count - 1; i++)
            Debug.DrawLine(new Vector3(path[i].x, path[i].y), new Vector3(path[i + 1].x, path[i + 1].y), Color.red);
    }

}
