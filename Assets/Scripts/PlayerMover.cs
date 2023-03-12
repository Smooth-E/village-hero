using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{

    [SerializeField] private CharacterMover _mover;
    private PlayerControls.PlayerActions _actions;

    private void Start()
    {
        _actions = GameInput.GetPlayerActions();
        _actions.Jump.performed += OnJumpEvent;
    }

    private void FixedUpdate() =>
        _mover.HorizontalVelocity = _actions.Walk.ReadValue<float>();
    
    private void OnDestroy() =>
        _actions.Jump.performed -= OnJumpEvent;

    private void OnJumpEvent(InputAction.CallbackContext context) =>
        _mover.Jump();

}
