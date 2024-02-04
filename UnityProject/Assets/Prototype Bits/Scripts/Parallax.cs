using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float startPosition;
    private float spriteLength; //This is the length of the sprites.
    public float parallaxAmount; //This is amount of parallax scroll. 
    public Camera mainCamera; //Reference of the camera.



    private void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        startPosition = transform.position.x;
        spriteLength = GetComponent<SpriteRenderer>().bounds.size.x;
    }



    private void Update()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        float temp = cameraPosition.x * (1 - parallaxAmount);
        float distance = cameraPosition.x * parallaxAmount;

        Vector3 newPosition = new Vector3(startPosition + distance, transform.position.y, transform.position.z);

        transform.position = newPosition;

        if (temp > startPosition + (spriteLength / 2))
        {
            startPosition += spriteLength;
        }
        else if (temp < startPosition - (spriteLength / 2))
        {
            startPosition -= spriteLength;
        }
    }
}
