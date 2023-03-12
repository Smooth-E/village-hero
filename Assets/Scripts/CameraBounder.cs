using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraBounder : MonoBehaviour
{

    private Camera _camera;

    [SerializeField] private float _rightBound = 30f;
    [SerializeField] private float _leftBound = -30f;
    [SerializeField] private float _bottomBound = -9.45f;

    private void Start() =>
        _camera = GetComponent<Camera>();

    private void Update()
    {
        var screenWidth = Screen.width;
        var screenHeight = Screen.height;
        var halfSize = new Vector2(_camera.orthographicSize / screenHeight * screenWidth, _camera.orthographicSize);

        var newX = transform.position.x;
        if (newX + halfSize.x > _rightBound)
            newX = _rightBound - halfSize.x;
        else if (newX - halfSize.x < _leftBound)
            newX = _leftBound + halfSize.x;
        
        var newY = transform.position.y;
        if (newY - halfSize.y < _bottomBound)
            newY = _bottomBound + halfSize.y;
        
        transform.position = new Vector3(newX, newY, transform.position.z);
    }

}
