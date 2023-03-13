using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private int _sceneIndex = 1;

    public void StartPlaying() =>
        SceneManager.LoadScene(_sceneIndex);


    public void Quit() =>
        Application.Quit();

}
