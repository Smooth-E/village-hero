using System;
using UnityEngine;

public class CharacterGrounder : MonoBehaviour
{

    private bool _isGrounded = false;

    [SerializeField] private Rigidbody2D _rigidbody;

    public bool IsGrounded => _isGrounded;

    public event Action<Platform> OnGrounded;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            var platform = other.gameObject.GetComponent<Platform>();
            OnGrounded?.Invoke(platform);
            _isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground") || _rigidbody.velocity.y == 0)
            _isGrounded = false;
    }

}
