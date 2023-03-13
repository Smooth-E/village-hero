using UnityEngine;

public class CharacterKiller : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Death Zone"))
        {
            Debug.Log("Dead!");
            GetComponent<CharacterHealth>().Kill();
        }

    }
}
