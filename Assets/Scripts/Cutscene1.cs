using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene1 : MonoBehaviour
{

    private readonly int _platformParameter = Animator.StringToHash("Modifier");

    [SerializeField] private CharacterMover _mover;
    [SerializeField] private PlayerMover _playerMover;
    [SerializeField] private Shooter _playerShooter;
    [SerializeField] private Transform[] _points;
    [SerializeField] private Animator _mainPlatformAnimator;
    [SerializeField] private Animator[] _otherPlatforms;
    [SerializeField] private GameObject _boxOfText;

    private void Start() =>
        StartCoroutine(CutsceneCoroutine());

    private IEnumerator CutsceneCoroutine()
    {
        GameInput.GetPlayerActions().Disable();
        _playerMover.enabled = false;
        _playerShooter.enabled = false;
        _mainPlatformAnimator.SetFloat(_platformParameter, 1);

        _mover.HorizontalVelocity = 1;
        while (_playerMover.transform.position.x< _points[0].position.x)
            yield return null;

        Debug.Log("Jump~ " + PlayerInfo.Position);
        _mover.Jump();
        yield return new WaitUntil(() => _playerMover.transform.position.x >= _points[1].position.x);

        _mover.HorizontalVelocity = 0;
        _boxOfText.SetActive(true);
    }

    public void EndCutscene()
    {
        _boxOfText.SetActive(false);
        
        foreach (var animator in _otherPlatforms)
            animator.SetFloat(_platformParameter, 1);
        
        GameInput.GetPlayerActions().Enable();
        _playerMover.enabled = true;
        _playerShooter.enabled = true;
    }

}
