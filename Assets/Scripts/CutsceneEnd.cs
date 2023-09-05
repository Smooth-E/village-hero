using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneEnd : MonoBehaviour
{

    [SerializeField] private CharacterMover _mover;
    [SerializeField] private GameObject _boxDead;
    [SerializeField] private GameObject _boxAlive;
    [SerializeField] private GameObject _sceneDead;
    [SerializeField] private GameObject _sceneAlive;
    [SerializeField] private Transform[] _points;

    private void Start()
    {
        _sceneDead.SetActive(Cat.WasHurt);
        _sceneAlive.SetActive(!Cat.WasHurt);
        StartCoroutine(TheCoroutine());
    }

    private IEnumerator TheCoroutine()
    {
        GameInput.GetPlayerActions().Disable();

        _mover.HorizontalVelocity = 1;
        yield return new WaitUntil(() => _mover.transform.position.x >= _points[0].position.x);
        _mover.HorizontalVelocity = 0;

        _boxDead.SetActive(Cat.WasHurt);
        _boxAlive.SetActive(!Cat.WasHurt);
    }

    public void End()
    {
        SceneManager.LoadScene(0);
    }

}
