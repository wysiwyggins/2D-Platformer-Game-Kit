using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DisableObject : MonoBehaviour
{
    [Tooltip("Destroy this object if its Y-axis position is less than this value.")]
    public int destroyBelowThisPosition = -50;

    void Start()
    {
        InvokeRepeating(nameof(Clock), Random.Range(0f, 1f), 1);
    }

    void Clock()
    {
        if (transform.position.y < destroyBelowThisPosition)
        {
            gameObject.SetActive(false);
        }
    }
}
