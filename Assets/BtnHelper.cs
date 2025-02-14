using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnHelper : MonoBehaviour
{
    public GameObject obj;
    DrawSystem ds;
    private void Awake()
    {
        ds = FindAnyObjectByType<DrawSystem>();

    }
    public void Name()
    {
        ds.name = gameObject.name;
    }
    public void on_off()
    {
        if (obj.activeSelf)
        {
            ds.CanDraw = true;
            obj.SetActive(false);
        }
        else
        {
            ds.CanDraw = false;
            obj.SetActive(true);
        }

    }


}
