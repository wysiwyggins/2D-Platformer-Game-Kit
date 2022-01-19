using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMass : MonoBehaviour
{
    void Awake()
    {
        Vector3 ls = transform.localScale;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = (ls.x + ls.y + ls.z) / 30f;
        }

        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.mass = (ls.x + ls.y) / 20f;
        }
    }
}
