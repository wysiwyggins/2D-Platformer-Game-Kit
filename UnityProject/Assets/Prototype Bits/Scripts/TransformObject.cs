using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformObject : MonoBehaviour
{

    public bool isConstantlyMoving = false;
    public List<Vector3> movePath;
    int movePathIndex = 0;

    float origTransformDuration;
    public float transformDuration = 0;


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

        if(isConstantlyMoving)
        {
            StartCoroutine(MoveConstantly());
        }

    }


    void FixedUpdate()
    {
        
    }

    
    public IEnumerator MoveConstantly()
    {
        movePathIndex = (movePathIndex + 1) % movePath.Count;

        yield return null;
    }

    public IEnumerator Move(Vector3 targetPosition)
    {
        yield return null;
    }

    public IEnumerator Rotate(Quaternion targetRotation)
    {
        yield return null;
    }

    public IEnumerator Scale(Vector3 targetScale)
    {
        yield return null;
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



}
