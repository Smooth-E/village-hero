using System;
using ScriptableObjects;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{

    private float _horizontalVelocity;

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private CharacterGrounder _grounder;
    
    public float HorizontalVelocity 
    {
        set => _horizontalVelocity = Mathf.Max(-1, Mathf.Min(value, 1));
        get => _horizontalVelocity;
    }

    public event Action OnJump;

    private void Awake()
    {
        _rigidbody.mass = EntityParameters.Mass;
        _rigidbody.gravityScale = EntityParameters.GravityScale;
    }

    private void Update()
    {
        var xVelocity = _horizontalVelocity * EntityParameters.MovementVelocity;
        _rigidbody.velocity = new Vector2(xVelocity, _rigidbody.velocity.y);
    }

    public void Jump()
    {
        if (_grounder.IsGrounded)
        {
            OnJump?.Invoke();
            _rigidbody.AddForce(new Vector2(0, EntityParameters.JumpForce), ForceMode2D.Impulse);
        }
    }

}
