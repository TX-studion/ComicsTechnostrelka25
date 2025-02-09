using Unity.UI;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Mathematics;

public class ColorPicker : MonoBehaviour
{
    public float currentHue, currentSaturation, currentValue, currentTransparent;

    [SerializeField]private RawImage hueImage, satValImage, transpImage, transpBGImage;
    [SerializeField]private Slider hueSlider, transparentSlider;

    [SerializeField]private TMP_InputField inputField;

    [SerializeField]private Texture2D hueTexture, svTexture, transpTexture, transpBGTexture;

    [SerializeField] MeshRenderer changeColor;
    DrawSystem Ds;

    [SerializeField] private Image OutputImage;

    private void Update()
    {
        Color currentColor = Color.HSVToRGB(currentHue, currentSaturation, currentValue);
        for (int i = 0; i < 101; i++)
        {
            Color TrColor = Color.Lerp(new Color(currentColor.r, currentColor.g, currentColor.b, 0), currentColor, i / 97.43f);
            transpTexture.SetPixel(0, i, TrColor);
        }
        transpTexture.Apply();
    }

    private void Start()
    {
        CreateHueImage();
        CreateSaturationImage();
        CreateTransparentImage();
        Ds = FindAnyObjectByType<DrawSystem>();
        UpdateOutputColor();

    }
    private void CreateHueImage()
    {
        hueTexture = new Texture2D(1, 16);
        hueTexture.wrapMode = TextureWrapMode.Clamp;
        hueTexture.name = "HueTexture";
        for (int i = 0; i < 16; i++)
        {
            hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / 16, 1, 1));
        }
        hueTexture.Apply();

        currentHue = 0;
        hueImage.texture = hueTexture;
    }
    private void CreateSaturationImage()
    {
        svTexture = new Texture2D(16, 16);
        svTexture.wrapMode = TextureWrapMode.Clamp;
        svTexture.name = "SatValTexture";

        for (int i = 0; i < 16; i++)
        {
            for (int x = 0; x < 16; x++)
            {
                svTexture.SetPixel(x, i, Color.HSVToRGB(currentHue, (float)x / 16, (float)i / 16));
            }
        }
        svTexture.Apply();
        currentSaturation = 0;
        currentValue = 0;

        satValImage.texture = svTexture;
    }

    private void CreateTransparentImage()
    {
        transpBGTexture = new Texture2D(4, 32);
        transpBGTexture.wrapMode = TextureWrapMode.Clamp;
        transpBGTexture.filterMode = FilterMode.Point;
        for (int i = 0; i < 64; i++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (i % 2 == 0 & x % 2 == 0)
                {
                    transpBGTexture.SetPixel(x, i, Color.white);
                }
                else if (i % 2 == 1 & x % 2 == 0)
                {
                    transpBGTexture.SetPixel(x, i, Color.HSVToRGB(0, 0, 0.8f));
                }
                else if (i % 2 == 0 & x % 2 == 1)
                {
                    transpBGTexture.SetPixel(x, i, Color.HSVToRGB(0, 0, 0.8f));
                }
                else
                {
                    transpBGTexture.SetPixel(x, i, Color.white);
                }
            }
        }
        transpBGTexture.Apply();
        transpBGImage.texture = transpBGTexture;


        transpTexture = new Texture2D(1, 100);
        transpTexture.wrapMode = TextureWrapMode.Clamp;
        transpTexture.name = "TransparentTexture";

        currentTransparent = 1;
        transpImage.texture = transpTexture;
    }
    private void UpdateOutputColor()
    {
        Ds.color = Color.HSVToRGB(currentHue, currentSaturation, currentValue);
        Ds.color.a = currentTransparent;
   
        OutputImage.color = Ds.color;
    }

    public void SetSV(float S, float V)
    {
        currentSaturation = S;
        currentValue = V;   

        UpdateOutputColor();
    }
    public void UpdateSVImage()
    {
        currentHue = hueSlider.value;
        currentTransparent = transparentSlider.value;

        for (int i = 0; i < 16; i++)
        {
            for (int x = 0; x < 16; x++)
            {
                svTexture.SetPixel(x, i, Color.HSVToRGB(currentHue, (float)x / 16, (float)i / 16));
            }
        }


        svTexture.Apply();
        UpdateOutputColor();
    }

}
