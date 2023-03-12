using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemyPathRegulator : MonoBehaviour
{

    private const float PlayerHeight = 1;
    private const float TargetCheckInterval = 0.01f;
    
    private Coroutine _currentActionCoroutine = null;
    private PlatformArea _currentPlatformArea = null;

    private PlatformArea _tempHighlightPlatformArea = null;
    private List<PathFindingNode> _tempLastPath = null;

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

    private void OnDrawGizmos()
    {
        if (_tempHighlightPlatformArea == null || _tempLastPath == null || !Application.isPlaying)
            return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(_tempHighlightPlatformArea.transform.position, Vector3.one);
        
        Gizmos.color = Color.green;
        for (int i = 0; i < _tempLastPath.Count - 1; i++)
        {
            Gizmos.DrawLine(_tempLastPath[i].LinkedPlatformArea.transform.position, _tempLastPath[i + 1].LinkedPlatformArea.transform.position);
            Gizmos.DrawWireSphere(_tempLastPath[i + 1].LinkedPlatformArea.transform.position, 1.2f);
        }
    }

    private void OnGrounded(PlatformArea platformArea) =>
        _currentPlatformArea = platformArea;

    private IEnumerator TargetCheckCoroutine()
    {
        yield return new WaitUntil(() => _currentPlatformArea != null);
        Debug.Log("Started patrolling!");

        while (true)
        {
            GetToTargetIfNeeded();
            yield return new WaitForSeconds(TargetCheckInterval);
        }
    }

    private void GetToTargetIfNeeded()
    {
        var origin = new Vector2(_originTransform.position.x, _originTransform.position.y);
        var direction = (PlayerInfo.Position - origin).normalized;

        var distance = Vector2.Distance(origin, PlayerInfo.Position);
        var mask = LayerMask.GetMask(new string[]{ "Platform", "Player" });
        var hit = Physics2D.Raycast(_originTransform.position, direction, distance, mask);
        Debug.DrawRay(origin, direction * distance, Color.magenta);

        var newTargetStatus =  hit.collider != null && hit.collider.CompareTag("Player");
        var requiresAction = TargetAvailable != newTargetStatus && !newTargetStatus;
        TargetAvailable = newTargetStatus;

        if (requiresAction)
            SelectClosestPlatformAndMoveToIt();
    }

    private void SelectClosestPlatformAndMoveToIt()
    {
        Debug.Log("Selecting closest platform!");

        float closestDistance = -1;
        PlatformArea closestPlatformArea = null;

        foreach (var platform in PlayerInfo.ReachableFromPlatformAreas)
        {
            var currentDistance = Vector2.Distance(_originTransform.position, platform.transform.position);

            if (platform != _currentPlatformArea && (closestDistance == -1 || closestDistance > currentDistance))
            {
                closestPlatformArea = platform;
                closestDistance = currentDistance;
            }
        }

        StopCurrentAction();

        if (closestDistance == -1)
            _currentActionCoroutine = StartCoroutine(HangAroundCoroutine());
        else
            _currentActionCoroutine = StartCoroutine(MoveToPlatformCoroutine(closestPlatformArea));
    }

    private void StopCurrentAction()
    {
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
            Debug.Log($"Selected action: {action}");

            _tempHighlightPlatformArea = nextPlatform;

            while (behaviour.MoveNext())
                yield return behaviour.Current;
            
            yield return null;
        }
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
        Debug.Log("Jumping from edge!");

        var enumerator = MoveToThePointX(edge);
        while (enumerator.MoveNext())
            yield return enumerator.Current;

        var oldPlatform = _currentPlatformArea;
        _mover.Jump();
        yield return new WaitUntil(() => _currentPlatformArea != oldPlatform);

        yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
        _mover.HorizontalVelocity = 0;
    }

    private IEnumerator JumpAnywhere(PlatformArea destination)
    {
        Debug.Log("Jumping anywhere!");
        
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
        Debug.Log("Falling from edge!");

        var enumerator = MoveToThePointX(edge);
        while (enumerator.MoveNext())
            yield return enumerator.Current;

        var oldPlatform = _currentPlatformArea;
        yield return new WaitUntil(() => _currentPlatformArea != oldPlatform);

        _mover.HorizontalVelocity = 0;
    }

    private IEnumerator HangAroundCoroutine()
    {
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
