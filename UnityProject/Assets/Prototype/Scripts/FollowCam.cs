using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{
    public Transform target;
    public float speed = 1;
    Vector3 targetPosition, offset;

    void Start()
    {
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        targetPosition = target.position + offset;
        targetPosition.x = target.position.x;
        targetPosition.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
    }
}
