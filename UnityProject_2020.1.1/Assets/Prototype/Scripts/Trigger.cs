using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public bool zoomCameraIn;
    public bool zoomCameraOut;

    WaitForSeconds wait;

    void Start()
    {
        wait = new WaitForSeconds(0.25f);
        GetComponent<Renderer>().enabled = false;
    }

    IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        yield return wait;

        if (collision.gameObject.CompareTag("Player"))
        {

        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        var localScale = transform.localScale;
        Gizmos.DrawCube(transform.position, new Vector3(localScale.x, localScale.y, 0));
    }
}
