using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimator : MonoBehaviour
{

    private readonly int _parameterJump = Animator.StringToHash("Jump");
    private readonly int _parameterRunning = Animator.StringToHash("Running");
    private readonly int _parameterGrounded = Animator.StringToHash("Grounded");
    private readonly int _parameterHorizontalVelocity = Animator.StringToHash("Horizontal Velocity");

    private Animator _animator;
    
    [SerializeField] private CharacterMover _mover;
    [SerializeField] private CharacterGrounder _grounder;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _mover.OnJump += OnJump;
    }

    private void Update()
    {
        _animator.SetBool(_parameterRunning, _mover.HorizontalVelocity != 0);
        _animator.SetBool(_parameterGrounded, _grounder.IsGrounded);
        _animator.SetFloat(_parameterHorizontalVelocity, _mover.HorizontalVelocity);
    
        if (_mover.HorizontalVelocity == 0)
            return;

        var currentScale = transform.localScale;
        currentScale.x = Mathf.Abs(currentScale.x) * -Mathf.Sign(_mover.HorizontalVelocity);
        transform.localScale = currentScale;
    }

    private void OnDestroy() =>
        _mover.OnJump -= OnJump;

    private void OnJump() =>
        _animator.SetTrigger(_parameterJump);

}
