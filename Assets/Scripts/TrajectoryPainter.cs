using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPainter : MonoBehaviour
{

    [SerializeField] private float _pointInterval = 0.01f;
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 0;
        StartCoroutine(TrajectoryCoroutine());
    }

    private IEnumerator TrajectoryCoroutine()
    {
        while (true)
        {
            var positionCount = _lineRenderer.positionCount;
            _lineRenderer.positionCount = positionCount + 1;
            _lineRenderer.SetPosition(positionCount, transform.position);
            yield return new WaitForSeconds(_pointInterval);
        }
    }

}
