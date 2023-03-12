using UnityEngine;

public class Projectile : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D other)
    {
        var tag = other.gameObject.tag;
        if (tag == "Enemy" || tag == "Obstacle" || tag == "Player")
            Destroy(gameObject);
    }

}
