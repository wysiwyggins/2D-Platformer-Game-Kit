using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{
    Transform target;
    public float speed = 1;

    float lerp;
    Vector3 offset;
    Camera cam;
    float origSize, targetSize;
    float velocity;

    void Start()
    {
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        transform.parent = null;
        offset = transform.position - target.position;
        cam = GetComponent<Camera>();
        origSize = cam.orthographicSize;
        targetSize = origSize;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Compute offset and match camera x position to target x position
            var targetPosition = target.position + offset;
            targetPosition.x = target.position.x;

            // Speed up camera if target moves fast
            var s = speed;
            if (Vector3.Distance(targetPosition, transform.position) > 10)
            {
                s = speed * 3;
            }

            // Adjust lerp speed
            lerp = Mathf.Lerp(lerp, s, Time.deltaTime * 5);
            //targetPosition.z = transform.position.z;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerp);

            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref velocity, 0.5f);
        }
    }

    public void ZoomIn()
    {
        targetSize = origSize / 2;
    }

    public void ZoomOut()
    {
        targetSize = origSize * 2;
    }

    public void ZoomReset()
    {
        targetSize = origSize;
    }
}
