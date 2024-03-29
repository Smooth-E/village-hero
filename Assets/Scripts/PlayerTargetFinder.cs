using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTargetFinder : AbstractTargetFinder
{

    private Camera _camera;

    [SerializeField] private Transform _cursorTransform;

    private void Awake() =>
        _camera = Camera.main;

    private void Update()
    {
        var mousePosition = Mouse.current.position.ReadValue();
        _cursorTransform.position = _camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10));
    }

    public override Transform GetTargetTransform() =>
        _cursorTransform;

    public override bool ShouldShoot() =>
        Mouse.current.leftButton.isPressed;

}
