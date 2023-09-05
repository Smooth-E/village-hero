using System;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{

    private readonly float _jumpForce = 30f;
    private readonly float _moveSpeed = 10f;

    private float _horizontalVelocity;

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private CharacterGrounder _grounder;
    
    public float HorizontalVelocity 
    {
        set => _horizontalVelocity = Mathf.Max(-1, Mathf.Min(value, 1));
        get => _horizontalVelocity;
    }

    public event Action OnJump;

    private void Update() =>
        _rigidbody.velocity = new Vector2(_horizontalVelocity * _moveSpeed, _rigidbody.velocity.y);

    public void Jump()
    {
        if (_grounder.IsGrounded)
        {
            OnJump?.Invoke();
            _rigidbody.AddForce(new Vector2(0, _jumpForce), ForceMode2D.Impulse);
        }
    }

}
