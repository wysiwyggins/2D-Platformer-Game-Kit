using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Trigger : MonoBehaviour
{
    [System.Serializable]
    public class ReColorObject
    {
        public Renderer renderer;
        public Color newColor;
        public bool glow;
    }

    static List<Trigger> triggers = new List<Trigger>();

    public enum CameraZoom { NONE, ZOOM_IN, ZOOM_OUT, RESET }

    [Header("Configuration")]
    [Tooltip("Enable to deactivate trigger after one use.")]
    public bool singleUse = false;
    public bool showConnections = true;

    [Header("Camera FX")]
    public CameraZoom cameraZoom;

    [Header("Change Object Size")]
    public List<ResizeObject> growObjects;
    public List<ResizeObject> shrinkObjects;
    public List<ResizeObject> resetObjects;

    [Header("Change Object Color")]
    public List<SpriteColor> colorObjects;
    public Color newColor = new Color(0, 0, 0, 1);
    public bool glow;

    [Header("Audio")]
    [Tooltip("Play a short sound effect.")]
    public AudioClip soundFX;

    [Tooltip("Play music or ambient soundscape. This will cancel other triggered audio.")]
    public AudioClip ambientSound;

    [Space(5)]
    [Range(0f, 1f)]
    public float volume = 1;
    public bool loop;

    bool stopAudio;
    AudioSource audioSource;
    WaitForSeconds wait;

    //MaterialPropertyBlock block;
    //public Color baseColor;

    void Awake()
    {
        // Register object in class
        if (!triggers.Contains(this))
        {
            triggers.Add(this);
        }

        audioSource = GetComponent<AudioSource>();
        GetComponent<SpriteRenderer>().enabled = false;

        wait = new WaitForSeconds(0.1f);
    }

    void Update()
    {
        if (stopAudio)
        {
            audioSource.volume -= Time.deltaTime; // Fade out

            if (audioSource.volume < Mathf.Epsilon)
            {
                audioSource.Stop();
                stopAudio = false;
            }
        }
    }

    IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        yield return wait;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Audio
            audioSource.volume = volume;
            audioSource.loop = loop;

            if (soundFX != null)
            {
                audioSource.PlayOneShot(soundFX);
            }

            if (ambientSound != null)
            {
                // Stop ambient audio on other triggers
                foreach (var t in triggers)
                {
                    if (t != null && t.enabled && t != this)
                    {
                        t.StopAudio();
                    }
                }

                if (!audioSource.isPlaying)
                {
                    audioSource.clip = ambientSound;
                    audioSource.Play();
                }
            }


            // Camera
            switch (cameraZoom)
            {
                case CameraZoom.NONE:
                    break;

                case CameraZoom.ZOOM_IN:
                    FindObjectOfType<FollowCam>()?.ZoomIn();
                    break;

                case CameraZoom.ZOOM_OUT:
                    FindObjectOfType<FollowCam>()?.ZoomOut();
                    break;

                case CameraZoom.RESET:
                    FindObjectOfType<FollowCam>()?.ZoomReset();
                    break;
            }

            // Grow Objects
            for (var i = 0; i < growObjects.Count; i++)
            {
                if (growObjects[i] != null)
                {
                    growObjects[i]?.Grow();
                }
            }

            // Shrink Objects
            for (var i = 0; i < shrinkObjects.Count; i++)
            {
                if (shrinkObjects[i] != null)
                {
                    shrinkObjects[i]?.Shrink();
                }
            }

            // Shrink Objects
            for (var i = 0; i < resetObjects.Count; i++)
            {
                if (resetObjects[i] != null)
                {
                    resetObjects[i]?.ResetSize();
                }
            }

            // Color Objects
            for (var i = 0; i < colorObjects.Count; i++)
            {
                if (colorObjects[i] != null)
                {
                    colorObjects[i].SetColor(newColor, glow);
                }
            }

            // Single Use
            if (singleUse)
            {
                var collider = GetComponent<Collider2D>();
                print(collider);
                if (collider != null)
                {
                    collider.enabled = false;
                }
            }
        }
    }

    void OnDestroy()
    {
        triggers.Remove(this);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        growObjects.RemoveAll(x => x == null);
        shrinkObjects.RemoveAll(x => x == null);
        resetObjects.RemoveAll(x => x == null);
        colorObjects.RemoveAll(x => x == null);
    }

    void OnDrawGizmos()
    {
        var localScale = transform.localScale;
        Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(localScale.x, localScale.y, 0));

        if (showConnections)
        {
            var position = transform.position;

            Gizmos.color = Color.green;
            foreach (var o in growObjects)
            {
                Gizmos.DrawLine(o.transform.position, position);
                Handles.Label(Vector3.Lerp(o.transform.position, position, 0.55f), "Grow");
            }

            Gizmos.color = Color.cyan;
            foreach (var o in shrinkObjects)
            {
                Gizmos.DrawLine(o.transform.position, position);
                Handles.Label(Vector3.Lerp(o.transform.position, position, 0.55f), "Shrink");
            }

            Gizmos.color = Color.blue;
            foreach (var o in resetObjects)
            {
                Gizmos.DrawLine(o.transform.position, position);
                Handles.Label(Vector3.Lerp(o.transform.position, position, 0.55f), "Reset Size");
            }

            Gizmos.color = newColor;
            var offset = Vector3.right * 0.25f;
            foreach (var o in colorObjects)
            {
                Gizmos.DrawLine(o.transform.position + offset, position + offset);
                Handles.Label(Vector3.Lerp(o.transform.position + offset, position + offset, 0.45f), "Color");
            }
        }
    }
#endif

    public void StopAudio()
    {
        if (audioSource.isPlaying)
        {
            stopAudio = true;
        }
    }
}
