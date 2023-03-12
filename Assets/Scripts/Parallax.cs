using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Parallax : MonoBehaviour
{

    private Camera _camera;
    private Vector2 _startPosition;
    private Vector2 _size;

    [SerializeField] private float _modifier;
    [SerializeField] private bool _tileUpwards = true;

    private void Start()
    {
        _startPosition = new Vector2(transform.position.x, transform.position.y);
        _size = GetComponent<SpriteRenderer>().bounds.size;
        _camera = Camera.main;
    }

    private void Update()
    {
        var distance = (Vector2) _camera.transform.position * _modifier;
        transform.position = _startPosition + distance;

        var maximumDistance = _camera.transform.position * (1 - _modifier);
        
        var clampedX = _startPosition.x;
        if (maximumDistance.x > _startPosition.x + _size.x)
            clampedX += _size.x;
        else if (maximumDistance.x < _startPosition.x - _size.x)
            clampedX -= _size.x;

        var clampedY = _startPosition.y;
        if (_tileUpwards)
        {
            if (maximumDistance.y > _startPosition.y + _size.y)
                clampedY += _size.y;
            else if (maximumDistance.y < _startPosition.y - _size.y)
                clampedY -= _size.y;
        }

        _startPosition = new Vector2(clampedX, clampedY);
    }

}
