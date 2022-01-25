using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColor : MonoBehaviour
{
    public bool glow;

    void Awake()
    {
        RefreshColor();
    }

    public void SetColor(Color color, bool glow)
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        var _EmissionColor = Shader.PropertyToID("_EmissionColor");
        var block = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(block);
        block.SetVector(_EmissionColor, glow ? (Vector4)color * 3 : (Vector4)color);
        spriteRenderer.SetPropertyBlock(block);
    }

    void RefreshColor()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        SetColor(spriteRenderer.color, glow);
    }

    void OnValidate()
    {
        RefreshColor();
    }

    //void OnDrawGizmos()
    //{
    //    RefreshColor();
    //}
}
