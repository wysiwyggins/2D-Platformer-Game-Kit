using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInPanel : MonoBehaviour
{
    public Image panel;
    Color origColor;
    Color targetColor;
    float t = 0;

    void Start()
    {
        origColor = panel.color;
        origColor.a = 0;
        targetColor = panel.color;
        targetColor.a = 1;

    }

    void Update()
    {
        panel.color = Color.Lerp(origColor, targetColor, Mathf.Clamp01(t));
        t += Time.unscaledDeltaTime;

        if (t > 1)
        {
            enabled = false;
        }
    }
}
