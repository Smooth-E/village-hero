using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>A cutscene behaviour that plays when the player dies.</summary>
public class DeathCutscene : MonoBehaviour
{

    [SerializeField] private CharacterHealth _playerHealth;
    [SerializeField] private GameObject _textBox;
    [SerializeField] private FancyText _text;

    private void Start() =>
        _playerHealth.OnDeath += OnDeath;

    private void OnDeath()
    {
        Debug.Log("On death!");
        _textBox.SetActive(true);
        _text.ChangeText( 
            "Герой не справился с натиском Культистов Милых Собакенов." +
            "Надеюсь, в другой раз ему повезет больше..."
        );
    }

    public void Something() => SceneManager.LoadScene(0);

}
