using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TransformObject : MonoBehaviour
{


    public enum TransformPathType {LINEAR, PINGPONG}
    [Tooltip("Determines the type of continuous movement. LINEAR moves only forwards through the list, PINGPONG moves forwards then backwards.")]
    public TransformPathType transformPathType;

    [Tooltip("The amount of time each transformation takes")]
    public float transformDuration = 0;
    [Tooltip("Moves continuously through starting position and the elements in the list")]
    public List<Vector3> movePath = new List<Vector3>();
    int movePathIndex = 0;
    [Tooltip("Rotates continuously through starting position and the elements in the list")]
    public List<float> rotatePath = new List<float>();
    int rotatePathIndex = 0;
    [Tooltip("Changes object's scale continuously through starting position and the elements in the list")]
    public List<Vector3> scalePath = new List<Vector3>();
    int scalePathIndex = 0;


    Coroutine activeMoveCoroutine = null;
    Coroutine activeRotateCoroutine = null;
    Coroutine activeScaleCoroutine = null;



    private Rigidbody2D rb;
    private bool isOrigKinematic = true; //if object is originally kinematic, true by default


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); //gets object's Rigidbody component if it has one
        isOrigKinematic = rb.isKinematic; //store original rb value


        movePath.Insert(0, transform.position);
        rotatePath.Insert(0, transform.rotation.eulerAngles.z);
        scalePath.Insert(0, transform.localScale);


    }

    void FixedUpdate()
    {
        if(movePath.Count > 1)
        {
            if(activeMoveCoroutine == null)
            {
                movePathIndex = DetermineIndex(movePathIndex, movePath);
                activeMoveCoroutine = StartCoroutine(Move(movePath[movePathIndex], transformDuration));
            }
        }
        if(rotatePath.Count > 1)
        {
            if(activeRotateCoroutine == null)
            {
                rotatePathIndex = DetermineIndex(rotatePathIndex, rotatePath);
                activeRotateCoroutine = StartCoroutine(Rotate(rotatePath[rotatePathIndex], transformDuration));
            }
        }
        if(scalePath.Count > 1)
        {
            if(activeScaleCoroutine == null)
            {
                scalePathIndex = DetermineIndex(scalePathIndex, scalePath);
                activeScaleCoroutine = StartCoroutine(Scale(scalePath[scalePathIndex], transformDuration));
            }
        }
    }



    private IEnumerator Move(Vector3 targetPosition, float duration)
    {
        if(!isOrigKinematic)
        {
            rb.isKinematic = true;
        }

        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        if(!isOrigKinematic)
        {
            rb.isKinematic = false;
        }

        activeMoveCoroutine = null;
    }

    private IEnumerator Rotate(float targetRotationDegrees, float duration)
    {
        if(!isOrigKinematic)
        {
            rb.isKinematic = true;
        }

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetRotationDegrees);
        float elapsedTime = 0f;
        Quaternion startingRotation = transform.rotation;

        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Lerp(startingRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;

        if(!isOrigKinematic)
        {
            rb.isKinematic = false;
        }

        activeRotateCoroutine = null;
    }

    private IEnumerator Scale(Vector3 targetScale, float duration)
    {
        if(!isOrigKinematic)
        {
            rb.isKinematic = true;
        }

        float elapsedTime = 0f;
        Vector3 startingScale = transform.localScale;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startingScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;

        if(!isOrigKinematic)
        {
            rb.isKinematic = false;
        }

        activeScaleCoroutine = null;
        //Debug.Log("scaling finished! Scaled to "+targetScale);
    }



    //Public methods that start the Move, Scale and Rotate Coroutines and stop them if the object is already moving.
    public void StartMove(Vector3 targetPosition, float duration)
    {
        if(activeMoveCoroutine != null)
        {
            StopCoroutine(activeMoveCoroutine);
        }

        activeMoveCoroutine = StartCoroutine(Move(targetPosition, duration));
    }

    public void StartRotate(float targetRotation, float duration)
    {
        if(activeRotateCoroutine != null)
        {
            StopCoroutine(activeRotateCoroutine);
        }

        activeRotateCoroutine = StartCoroutine(Rotate(targetRotation, duration));
    }

    public void StartScale(Vector3 targetScale, float duration)
    {
        if(activeScaleCoroutine != null)
        {
            StopCoroutine(activeScaleCoroutine);
        }

        activeScaleCoroutine = StartCoroutine(Scale(targetScale, duration));
    }

    public void Reset(float duration)
    {
        StartMove(movePath[0], duration);
        StartRotate(rotatePath[0], duration);
        StartScale(scalePath[0], duration);
    }




    //overloading DetermineIndex for movement and scale
    int DetermineIndex(int currentIndex, List<Vector3> aList)
    {
        switch(transformPathType)
        {
            case TransformPathType.LINEAR:
                return (currentIndex + 1) % aList.Count;
            
            case TransformPathType.PINGPONG:
                return Mathf.RoundToInt(Mathf.PingPong(Time.time, aList.Count - 1));
        }
        
        return 0;
    }

    //overloading DetermineIndex for rotation
    int DetermineIndex(int currentIndex, List<float> aList)
    {
        switch(transformPathType)
        {
            case TransformPathType.LINEAR:
                return (currentIndex + 1) % aList.Count;
            
            case TransformPathType.PINGPONG:
                return Mathf.RoundToInt(Mathf.PingPong(Time.time, aList.Count - 1));
        }
        
        return 0;
    }
    

    #if UNITY_EDITOR

        void OnDrawGizmos()
        {
            string info = "";

            Gizmos.color = Color.green;
            Handles.color = Color.green;
            if(movePath.Count > 0)
            {
                for(int i = 0; i < movePath.Count; i++)
                {
                    Gizmos.DrawWireCube(movePath[i], Vector3.one);
                    Handles.Label(movePath[i], ""+i);
                }
            }

            Handles.color = Color.white;
            if(rotatePath.Count > 0)
            {
                info += "Rotate Path: ";
                for(int i = 0; i < rotatePath.Count; i++)
                {
                    if(i == rotatePath.Count - 1)
                    {
                        info += rotatePath[i] + "";
                    }
                    else
                    {
                        info += rotatePath[i] + ", ";
                    }
                }
            }

            if(scalePath.Count > 0)
            {
                info += "\nScale Path: ";
                foreach(var scale in scalePath)
                {
                    info += scale.ToString() + "\n\t      ";
                }
            }

            Handles.Label(transform.position + Vector3.up * transform.localScale.y / 2, info);

        }

    #endif

}


