using System.Collections;
using UnityEngine;

public class PhysicsTester : MonoBehaviour
{

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.velocity = new Vector2();
        _rigidbody.AddForce(Vector2.up, ForceMode2D.Impulse);
    }

    private IEnumerator GetMaxPosition()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => _rigidbody.velocity.y < 0);
        Debug.Log(transform.position);
        Debug.Log(1 / Physics2D.gravity.y / 2f);
    }

}
