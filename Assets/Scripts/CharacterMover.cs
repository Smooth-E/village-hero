using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMover : MonoBehaviour
{

    private readonly float _jumpForce = 30f;
    private readonly float _moveSpeed = 10f;

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private CharacterGrounder _grounder;
    
    private float _horizontalVelocity = 0;
    public float HorizontalVelocity 
    {
        set => _horizontalVelocity = Mathf.Max(-1, Mathf.Min(value, 1));
        get => _horizontalVelocity;
    }

    private void Update() =>
        _rigidbody.velocity = new Vector2(_horizontalVelocity * _moveSpeed, _rigidbody.velocity.y);

    public void Jump(InputAction.CallbackContext context)
    {
        if (_grounder.IsGrounded)
            _rigidbody.AddForce(new Vector2(0, _jumpForce), ForceMode2D.Impulse);
    }

}
