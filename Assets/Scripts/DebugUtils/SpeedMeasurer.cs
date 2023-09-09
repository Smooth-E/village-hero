using UnityEngine;

namespace DebugUtils
{
    public class SpeedMeasurer : MonoBehaviour
    {

        private void Start()
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * 30f, ForceMode2D.Impulse);
        }

        private float _maxVelocity = 0;

        private void Update()
        {
            _maxVelocity = Mathf.Max(_maxVelocity, GetComponent<Rigidbody2D>().velocity.y);
            Debug.Log(_maxVelocity);
        }

    }
}
