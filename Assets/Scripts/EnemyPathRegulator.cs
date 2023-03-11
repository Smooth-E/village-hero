using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine;

public class EnemyPathRegulator : MonoBehaviour
{

    private const float PlayerHeight = 1;
    private const float TargetCheckInterval = 0.01f;
    
    private Coroutine _currentActionCoroutine = null;
    private Platform _currentPlatform = null;

    [SerializeField] private Transform _originTransform;
    [SerializeField] private CharacterGrounder _grounder;
    [SerializeField] private CharacterMover _mover;

    public bool TargetAvailable { private set; get; } = true;

    private void Awake()
    {
        _grounder.OnGrounded += OnGrounded;
        StartCoroutine(TargetCheckCoroutine());
    }

    private void OnDestroy() =>
        _grounder.OnGrounded -= OnGrounded;

    private void OnGrounded(Platform platform) =>
        _currentPlatform = platform;

    private IEnumerator TargetCheckCoroutine()
    {
        yield return new WaitUntil(() => _currentPlatform != null);
        Debug.Log("Started patroling!");
        while (true)
        {
            GetToTargetIfNeeded();
            yield return new WaitForSeconds(TargetCheckInterval);
        }
    }

    private void GetToTargetIfNeeded()
    {
        var origin = new Vector2(_originTransform.position.x, _originTransform.position.y);
        var direction = (origin - PlayerInfo.Position).normalized;
        var distance = Vector2.Distance(origin, PlayerInfo.Position);
        var hit = Physics2D.Raycast(_originTransform.position, direction, distance);
        var newTargetStatus =  hit.collider != null && hit.collider.CompareTag("Player");
        var requiresAction = TargetAvailable != newTargetStatus && !newTargetStatus;
        TargetAvailable = newTargetStatus;

        if (requiresAction)
            SelectClosestPlatformAndMoveToIt();
    }

    private void SelectClosestPlatformAndMoveToIt()
    {
        float closestDistance = -1;
        Platform closestPlatform = null;

        foreach (var platform in PlayerInfo.ReachableFromPlatforms)
        {
            var currentDistance = Vector2.Distance(_originTransform.position, platform.transform.position);

            if (closestDistance == -1 || closestDistance > currentDistance)
            {
                closestPlatform = platform;
                closestDistance = currentDistance;
            }
        }

        StopCurrentAction();

        if (closestDistance == -1)
            _currentActionCoroutine = StartCoroutine(HangAroundCoroutine());
        else
            _currentActionCoroutine = StartCoroutine(MoveToPlatformCoroutine(closestPlatform));
    }

    private void StopCurrentAction()
    {
        if (_currentActionCoroutine != null)
        {
            StopCoroutine(_currentActionCoroutine);
            _currentActionCoroutine = null;
        }
    }

    private IEnumerator MoveToPlatformCoroutine(Platform destinationPlatform)
    {
        Debug.Log("Starting to search for a path");
        yield return null;
        var path = PathFinder.FindPath(_currentPlatform, destinationPlatform);
        yield return null;
        Debug.Log("Pathfinding finished!");
        yield return null;
        // List<PathFindingNode> path = null;

        if (path == null || path.Count == 0)
        {
            _currentActionCoroutine = StartCoroutine(HangAroundCoroutine());
            yield break;
        }

        for (var i = 0; i < path.Count - 1; i++)
        {
            var currentPlatform = path[i].LinkedPlatform;
            var nextPlatform = path[i + 1].LinkedPlatform;
            var action = currentPlatform.GetActionForDestination(nextPlatform);

            IEnumerator behaviour = null;
            switch (action)
            {
                case PathFindingAction.FallFromEdge:
                    behaviour = FallFromEdge();
                    break;
                case PathFindingAction.JumpAnywhere:
                    behaviour = JumpAnywhere();
                    break;
                case PathFindingAction.JumpFromEdge:
                    behaviour = JumpFromEdge();
                    break;
            }
            Debug.Log($"Selected action: {action}");

            // while (behaviour.MoveNext())
            //    yield return null;
            
            yield return null;
        }
    }

    private IEnumerator JumpFromEdge()
    {
        Debug.Log("Jumping from edge!");
        yield break;
    }

    private IEnumerator JumpAnywhere()
    {
        Debug.Log("Jumping anywhere!");
        yield break;
    }

    private IEnumerator FallFromEdge()
    {
        Debug.Log("Falling from edge!");
        yield break;
    }

    private IEnumerator HangAroundCoroutine()
    {
        var direction = Random.Range(0, 2) == 0 ? 0.5f : -0.5f;
        while (true)
        {
            _mover.HorizontalVelocity = direction;

            if (direction > 0)
                yield return new WaitUntil(() => transform.position.x > _currentPlatform.RightEdge);
            else
                yield return new WaitUntil(() => transform.position.x < _currentPlatform.LeftEdge);

            direction = -direction;
        }
    }

}
