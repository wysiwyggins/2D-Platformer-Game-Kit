using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutPanel : MonoBehaviour
{
    Image panel;
    Color origColor;
    Color targetColor;
    float t = 0;
    bool go;

    void Start()
    {
        panel = GetComponent<Image>();
        origColor = panel.color;
        targetColor = panel.color;
        targetColor.a = 0;
        Invoke(nameof(Go), 1f);
    }

    void Update()
    {
        if (go)
        {
            panel.color = Color.Lerp(origColor, targetColor, Mathf.Clamp01(t));
            t += Time.deltaTime;

            if (t > 1)
            {
                gameObject.SetActive(false);
            }
        }
    }

    void Go()
    {
        go = true;
    }
}
