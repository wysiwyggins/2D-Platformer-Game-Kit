using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TransformObject : MonoBehaviour
{

    public float transformDuration = 0;
    public List<Vector3> movePath;
    int movePathIndex = 0;
    public List<Quaternion> rotatePath;
    int rotatePathIndex = 0;
    public List<Vector3> scalePath;
    int scalePathIndex = 0;

    bool isMoving = false;
    bool isRotating = false;
    bool isScaling = false;



    // Vector3 origScale;
    // Vector3 targetScale;
    // Quaternion origRotation;
    // Quaternion targetRotation;
    // Vector3 origPosition;
    // Vector3 targetPosition;


    private Rigidbody2D rb;
    private bool rbOrigKinematic = true; //if object is originally kinematic, true by default


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); //gets object's Rigidbody component if it has one
        rbOrigKinematic = rb.isKinematic; //store original rb value

        //origScale = transform.localScale;
        //origRot = Quaternion.Euler(0f, 0f, rb.rotation);
        //origPosition = rb.position;

        movePath.Insert(0, transform.position);
        rotatePath.Insert(0, transform.rotation);
        scalePath.Insert(0, transform.localScale);


    }

    void FixedUpdate()
    {
        if(movePath.Count > 1)
        {
            if(!isMoving)
            {
                movePathIndex = (movePathIndex + 1) % movePath.Count;
                StartCoroutine(Move(movePath[movePathIndex], transformDuration));
            }
        }
        if(rotatePath.Count > 1)
        {
            if(!isRotating)
            {       
                rotatePathIndex = (rotatePathIndex + 1) % rotatePath.Count;
                StartCoroutine(Rotate(rotatePath[rotatePathIndex], transformDuration));
            }
        }
        if(scalePath.Count > 1)
        {
            if(!isScaling)
            {
                scalePathIndex = (scalePathIndex + 1) % scalePath.Count;
                StartCoroutine(Scale(scalePath[scalePathIndex], transformDuration));
            }
        }
    }



    public IEnumerator Move(Vector3 targetPosition, float duration)
    {
        isMoving = true;
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }

    public IEnumerator Rotate(Quaternion targetRotation, float duration)
    {
        isRotating = true;
        float elapsedTime = 0f;
        Quaternion startingRotation = transform.rotation;

        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Lerp(startingRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        isRotating = false;
    }

    public IEnumerator Scale(Vector3 targetScale, float duration)
    {
        isScaling = true;
        float elapsedTime = 0f;
        Vector3 startingScale = transform.localScale;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startingScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        isScaling = false;
    }


    // public void SetMoveTarget(Vector3 target)
    // {
    //     targetPosition = target;
    // }

    // public void SetRotateTarget(Quaternion target)
    // {
    //     targetRotation = target;
    // }

    // public void SetScaleTarget(Vector3 target)
    // {
    //     targetScale = target;
    // }
    

    #if UNITY_EDITOR

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            foreach(var i in movePath)
            {
                //Gizmos.DrawWireCube(movePath[i], Vector3.one);
            }
        }

    #endif

}


