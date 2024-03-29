using System;
using PathFinding;
using UnityEngine;

public class CharacterGrounder : MonoBehaviour
{
    
    private readonly float _circleRadius = 0.2f;

    public bool IsGrounded { private set; get; }

    public event Action<Platform> OnGrounded;

    private void FixedUpdate()
    {
        var layerMask = LayerMask.GetMask("Platform");
        var collider = Physics2D.OverlapCircle(transform.position, _circleRadius, layerMask);
        
        var nowGrounded = collider != null;

        if (nowGrounded != IsGrounded && nowGrounded)
            OnGrounded?.Invoke(collider.transform.parent.GetComponentInChildren<Platform>());

        IsGrounded = nowGrounded;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _circleRadius);
    }

}
