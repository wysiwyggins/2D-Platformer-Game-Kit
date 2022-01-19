using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroLevel : MonoBehaviour
{
    //public string message = "Level 1";
    public Text uiText;

    // Use this for initialization
    void Start()
    {
        if (uiText != null)
        {
            uiText.text = SceneManager.GetActiveScene().name;
            Invoke("ClearText", 3);
        }
    }

    void ClearText()
    {
        uiText.text = "";
    }
}
