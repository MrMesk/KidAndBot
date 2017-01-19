using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PavNumPrezShortcut : MonoBehaviour {

    public KeyCode scene1;
    public string scene1Name;

    public KeyCode scene2;
    public string scene2Name;

    public KeyCode slide;
    public GameObject slideGo;

    private void Update()
    {
        if (Input.GetKeyDown(scene1))
        {
            Debug.Log("scene1Name");
            SceneManager.LoadScene(scene1Name);
        }
        else if (Input.GetKeyDown(scene2))
        {
            SceneManager.LoadScene(scene2Name);
        }
        else if(Input.GetKeyDown(slide))
        {
            // toogle
            slideGo.SetActive(!slideGo.activeInHierarchy);
        }
    }
}
