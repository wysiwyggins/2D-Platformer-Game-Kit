using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DisableObject : MonoBehaviour
{
    [Tooltip("Disable this object if its Y-axis position is less than this value.")]
    public int threshold = -50;

    void Start()
    {
        InvokeRepeating(nameof(Clock), Random.Range(0f, 1f), 1);
    }

    void Clock()
    {
        if (transform.position.y < threshold)
        {
            gameObject.SetActive(false);
        }
    }
}
