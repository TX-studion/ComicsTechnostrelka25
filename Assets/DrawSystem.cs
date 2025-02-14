using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DrawSystem : MonoBehaviour
{
    [SerializeField]Slider slider;
    [SerializeField] TMP_Text BrSizetext;
    private Collider _Collider;
    [SerializeField] private Texture2D _texture;
    private Touch _touch;
    private Camera _camera;
    [Range(2, 2000)]
    [SerializeField] private int _resolutionX;
    [Range(0, 2)]
    [SerializeField] private float step;
    [Range(2, 2000)]
    [SerializeField] private int _resolutionY;
    private int oldX;
    private int oldY; 
    private List<Vector2> _Coords = new List<Vector2>(10000);
    private List<Color> _Colors = new List<Color>(10000);
    private Vector2 Lastpixel;

    public List<List<Vector2>> _UndoLists = new List<List<Vector2>>(10000);
    public List<List<Color>> _ColorUndo = new List<List<Color>>();
    public List<List<Vector2>> _RedoLists = new List<List<Vector2>>(10000);
    public List<List<Color>> _ColorRedo = new List<List<Color>>();
    public Color color;
    public int BrushSize;
    public string DrawType;
    public GameObject obj;
    private GameObject cursor;
    RaycastHit hit;
    Ray ray;
    public bool CanDraw;
    float r2, x2, y2;
    Color OldColor, newColor;

    void Start()
    {
        _camera = Camera.main;
        _texture = new Texture2D(_resolutionX, _resolutionY);
        GetComponent<Renderer>().material.mainTexture = _texture;
        _texture.wrapMode = TextureWrapMode.Clamp;
        _texture.filterMode = FilterMode.Bilinear;
        _Collider = GetComponent<Collider>();
        for (int x = 0; x < _resolutionX; x++)
        {
            for (int y = 0; y < _resolutionY; y++)
            {

                _texture.SetPixel(x, y, Color.white);

            }
        }        
        _texture.Apply();
        Reload();
    }
    void Update()
    {
       
        if (Input.touchCount == 1 && CanDraw == true)
        {
            _touch = Input.GetTouch(0);
            if(_touch.phase == TouchPhase.Began)
            {

                obj.transform.position = _camera.ScreenToWorldPoint(_touch.position);
            }
            

            obj.transform.position = Vector3.MoveTowards(obj.transform.position, _camera.ScreenToWorldPoint(_touch.position), step * BrushSize / 4);
        }
        else
        {
            obj.gameObject.SetActive(false);

        }
    }


    void FixedUpdate()
    {

        if (Input.touchCount == 1)
        {
            
            if(Physics.Raycast(obj.transform.position,Vector3.forward, out hit, Mathf.Infinity) && _camera.WorldToScreenPoint(obj.transform.position).y < 1300 && _camera.WorldToScreenPoint(obj.transform.position).y > 320)
            {
                obj.gameObject.SetActive(true);
                int rayX = (int)(hit.textureCoord.x * _resolutionX);
                int rayY = (int)(hit.textureCoord.y * _resolutionY);  
                if (DrawType == "Circle")
                {
                    DrawCircle(rayX, rayY, color);
                }
                else if (DrawType == "Eraser")
                {
                    Erase(rayX, rayY);
                }
            }
                
        }
        else
        {
            if (_Coords.Count != 0)
            {
                _UndoLists.Add(_Coords);
                _Coords.Clear();
                _ColorUndo.Add(_Colors);
                _Colors.Clear();
                Lastpixel = new Vector2(1000, 1000);
            }
        }
    }

   void Line(int rayX, int rayY, int _oldX, int _oldY)
    {

        int deltaX = Math.Abs(rayX - _oldX);
        int deltaY = Math.Abs(rayY - _oldY);
        float error = 0;
        
        float deltaerr = (deltaY + 1) / (deltaX + 1);
        int dirY = rayY - _oldY;
        int y = _oldY;
        if (dirY < 0)
        {
            dirY = -1;
        }
        if (dirY > 0)
        {
            dirY = 1;
        }
        if (_oldX > rayX)
        {
            for (int x = _oldX; x > rayX; x--)
            {
                Debug.Log(x + " " + y);
                DrawCircle(x, y, color);
                error = error + deltaerr;
                if (error >= 1.0f)
                {
                    y = y + dirY;
                    error = error - 1.0f;
                }

            }
        }
        else if (_oldX < rayX)
        {
            for (int x = _oldX; x < rayX; x++)
            {
                DrawCircle(x, y, color);
                error = error + deltaerr;
                if (error >= 1.0f) 
                {
                    y = y + dirY;
                    error = error - 1.0f;
                }
            }
                
        }
    }
    void BucketFill(int rayX, int rayY)
    {
        Color FillColor = _texture.GetPixel(rayX, rayY);



    }
    
    void DrawCircle(int rayX, int rayY, Color color)
    {
        if (Lastpixel != new Vector2(1000, 1000))
        {
            if (new Vector2(rayX, rayY) != Lastpixel)
            {
                for (int x = 0; x < BrushSize; x++)
                {
                    x2 = Mathf.Pow(x - BrushSize / 2, 2);
                    for (int y = 0; y < BrushSize; y++)
                    {
                        y2 = Mathf.Pow(y - BrushSize / 2, 2);
                        if (rayX + x - BrushSize / 2 >= 0 && rayX + x - BrushSize / 2 < _resolutionX && rayY + y - BrushSize / 2 >= 0 && rayY + y - BrushSize / 2 < _resolutionY)
                        {
                            if (x2 + y2 < r2)
                            {
                                OldColor = _texture.GetPixel(rayX + x - BrushSize / 2, rayY + y - BrushSize / 2);
                                if (color.a != 1)
                                {
                                    newColor = Color.Lerp(OldColor, color, color.a);
                                    _texture.SetPixel(rayX + x - BrushSize / 2, rayY + y - BrushSize / 2, newColor);
                                }
                                else
                                {
                                    _texture.SetPixel(rayX + x - BrushSize / 2, rayY + y - BrushSize / 2, color);
                                }



                            }
                        }
                    }
                }
               


            }

        }
        else
        {
            for (int x = 0; x < BrushSize; x++)
            {
                x2 = Mathf.Pow(x - BrushSize / 2, 2);
                for (int y = 0; y < BrushSize; y++)
                {
                    y2 = Mathf.Pow(y - BrushSize / 2, 2);
                    if (rayX + x - BrushSize / 2 >= 0 && rayX + x - BrushSize / 2 < _resolutionX && rayY + y - BrushSize / 2 >= 0 && rayY + y - BrushSize / 2 < _resolutionY)
                    {
                        if (x2 + y2 < r2)
                        {
                            OldColor = _texture.GetPixel(rayX + x - BrushSize / 2, rayY + y - BrushSize / 2);
                            newColor = Color.Lerp(OldColor, color, color.a);
                            _texture.SetPixel(rayX + x - BrushSize / 2, rayY + y - BrushSize / 2, newColor);
                            _Coords.Add(new Vector2(rayX + x - BrushSize / 2, rayY + y - BrushSize / 2));
                            _Colors.Add(OldColor);
                            Lastpixel = new Vector2(rayX, rayY);

                        }
                    }
                }
            }
        }
        _Coords.Add(new Vector2(rayX, rayY));
        _Colors.Add(OldColor);
        Lastpixel = new Vector2(rayX, rayY);
        _texture.Apply(false);
    }
    void Erase(int rayX, int rayY)
    {
        if(new Vector2(rayX, rayY) != Lastpixel)
        {
            for (int x = 0; x < BrushSize; x++)
            {
                for (int y = 0; y < BrushSize; y++)
                {


                    Color OldColor = _texture.GetPixel(rayX + x - BrushSize / 2, rayY + y - BrushSize / 2);
                    _texture.SetPixel(rayX + x - BrushSize / 2, rayY + y - BrushSize / 2, new Color(1, 1, 1, 0));
                    _Coords.Add(new Vector2(rayX + x - BrushSize / 2, rayY + y - BrushSize / 2));
                    _Colors.Add(OldColor);
                    Lastpixel = new Vector2(rayX, rayY);

                }
            }
            _texture.Apply();
        }
        

    }
    public void Reload()
    {

        BrushSize = (int)slider.value;
        BrSizetext.text = "Размер: " + BrushSize.ToString();
        r2 = Mathf.Pow(BrushSize / 2 - 0.5f, 2);
        obj.transform.localScale = new Vector3(BrushSize / 3, BrushSize / 3, 0);
    }
    public void Undo()
    {
        if(_UndoLists.Count > 0)
        {
            List<Vector2> toUndo = _UndoLists[_UndoLists.Count - 1];
            Debug.Log(toUndo.Count);
            List<Color> toUndoColors = _ColorUndo[_ColorUndo.Count - 1];
            _UndoLists.RemoveAt(_UndoLists.Count - 1);
            _ColorUndo.RemoveAt(_ColorUndo.Count - 1);
            for(int i = 0; i < toUndo.Count - 1; i++) 
            {
            }
            _ColorRedo.Add(toUndoColors);
            _RedoLists.Add(toUndo);
            _texture.Apply();
        }

    }
    public void Redo()
    {
        if (_RedoLists.Count > 0)
        {
            List<Vector2> toRedo = _RedoLists[_RedoLists.Count - 1];
            List<Color> toRedoColors = _ColorRedo[_ColorRedo.Count - 1];
            _RedoLists.RemoveAt(_RedoLists.Count - 1);
            _ColorRedo.RemoveAt(_ColorRedo.Count - 1);
            for (int i = 0; i != toRedo.Count - 1; i++)
            {
                _texture.SetPixel((int)toRedo[i].x, (int)toRedo[i].y, toRedoColors[i]);
            }
            _ColorUndo.Add(toRedoColors);
            _UndoLists.Add(toRedo);
            _texture.Apply();
        }
    }
    public void Change()
    {
        DrawType = gameObject.name;
    }

    
}
