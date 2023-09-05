using System;
using UnityEngine;

public class CharacterGrounder : MonoBehaviour
{
    
    [SerializeField] private float _circleRadius = 0.2f;

    public bool IsGrounded { private set; get; }

    public event Action<PlatformArea> OnGrounded;

    private void FixedUpdate()
    {
        var layerMask = LayerMask.GetMask(new string[]{ "Platform" });
        var collider = Physics2D.OverlapCircle(transform.position, _circleRadius, layerMask);
        
        var nowGrounded = collider != null;

        if (nowGrounded != IsGrounded && nowGrounded)
            OnGrounded?.Invoke(collider.transform.parent.GetComponentInChildren<PlatformArea>());

        IsGrounded = nowGrounded;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _circleRadius);
    }

}
