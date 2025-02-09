using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraControl : MonoBehaviour
{
    public float maxZoom;
    public float minZoom;
    public float sensitivity;
    public float moveSpeed;
    public float rotateSpeed;
    public bool move;

    private Camera _camera;
    private Touch _firstT;
    private Touch _secondT;
    private Vector2 _firstT_direction;
    private Vector2 _secondT_direction;
    private float _distanceTPos;
    private float _distanceTDir;
    private float _zoom;
    private Vector3 _lastTouchPosition;
    private float _lastDistance;

    private void Awake()
    {
        _camera = Camera.main;
    }
    private void Update()
    {

        if (Input.touchCount == 2)
        {
            move = true;
            _firstT = Input.GetTouch(0);
            _secondT = Input.GetTouch(1);


            _firstT_direction = _firstT.position - _firstT.deltaPosition;
            _secondT_direction = _secondT.position - _secondT.deltaPosition;

            _distanceTPos = Vector2.Distance(_firstT.position, _secondT.position);
            _distanceTDir = Vector2.Distance(_firstT_direction, _secondT_direction);
            _zoom = _distanceTPos - _distanceTDir;


            var currentZoom = _camera.orthographicSize - _zoom * sensitivity * Time.deltaTime * _camera.orthographicSize;
            if (true)
            {
                _camera.orthographicSize = Mathf.Clamp(currentZoom, minZoom, maxZoom);
            }


            if (_secondT_direction != _secondT.position)
            {
                var angle = Vector3.SignedAngle(_secondT.position - _firstT.position, _secondT_direction - _firstT_direction, _camera.transform.forward);
                _camera.transform.RotateAround(_camera.transform.position, _camera.transform.forward, angle * Time.deltaTime * rotateSpeed);
            }
            if (_firstT.phase == TouchPhase.Began)
            {
                _lastDistance = _distanceTDir;
            }
            if (_firstT.phase == TouchPhase.Moved & math.abs(_distanceTDir - _lastDistance) > 5)
            {
                Vector3 Dir = Camera.main.ScreenToWorldPoint(_firstT.position) - Camera.main.ScreenToWorldPoint(_firstT_direction);

                _camera.transform.position -= Dir.normalized * Time.deltaTime * moveSpeed;
                if (_camera.transform.position.x < -30)
                {
                    _camera.transform.position = new Vector3(-30, transform.position.y, 0);
                }
                if (_camera.transform.position.x > 30)
                {
                    _camera.transform.position = new Vector3(30, transform.position.y, 0);
                }
                if (_camera.transform.position.y < -30)
                {
                    _camera.transform.position = new Vector3(transform.position.x, -30, 0);
                }
                if (_camera.transform.position.y > 30)
                {
                    _camera.transform.position = new Vector3(transform.position.x, 30, 0);
                }
            }

        }
        else
        {
            move = false;
        }
    }
}