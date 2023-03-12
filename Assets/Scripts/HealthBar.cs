using UnityEngine;

public class HealthBar : MonoBehaviour
{

    private readonly int _parameter = Animator.StringToHash("Present");

    [SerializeField] private Animator[] _hearts;
    [SerializeField] private CharacterHealth _playerHealth;

    private void Start()
    {
        _playerHealth.OnHealthDecreased += UpdateHealthBar;
        _playerHealth.OnHealthIncreased += UpdateHealthBar;
    }

    private void OnDestroy()
    {
        _playerHealth.OnHealthDecreased -= UpdateHealthBar;
        _playerHealth.OnHealthIncreased -= UpdateHealthBar;
    }

    private void UpdateHealthBar(int newHealth)
    {
        for (int index = 1; index <= _playerHealth.MaxHealth; index++)
            _hearts[index].SetBool(_parameter, index <= newHealth);
    }

}
