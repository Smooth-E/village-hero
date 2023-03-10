using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMover : MonoBehaviour
{

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private CharacterGrounder _grounder;
    [SerializeField] private float _jumpForce = 30f;
    [SerializeField] private float _moveSpeed = 10f;

    private PlayerControls.PlayerActions _controls;

    private void Start()
    {
        Debug.Log("Hello!");
        _controls = new PlayerControls().Player;
        _controls.Enable();
        _controls.Jump.performed += Jump;
    }

    private void FixedUpdate()
    {
        var velocity = new Vector2(_controls.Walk.ReadValue<float>() * _moveSpeed, _rigidbody.velocity.y);
        _rigidbody.velocity = velocity;
    }

    private void OnDestroy() =>
        _controls.Jump.performed -= Jump;

    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump!");
        if (_grounder.Grounded)
            _rigidbody.AddForce(new Vector2(0, _jumpForce), ForceMode2D.Impulse);
    }

}
