using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComicsManager : MonoBehaviour
{
    public GameObject previewPrefab;
    public float StandartY;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("ColvoProjects", 4);
        Library();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void NewProject(string name)
    {
        PlayerPrefs.SetInt("ColvoProjects", +1);
        
    }
    void Library()
    {
        Debug.Log(PlayerPrefs.GetInt("ColvoProjects"));

        int down = 0;
        for (int i = 0; i < PlayerPrefs.GetInt("ColvoProjects"); i++)
        {
            if (i % 3 == 0)
            {
                GameObject preview = Instantiate(previewPrefab, gameObject.transform);
                preview.transform.position = new Vector3(-320, StandartY - down, 0);
                Texture2D _tex = preview.GetComponent<Texture2D>();
            }
            else if (i % 3 == 1)
            {
                GameObject preview = Instantiate(previewPrefab, new Vector3(0, StandartY - down, 0), transform.rotation);
                preview.transform.position = new Vector3(-320, StandartY - down, 0);
                Texture2D _tex = preview.GetComponent<Texture2D>();
            }
            else
            {
                GameObject preview = Instantiate(previewPrefab, new Vector3(320, StandartY - down, 0), transform.rotation);
                preview.transform.position = new Vector3(-320, StandartY - down, 0);
                Texture2D _tex = preview.GetComponent<Texture2D>();
                down = down + 500;
            }
        }
    }
}
