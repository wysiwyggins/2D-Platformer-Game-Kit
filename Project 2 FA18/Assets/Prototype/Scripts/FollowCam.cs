using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{
    public Transform target;
    public float speed = 1;

    float lerp;
    Vector3 offset;
    new Camera camera;
    float origSize, targetSize;

    void Start()
    {
        offset = transform.position - target.position;
        camera = GetComponent<Camera>();
        origSize = camera.orthographicSize;
        targetSize = origSize;
    }

    void LateUpdate()
    {
        var targetPosition = target.position + offset;
        targetPosition.x = target.position.x;
        var s = speed;

        if (Vector3.Distance(targetPosition, transform.position) > 10)
        {
            s = speed * 3;
        }

        lerp = Mathf.Lerp(lerp, s, Time.deltaTime * 5);
        targetPosition.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerp);

        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetSize, Time.deltaTime * 5);
    }

    public void ZoomIn()
    {
        targetSize = origSize / 2;
    }

    public void ZoomOut()
    {
        targetSize = origSize * 2;
    }

    public void ResetZoom()
    {
        targetSize = origSize;
    }
}
