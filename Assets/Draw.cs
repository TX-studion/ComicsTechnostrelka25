using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Draw : MonoBehaviour
{
    public Camera m_camera;
    public GameObject brush;
    private Touch _touch;
    LineRenderer currentLineRenderer;
    public List<GameObject> _lines = new List<GameObject>(10000);
    public List<GameObject> _redoLines = new List<GameObject>(10000);
    public CameraControl _crl;
    public LayerMask _layerMask;

    Vector2 lastPos;
    private void Awake()
    {
        m_camera = Camera.main;
        _crl = FindAnyObjectByType<CameraControl>();
    }
    private void Update()
    { 
        Drawing();
    }

    void Drawing()
    {
        if (Input.touchCount == 1)
        {

            _touch = Input.GetTouch(0);

            if (currentLineRenderer == null)
            {
                CreateBrush();
            }
            else if (_touch.phase == TouchPhase.Moved)
            {
                PointToMousePos();
            }



        }
        else
        {
            if (currentLineRenderer != null )
            {
                if (currentLineRenderer.positionCount < 5)
                {
                    Destroy(_lines[_lines.Count - 1]);
                    _lines.RemoveAt(_lines.Count - 1);
                }

            }
            currentLineRenderer = null;
        }

    }

    void CreateBrush()
    {
        Vector2 _touchPos = m_camera.ScreenToWorldPoint(_touch.position);
        if (Physics2D.OverlapCircle(_touchPos, 0.1f, _layerMask))
        {
            GameObject brushInstance = Instantiate(brush);
            currentLineRenderer = brushInstance.GetComponent<LineRenderer>();
            _lines.Add(brushInstance);
            _redoLines = null;
            //because you gotta have 2 points to start a line renderer, 
            brushInstance.SetActive(false);
            currentLineRenderer.SetPosition(0, _touchPos);
            currentLineRenderer.SetPosition(1, _touchPos);

        }

    }

    void AddAPoint(Vector2 pointPos)
    {
        if (Physics2D.OverlapCircle(pointPos,0.01f, _layerMask))
        {
            _lines[_lines.Count - 1].gameObject.SetActive(true);
            currentLineRenderer.positionCount++;
            int positionIndex = currentLineRenderer.positionCount - 1;
            currentLineRenderer.SetPosition(positionIndex, pointPos);
        }

    }

    void PointToMousePos()
    {

        Vector2 _touchPos = m_camera.ScreenToWorldPoint(_touch.position);
        if (lastPos != _touchPos)
        {
            AddAPoint(_touchPos);
            lastPos = _touchPos;
        }
    }
    public void Undo()
    {
        if (_lines.Count != 0)
        {
            GameObject line = _lines[_lines.Count - 1];
            _redoLines.Add(line);
            _lines[_lines.Count - 1].gameObject.SetActive(false);
            _lines.RemoveAt(_lines.Count - 1);
        }
    }
    public void Redo()
    {
        if (_redoLines.Count != 0)
        {
            GameObject line = _redoLines[_redoLines.Count - 1];
            _lines.Add(line);
            _lines[_lines.Count -1].gameObject.SetActive(true);
            _redoLines.RemoveAt(_redoLines.Count - 1);
        }
    }



}
