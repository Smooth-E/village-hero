using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the movement of an enemy based on whether it has a target and whether it is reachable
/// </summary>
public class EnemyMovementGovernor : MonoBehaviour
{

    /// <summary>
    /// An action type tells the reason why the enemy is currently moving
    /// </summary>
    private enum ActionType
    {
        /// <summary>The purpose of enemy's movement is currently unknown or not set, yet</summary>
        None,
        /// <summary>The enemy have successfully found a target and is now moving to it</summary>
        Traveling,
        /// <summary>The target is unreachable and the enemy is roaming around</summary>
        Roaming
    }
    
    private const float TargetCheckInterval = 0.01f;
    
    private Coroutine _currentActionCoroutine;
    private PlatformArea _currentPlatformArea;

    private PlatformArea _tempHighlightPlatformArea;
    private List<PathFindingNode> _tempLastPath;
    private ActionType _currentActionType = ActionType.None; 

    [SerializeField] private Transform _originTransform;
    [SerializeField] private CharacterGrounder _grounder;
    [SerializeField] private CharacterMover _mover;
    [SerializeField] private EnemyTargetFinder _targetFinder;

    public bool TargetAvailable { private set; get; } = true;

    private void Awake()
    {
        _grounder.OnGrounded += OnGrounded;
        StartCoroutine(TargetCheckCoroutine());
    }

    private void OnDestroy() =>
        _grounder.OnGrounded -= OnGrounded;

