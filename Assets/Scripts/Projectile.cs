using UnityEngine;

public class Projectile : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        var tag = other.gameObject.tag;
        if (tag == "Enemy" || tag == "Obstacle" || tag == "Player" || tag == "Enemy")
            Destroy(gameObject);
    }

}
