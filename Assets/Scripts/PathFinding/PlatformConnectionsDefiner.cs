using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformConnectionsDefiner : MonoBehaviour
{

    private readonly float _approximateJumpHeight = 3.17f;
    
    [Tooltip("A prefab of a probe object that is spawned to imitate character's behaviour and get the list of possible destination platforms one can get to.")]
    [SerializeField] private GameObject _probePrefab;
    
    private PlatformArea[] _platformAreas;

    private void Start() =>
        UpdatePlatformList();

    private void UpdatePlatformList()
    {
        _platformAreas = FindObjectsOfType<PlatformArea>();
        DefineConnections();
    }

    private void DefineConnections()
    {
        var parameters = new CreateSceneParameters(LocalPhysicsMode.Physics2D);
        var simulationScene = SceneManager.CreateScene("Path Builder Physics Scene", parameters);

        foreach (var platformArea in _platformAreas)
        {
            var platformGameObject = platformArea.gameObject;
            var platformTransform = platformGameObject.transform;
            var position = platformTransform.position;
            var rotation = platformTransform.rotation;
            var platformAreaInstance = Instantiate(platformArea, position, rotation);
            
            SceneManager.MoveGameObjectToScene(platformAreaInstance.gameObject, simulationScene);
            DefineConnectionsForPlatform(platformAreaInstance);
        }
            
    }

    private void DefineConnectionsForPlatform(PlatformArea platformArea)
    {
        platformArea.PossibleDestinations = new List<PathFindingDestination>();

        foreach (var result in JumpAnywhere(platformArea))
        {
            var destination = new PathFindingDestination(result, PathFindingAction.JumpAnywhereUnder);
            platformArea.PossibleDestinations.Add(destination);
        }
    }

    private List<PlatformArea> JumpAnywhere(PlatformArea platformArea)
    {
        Vector2 pointA = platformArea.RightEdgePosition;
        Vector2 pointB = platformArea.LeftEdgePosition + Vector3.up * _approximateJumpHeight;
        
        var filter = new ContactFilter2D();
        filter.layerMask = LayerMask.GetMask("Platform Area");
        
        List<Collider2D> results = new();
        Physics2D.OverlapArea(pointA, pointB, filter, results);

        List<PlatformArea> returnable = new();
        foreach (var platformAreaCollider in results)
            returnable.Add(platformAreaCollider.GetComponent<PlatformArea>());
        
        return returnable;
    }
    
}
