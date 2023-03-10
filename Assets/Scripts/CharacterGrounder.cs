using System;
using UnityEngine;

public class CharacterGrounder : MonoBehaviour
{

    private bool _grounded = false;
    public bool Grounded => _grounded;

    public event Action<bool> StateChanged;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            StateChanged?.Invoke(true);
            _grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            StateChanged?.Invoke(false);
            _grounded = false;
        }
    }

}
