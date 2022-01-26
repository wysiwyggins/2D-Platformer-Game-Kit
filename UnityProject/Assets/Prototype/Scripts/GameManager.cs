using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SecureSingleton<GameManager>
{
    public List<GameObject> enableOnStart;
    public List<GameObject> disableOnStart;

    protected override void Awake()
    {
        base.Awake();
foreach (var o in enableOnStart)
        {
            if (o != null)
            {
                o.SetActive(true);
            }
        }

        foreach (var o in disableOnStart)
        {
            if (o != null)
            {
                o.SetActive(false);
            }
        }

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    void Start()
    {
        

    }

    void OnValidate()
    {
        enableOnStart.RemoveAll(x => x == null);
        disableOnStart.RemoveAll(x => x == null);
    }

    void OnDrawGizmos()
    {
        // Draw death box at -50 on y axis
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(new Vector3(0, -300, 0), new Vector3(5000, 500, 0));
    }
}