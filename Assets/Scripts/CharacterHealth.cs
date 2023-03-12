using System;
using UnityEngine;

public class CharacterHealth : MonoBehaviour
{


    private int _health;

    [SerializeField] private int _maxHealth = 5;
    [SerializeField] private bool _canHeal = false;
    [SerializeField] private string _healTag = "Heal";
    [SerializeField] private string _badProjectile;

    public int MaxHealth => _maxHealth;

    public event Action<int> OnHealthDecreased;
    public event Action<int> OnHealthIncreased;
    public event Action OnDeath;

    private void Awake()
    {
        _health = _maxHealth;
        OnHealthIncreased?.Invoke(_health);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(_badProjectile))
        {
            _health--;
            OnHealthDecreased?.Invoke(_health);

            if (_health <= 0)
                OnDeath?.Invoke();
        }
        else if (_canHeal && other.gameObject.CompareTag("Heal"))
        {
            _health++;
            _health = Mathf.Min(_maxHealth, _health);
            OnHealthIncreased?.Invoke(_health);
        }
    }

}
