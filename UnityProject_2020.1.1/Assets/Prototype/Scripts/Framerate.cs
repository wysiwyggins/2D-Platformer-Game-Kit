using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Framerate : MonoBehaviour
{
    public Text uiText;
    public bool startEnabled;

    float framerate;
    float f, t, rate, lerp;
    string[] strInts = new string[] { };

    void Awake()
    {
        framerate = 60;
        rate = framerate;

        if (strInts.Length != 10000)
        {
            string padding = "";
            strInts = new string[10000]; // 10000 4 byte values = 40K in memory
            for (int i = 0; i < strInts.Length; i++)
            {
                if (i < 10) { padding = "   "; }
                if (i > 9 && i < 100) { padding = "  "; }
                if (i > 99 && i < 1000) { padding = " "; }
                strInts[i] = i.ToString() + padding;
            }
        }

        uiText.enabled = startEnabled;
    }

    void Update()
    {
        var freq = 0.5f;
        f++;
        lerp = rate / framerate;
        framerate = Mathf.Lerp(framerate, rate, lerp * Time.unscaledDeltaTime);

        t += Time.unscaledDeltaTime;
        if (t >= freq)
        {
            rate = f / freq;
            var index = (int)framerate;
            if (index < strInts.Length)
            {
                uiText.text = GetStringInt(index);
            }

            t = 0;
            f = 0;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            uiText.enabled = !uiText.enabled;
        }
    }

    string GetStringInt(int index)
    {
        return strInts[Mathf.Clamp(index, 0, 9999)];
    }
}
