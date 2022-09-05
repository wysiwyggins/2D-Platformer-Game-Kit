using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutPanel : MonoBehaviour
{
    public Image panel;
    Color origColor;
    Color targetColor;
    float t = 0;

    void Start()
    {
        origColor = panel.color;
        origColor.a = 1;
        targetColor = panel.color;
        targetColor.a = 0;

    }

    void Update()
    {
        panel.color = Color.Lerp(origColor, targetColor, Mathf.Clamp01(t));
        t += Time.unscaledDeltaTime;

        if (t > 1)
        {
            panel.gameObject.SetActive(false);
            enabled = false;
        }
    }
}
