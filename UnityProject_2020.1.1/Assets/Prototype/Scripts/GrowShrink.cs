using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowShrink : MonoBehaviour
{
    Vector3 origScale;
    public Vector3 targetScale;
    Vector3 velocity = Vector3.zero;

    void Awake()
    {
        origScale = transform.localScale;
        targetScale = origScale;
    }

    void Update()
    {
        transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref velocity, 0.1f);
    }

    public void Grow()
    {
        targetScale = origScale * 2;
    }

    public void Shrink()
    {
        targetScale = origScale / 2;
    }

    public void ResetSize()
    {
        targetScale = origScale;
    }
}
