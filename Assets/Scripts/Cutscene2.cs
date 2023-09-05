using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Cutscene2 : MonoBehaviour
{

    [SerializeField] private FancyText _fancyText;
    [SerializeField] private GameObject _boxOfText;
    [SerializeField] private EnemySpawner _spawner;

    private void Start() =>
        StartCoroutine(CutsceneCoroutine());

    private IEnumerator CutsceneCoroutine()
    {
        yield return new WaitUntil(() => 
        {
            Debug.Log(_spawner.EnemiesAlive + " " + _spawner.EnemiesLeftToSpawn);
            return _spawner.EnemiesAlive == 0 && _spawner.EnemiesLeftToSpawn == 0;
        });

        _boxOfText.SetActive(true);
        _fancyText.ChangeText( 
            "Наш герой победил всех захватчиков из Культа Милых Собакенов." +
            "Теперь ему предстоит вернуться в деревню и рассказать все жителям!"
        );
        GameInput.GetPlayerActions().Disable();
    }

    public void EndCutscene()
    {
        SceneManager.LoadScene(2);
    }

}
