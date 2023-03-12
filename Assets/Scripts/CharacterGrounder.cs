using System;
using UnityEngine;

public class CharacterGrounder : MonoBehaviour
{

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _circleRadius = 0.2f;

    public bool IsGrounded { private set; get; }

    public event Action<Platform> OnGrounded;

    private void FixedUpdate()
    {
        var layerMask = LayerMask.GetMask(new string[]{ "Platform" });
        var collider = Physics2D.OverlapCircle(transform.position, _circleRadius, layerMask);

        Debug.Log($"Collider: {collider == null}");
        var nowGrounded = collider != null;

        if (nowGrounded != IsGrounded && nowGrounded)
            OnGrounded?.Invoke(collider.GetComponentInChildren<Platform>());

        IsGrounded = nowGrounded;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _circleRadius);
    }

}
