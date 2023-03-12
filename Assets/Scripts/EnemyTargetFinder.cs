using UnityEngine;

public class EnemyTargetFinder : ITargetFinder
{

    private Transform _lastTarget;
    private bool _targetAvailable;

    private void Update()
    {
        var origin = new Vector2(transform.position.x, transform.position.y);
        var direction = (PlayerInfo.Position - origin).normalized;

        var distance = Vector2.Distance(origin, PlayerInfo.Position);
        var mask = LayerMask.GetMask(new string[]{ "Obstacle", "Player" });
        var hit = Physics2D.Raycast(transform.position, direction, distance, mask);
        Debug.DrawRay(origin, direction * distance, Color.magenta);

        _targetAvailable = hit.collider != null && hit.collider.CompareTag("Player");

        _lastTarget = _targetAvailable ? hit.transform : null;
    }

    public override Transform GetTargetTransform() =>
        _lastTarget;

    public override bool ShouldShoot() =>
        _targetAvailable;

}
