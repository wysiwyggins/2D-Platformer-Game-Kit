using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecureSingleton<T> : MonoBehaviour where T : SecureSingleton<T>
{
    protected static T This { get; set; }

    protected virtual void Awake()
    {
        if (This == null)
        {
            This = (T)this;
            return;
        }

        Debug.LogError("[Manager] instance of " + typeof(T) + " already exists");
    }

    protected virtual void OnDestroy()
    {
        StopAllCoroutines();
        CancelInvoke();
        This = null;
    }
}