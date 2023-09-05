using System.Collections;
using UnityEngine;

public class Shooter : MonoBehaviour
{

    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform _gunTip;
    [SerializeField] private float _shootingInterval;
    [SerializeField] private float _projectileVelocity;
    [SerializeField] private AbstractTargetFinder _targetFinder;
    [SerializeField] private Transform _shoulderTransform;

    private void Start() =>
        StartCoroutine(ShootingCoroutine());
    
    private IEnumerator ShootingCoroutine()
    {
        while (true)
        {
            yield return new WaitUntil(() => _targetFinder.ShouldShoot());

            var direction = (_targetFinder.GetTargetTransform().position - _gunTip.transform.position);
            var projectile = Instantiate(_projectile);
            projectile.transform.position = _gunTip.transform.position;
            projectile.transform.rotation = _shoulderTransform.transform.rotation;
            projectile.GetComponent<Rigidbody2D>().velocity = direction.normalized * (_projectileVelocity * 10);

            yield return new WaitForSeconds(_shootingInterval);
        }
    }

}
