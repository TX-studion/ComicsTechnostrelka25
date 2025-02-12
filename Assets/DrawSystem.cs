using TMPro;
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
    [Range(2, 2000)]
    [SerializeField] private int _resolutionY;
    private int _oldX = 0;
    private int _oldY = 0;
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
    RaycastHit hit;
    Ray ray;
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



    void FixedUpdate()
    {
        
        if (Input.touchCount == 1)
        {
            _touch = Input.GetTouch(0);
            ray = _camera.ScreenPointToRay(_touch.position);
            if(_Collider.Raycast(ray, out hit, Mathf.Infinity))
            {
                
                int rayX = (int)(hit.textureCoord.x * _resolutionX);
                int rayY = (int)(hit.textureCoord.y * _resolutionY);
                Debug.Log(111);   
                if (DrawType == "Circle")
                {
                    DrawCircle(rayX, rayY);
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
                Lastpixel = null;
            }
        }
    }

        
    
    void DrawCircle(int rayX, int rayY)
    {
        if (Lastpixel != null)
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
                                newColor = Color.Lerp(OldColor, color, color.a);
                                _texture.SetPixel(rayX + x - BrushSize / 2, rayY + y - BrushSize / 2, newColor);
                                _Coords.Add(new Vector2(rayX + x - BrushSize / 2, rayY + y - BrushSize / 2));
                                _Colors.Add(OldColor);
                                Lastpixel = new Vector2(rayX, rayY);
                                
                            }
                        }
                    }
                }
                int deltaX = math.abs(rayX - _oldX);
                int deltaY = math.abs(rayY - _oldY);
                float error = 0;
                float direrr = (deltaY + 1) / (deltaX + 1);
                int dirY = rayY - _oldY;
                int y = _oldX;
                if (dirY < 0)
                {
                    dirY = -1;
                }
                if (diry > 0)
                {
                    dirY = 1;
                }
                if (_oldX > rayX)
                {
                    for (int x = rayX; x < _oldX; x++)
                    {

                    }
                }
                else if (_oldX < rayX)
                {
                    for (int x = _oldX; x < rayX; x++)
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
                            _oldX = rayX;
                            _oldY = rayY;
                        }
                    }
                }
            }
        }
        _texture.Apply(false);
    }
    void Erase(int rayX, int rayY)
    {
        for (int x = 0; x < BrushSize; x++)
        {
            for (int y = 0; y < BrushSize; y++)
            {
                if (_Coords.Contains(new Vector2(rayX + x - BrushSize / 2, rayY + y - BrushSize / 2)) == false)
                {
                    Color OldColor = _texture.GetPixel(rayX + x - BrushSize / 2, rayY + y - BrushSize / 2);
                    _texture.SetPixel(rayX + x - BrushSize / 2, rayY + y - BrushSize / 2, new Color(1, 1, 1, 0));
                    _Coords.Add(new Vector2(rayX + x - BrushSize / 2, rayY + y - BrushSize / 2));
                    _Colors.Add(OldColor);
                }
            }
        }
        _texture.Apply();
    }
    public void Reload()
    {
        BrushSize = (int)slider.value;
        BrSizetext.text = "������: " + BrushSize.ToString();
        r2 = Mathf.Pow(BrushSize / 2 - 0.5f, 2);
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
                _texture.SetPixel((int)toUndo[i].x, (int)toUndo[i].y, toUndoColors[i]);
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
