using UnityEngine;

public class PlayerMover : MonoBehaviour
{

    [SerializeField] private CharacterMover _mover;
    private PlayerControls.PlayerActions _actions;

    private void Start()
    {
        _actions = GameInput.GetPlayerActions();
        _actions.Jump.performed += _mover.Jump;
    }

    private void FixedUpdate() =>
        _mover.HorizontalVelocity = _actions.Walk.ReadValue<float>();

}