    private void OnDrawGizmos()
    {
        if (_tempHighlightPlatformArea == null || _tempLastPath == null || !Application.isPlaying)
            return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(_tempHighlightPlatformArea.transform.position, Vector3.one);
        
        Gizmos.color = Color.green;
        for (int i = 0; i < _tempLastPath.Count - 1; i++)
        {
            var startingPoint = _tempLastPath[i].LinkedPlatformArea.transform.position;
            var endPoint = _tempLastPath[i + 1].LinkedPlatformArea.transform.position;
            Gizmos.DrawLine(startingPoint, endPoint);
            Gizmos.DrawWireSphere(_tempLastPath[i + 1].LinkedPlatformArea.transform.position, 1.2f);
        }

        Gizmos.color = TargetAvailable ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position + Vector3.up * 2, 0.5f);
    }

    private void OnGrounded(PlatformArea platformArea) =>
        _currentPlatformArea = platformArea;

    private IEnumerator TargetCheckCoroutine()
    {
        yield return new WaitUntil(() => _currentPlatformArea != null);

        while (true)
        {
            GetToTargetIfNeeded();
            yield return new WaitForSeconds(TargetCheckInterval);
        }
    }

    private void GetToTargetIfNeeded()
    {
        var newTargetStatus = _targetFinder.GetTargetTransform() != null;
        var requiresAction = !newTargetStatus && _currentActionType != ActionType.Traveling;
        TargetAvailable = newTargetStatus;

        if (requiresAction)
            SelectClosestPlatformAndMoveToIt();
    }

    private void SelectClosestPlatformAndMoveToIt()
    {
        float closestDistance = -1;
        PlatformArea closestPlatformArea = null;

        foreach (var platform in PlayerInfo.ReachableFromPlatformAreas)
        {
            var currentDistance = 
                Vector2.Distance(_originTransform.position, platform.transform.position);

            if (platform != _currentPlatformArea && (closestDistance == -1 || closestDistance > currentDistance))
            {
                closestPlatformArea = platform;
                closestDistance = currentDistance;
            }
        }


        if (closestDistance == -1 && _currentActionType != ActionType.Roaming)
            _currentActionCoroutine = StartCoroutine(HangAroundCoroutine());
        else if (closestDistance != -1)
            _currentActionCoroutine = StartCoroutine(MoveToPlatformCoroutine(closestPlatformArea));
    }

    private void StopCurrentAction()
    {
        _currentActionType = ActionType.None;
        if (_currentActionCoroutine != null)
        {
            StopCoroutine(_currentActionCoroutine);
            _currentActionCoroutine = null;
        }
    }

    private IEnumerator MoveToPlatformCoroutine(PlatformArea destinationPlatformArea)
    {

        var path = PathFinder.FindPath(_currentPlatformArea, destinationPlatformArea);
        _tempLastPath = path;

        if (path == null || path.Count == 0)
        {
            _currentActionCoroutine = StartCoroutine(HangAroundCoroutine());
            yield break;
        }

        StopCurrentAction();
        _currentActionType = ActionType.Traveling;

        for (var i = 0; i < path.Count - 1; i++)
        {
            var currentPlatform = path[i].LinkedPlatformArea;
            var nextPlatform = path[i + 1].LinkedPlatformArea;
            var action = currentPlatform.GetActionForDestination(nextPlatform);

            IEnumerator behaviour = null;
            switch (action)
            {
                case PathFindingAction.FallFromRightEdge:
                    behaviour = FallFromEdge(_currentPlatformArea.RightEdge);
                    break;
                case PathFindingAction.FallFromLeftEdge:
                    behaviour = FallFromEdge(_currentPlatformArea.LeftEdge);
                    break;
                case PathFindingAction.FallFromAnyEdge:
                    behaviour = FallFromEdge(GetClosestEdge());
                    break;
                case PathFindingAction.JumpFromRightEdge:
                    behaviour = JumpFromEdge(_currentPlatformArea.RightEdge);
                    break;
                case PathFindingAction.JumpFromLeftEdge:
                    behaviour = JumpFromEdge(_currentPlatformArea.LeftEdge);
                    break;
                case PathFindingAction.JumpFromAnyEdge:
                    behaviour = JumpFromEdge(GetClosestEdge());
                    break;
                case PathFindingAction.JumpAnywhereUnder:
                    behaviour = JumpAnywhere(nextPlatform);
                    break;
            }
            // Debug.Log($"Selected action: {action}");

            _tempHighlightPlatformArea = nextPlatform;

            while (behaviour.MoveNext())
                yield return behaviour.Current;
            
            yield return null;
        }

        _currentActionType = ActionType.None;
    }

    private float GetClosestEdge()
    {
        var currentX = transform.position.x;
        if (Mathf.Abs(currentX - _currentPlatformArea.LeftEdge) > Mathf.Abs(currentX - _currentPlatformArea.RightEdge))
            return _currentPlatformArea.RightEdge;
        else
            return _currentPlatformArea.LeftEdge;
    }

    private IEnumerator MoveToThePointX(float destinationX)
    {
        var direction = Mathf.Sign(destinationX - transform.position.x);
        _mover.HorizontalVelocity = direction;
        yield return new WaitUntil(() => (transform.position.x - destinationX) * direction > 0);
    }

    private IEnumerator JumpFromEdge(float edge)
    {
        // Debug.Log("Jumping from edge!");

        var enumerator = MoveToThePointX(edge);
        while (enumerator.MoveNext())
            yield return enumerator.Current;

        var oldPlatform = _currentPlatformArea;
        _mover.Jump();
        yield return new WaitUntil(() => _currentPlatformArea != oldPlatform);

        _mover.HorizontalVelocity = 0;
    }

    private IEnumerator JumpAnywhere(PlatformArea destination)
    {
        // Debug.Log("Jumping anywhere!");
        
        var direction = transform.position.x > destination.transform.position.x ? -1 : 1;
        _mover.HorizontalVelocity = direction;

        yield return new WaitUntil(() => 
            transform.position.x > destination.LeftEdge && transform.position.x < destination.RightEdge
        );

        _mover.Jump();
        _mover.HorizontalVelocity = direction * 0.5f;
        var oldPlatform = _currentPlatformArea;
        yield return new WaitUntil(() => _currentPlatformArea != oldPlatform);

        yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
        _mover.HorizontalVelocity = 0;
    }

    private IEnumerator FallFromEdge(float edge)
    {
        // Debug.Log("Falling from edge!");

        var enumerator = MoveToThePointX(edge);
        while (enumerator.MoveNext())
            yield return enumerator.Current;

        var oldPlatform = _currentPlatformArea;
        yield return new WaitUntil(() => _currentPlatformArea != oldPlatform);

        _mover.HorizontalVelocity = 0;
    }

    private IEnumerator HangAroundCoroutine()
    {
        StopCurrentAction();
        _currentActionType = ActionType.Roaming;
        _tempLastPath = null;

        var direction = Random.Range(0, 2) == 0 ? 0.5f : -0.5f;
        while (true)
        {
            _mover.HorizontalVelocity = direction;

            if (direction > 0)
                yield return new WaitUntil(() => transform.position.x > _currentPlatformArea.RightEdge);
            else
                yield return new WaitUntil(() => transform.position.x < _currentPlatformArea.LeftEdge);

            direction = -direction;
        }
    }

}
