using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class SVImageControl : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    [SerializeField] Image pickerIm;

    private RawImage SVImage;
    
    private ColorPicker cp;

    private RectTransform rectTransform, pickerTransform;

    private void Awake()
    {
        SVImage = GetComponent<RawImage>();
        cp = FindAnyObjectByType<ColorPicker>();
        rectTransform = GetComponent<RectTransform>();

        pickerTransform = pickerIm.GetComponent<RectTransform>();
        pickerTransform.position = new Vector2(-(rectTransform.sizeDelta.x * 0.5f), -(rectTransform.sizeDelta.y * 0.5f));
    }

    void UpdateColour(PointerEventData eventData)
    {
        Vector3 pos = rectTransform.InverseTransformPoint(eventData.position);

        float deltaX = rectTransform.sizeDelta.x * 0.5f;
        float deltaY = rectTransform.sizeDelta.y * 0.5f;

        if (pos.x < -deltaX)
        {
            pos.x = -deltaX;
        }
        else if (pos.x > deltaX) 
        {
            pos.x = deltaX;
        }
        if (pos.y < -deltaY)
        {
            pos.y = -deltaY;
        }
        else if(pos.y > deltaY)
        {
            pos.y = deltaY;
        }

        float x = pos.x + deltaX;
        float y = pos.y + deltaY;

        float xNorm = x / rectTransform.sizeDelta.x;
        float yNorm = y / rectTransform.sizeDelta.y;

        pickerTransform.localPosition = pos;

        cp.SetSV(xNorm, yNorm);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateColour(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateColour(eventData);
    }

}
