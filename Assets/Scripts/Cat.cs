using UnityEngine;

public class Cat : MonoBehaviour
{

    public static bool WasHurt { private set; get; } = false;

    private void Awake() => 
        WasHurt = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player's Bullet"))
        {
            WasHurt = true;
            Destroy(gameObject);
        }
    }

}
