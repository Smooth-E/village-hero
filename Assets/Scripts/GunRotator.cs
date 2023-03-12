using UnityEngine;

public class GunRotator : MonoBehaviour
{

    [SerializeField] private Vector2 _defaultPosition = Vector2.left;
    [SerializeField] private ITargetFinder _targetFinder;

    public float CurrentRotation => transform.eulerAngles.z;

    private void Update()
    {
        var target = _targetFinder.GetTargetTransform();

        if (target != null)
        {
            transform.LookAt(target, _defaultPosition);
            var eulerX = transform.rotation.eulerAngles.x * Mathf.Sign(transform.lossyScale.x);
            transform.rotation = Quaternion.Euler(0, 0, eulerX);
        }
    }

}
