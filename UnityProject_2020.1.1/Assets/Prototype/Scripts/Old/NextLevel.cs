using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class NextLevel : MonoBehaviour
{
    public float delay = 3;
    public string message = "Level Complete";
    public Text uiText;
    public bool reload;

    public string[] triggerTags = new string[] { "Player" };

    void OnTriggerEnter2D(Collider2D c)
    {
        foreach (string s in triggerTags)
        {
            if (s != null && c.gameObject.CompareTag(s))
            {
                if (uiText != null)
                {
                    uiText.text = message;
                }

                c.gameObject.SetActive(false);

                Invoke("Load", delay);
                return;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Load();
        }
    }

    void Load()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        if (!reload)
        {
            index += 1;
            if (index == SceneManager.sceneCountInBuildSettings) { index = 0; }
        }
        SceneManager.LoadScene(index);
    }
}