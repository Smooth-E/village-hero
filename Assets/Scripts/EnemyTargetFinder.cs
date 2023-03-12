using UnityEngine;

public class EnemyTargetFinder : ITargetFinder
{

    public override Transform GetTargetTransform()
    {
        var origin = new Vector2(transform.position.x, transform.position.y);
        var direction = (PlayerInfo.Position - origin).normalized;

        var distance = Vector2.Distance(origin, PlayerInfo.Position);
        var mask = LayerMask.GetMask(new string[]{ "Obstacle", "Player" });
        var hit = Physics2D.Raycast(transform.position, direction, distance, mask);
        Debug.DrawRay(origin, direction * distance, Color.magenta);

        var targetAvailable = hit.collider != null && hit.collider.CompareTag("Player");

        return targetAvailable ? hit.transform : null;
    }

}
