using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemyPathRegulator : MonoBehaviour
{

    private const float PlayerHeight = 1;
    private const float TargetCheckInterval = 0.01f;
    
    private Coroutine _currentActionCoroutine = null;
    private Platform _currentPlatform = null;

    private Platform _tempHighlightPlatform = null;
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
        if (_tempHighlightPlatform == null || _tempLastPath == null || !Application.isPlaying)
            return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(_tempHighlightPlatform.transform.position, Vector3.one);
        
        Gizmos.color = Color.green;
        for (int i = 0; i < _tempLastPath.Count - 1; i++)
        {
            Gizmos.DrawLine(_tempLastPath[i].LinkedPlatform.transform.position, _tempLastPath[i + 1].LinkedPlatform.transform.position);
            Gizmos.DrawWireSphere(_tempLastPath[i + 1].LinkedPlatform.transform.position, 1.2f);
        }
    }

    private void OnGrounded(Platform platform) =>
        _currentPlatform = platform;

    private IEnumerator TargetCheckCoroutine()
    {
        yield return new WaitUntil(() => _currentPlatform != null);
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
        Platform closestPlatform = null;

        foreach (var platform in PlayerInfo.ReachableFromPlatforms)
        {
            var currentDistance = Vector2.Distance(_originTransform.position, platform.transform.position);

            if (platform != _currentPlatform && (closestDistance == -1 || closestDistance > currentDistance))
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
        var path = PathFinder.FindPath(_currentPlatform, destinationPlatform);
        _tempLastPath = path;

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
                case PathFindingAction.FallFromRightEdge:
                    behaviour = FallFromEdge(_currentPlatform.RightEdge);
                    break;
                case PathFindingAction.FallFromLeftEdge:
                    behaviour = FallFromEdge(_currentPlatform.LeftEdge);
                    break;
                case PathFindingAction.FallFromAnyEdge:
                    behaviour = FallFromEdge(GetClosestEdge());
                    break;
                case PathFindingAction.JumpFromRightEdge:
                    behaviour = JumpFromEdge(_currentPlatform.RightEdge);
                    break;
                case PathFindingAction.JumpFromLeftEdge:
                    behaviour = JumpFromEdge(_currentPlatform.LeftEdge);
                    break;
                case PathFindingAction.JumpFromAnyEdge:
                    behaviour = JumpFromEdge(GetClosestEdge());
                    break;
                case PathFindingAction.JumpAnywhereUnder:
                    behaviour = JumpAnywhere(nextPlatform);
                    break;
            }
            Debug.Log($"Selected action: {action}");

            _tempHighlightPlatform = nextPlatform;

            while (behaviour.MoveNext())
                yield return behaviour.Current;
            
            yield return null;
        }
    }

    private float GetClosestEdge()
    {
        var currentX = transform.position.x;
        if (Mathf.Abs(currentX - _currentPlatform.LeftEdge) > Mathf.Abs(currentX - _currentPlatform.RightEdge))
            return _currentPlatform.RightEdge;
        else
            return _currentPlatform.LeftEdge;
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

        var oldPlatform = _currentPlatform;
        _mover.Jump();
        yield return new WaitUntil(() => _currentPlatform != oldPlatform);

        yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
        _mover.HorizontalVelocity = 0;
    }

    private IEnumerator JumpAnywhere(Platform destination)
    {
        Debug.Log("Jumping anywhere!");
        
        var direction = transform.position.x > destination.transform.position.x ? -1 : 1;
        _mover.HorizontalVelocity = direction;

        yield return new WaitUntil(() => 
            transform.position.x > destination.LeftEdge && transform.position.x < destination.RightEdge
        );

        _mover.Jump();
        _mover.HorizontalVelocity = direction * 0.5f;
        var oldPlatform = _currentPlatform;
        yield return new WaitUntil(() => _currentPlatform != oldPlatform);

        yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
        _mover.HorizontalVelocity = 0;
    }

    private IEnumerator FallFromEdge(float edge)
    {
        Debug.Log("Falling from edge!");

        var enumerator = MoveToThePointX(edge);
        while (enumerator.MoveNext())
            yield return enumerator.Current;

        var oldPlatform = _currentPlatform;
        yield return new WaitUntil(() => _currentPlatform != oldPlatform);

        _mover.HorizontalVelocity = 0;
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
