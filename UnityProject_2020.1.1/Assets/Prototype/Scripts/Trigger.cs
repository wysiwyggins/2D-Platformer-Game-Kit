using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    static List<Trigger> triggers = new List<Trigger>();

    public enum CameraZoom { NONE, ZOOM_IN, ZOOM_OUT, UNZOOM }

    [Header("Configuration")]
    [Tooltip("Enable to deactivate trigger after one use.")]
    public bool singleUse;

    [Header("Camera FX")]
    public CameraZoom cameraZoom;

    [Header("Grow or Shrink Objects")]
    public List<Transform> growObjects;
    public List<Transform> shrinkObjects;
    public List<Transform> resetObjects;

    [Header("Audio")]
    [Tooltip("Play a short sound effect.")]
    public AudioClip fxClip;

    [Tooltip("Play music or ambient soundscape. This will cancel other triggered audio.")]
    public AudioClip ambientClip;

    [Space(5)]
    [Range(0f, 1f)]
    public float volume = 1;
    public bool loop;

    bool stopAudio;
    AudioSource audioSource;
    WaitForSeconds wait;

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

            if (fxClip != null)
            {
                audioSource.PlayOneShot(fxClip);
            }

            if (ambientClip != null)
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
                    audioSource.clip = ambientClip;
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

                case CameraZoom.UNZOOM:
                    FindObjectOfType<FollowCam>()?.UnZoom();
                    break;
            }

            // Grow Objects
            for (var i = 0; i < growObjects.Count; i++)
            {
                if (growObjects[i] != null)
                {
                    growObjects[i].GetComponent<GrowShrink>()?.Grow();
                }
            }

            // Shrink Objects
            for (var i = 0; i < shrinkObjects.Count; i++)
            {
                if (shrinkObjects[i] != null)
                {
                    shrinkObjects[i].GetComponent<GrowShrink>()?.Shrink();
                }
            }

            // Shrink Objects
            for (var i = 0; i < resetObjects.Count; i++)
            {
                if (resetObjects[i] != null)
                {
                    resetObjects[i].GetComponent<GrowShrink>()?.ResetSize();
                }
            }

            // Single Use
            if (singleUse)
            {
                var collider = GetComponent<Collider>();
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

    void OnValidate()
    {
        growObjects.RemoveAll(x => x == null);
        shrinkObjects.RemoveAll(x => x == null);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.5f);
        var localScale = transform.localScale;
        Gizmos.DrawCube(transform.position, new Vector3(localScale.x, localScale.y, 0));
    }

    public void StopAudio()
    {
        if (audioSource.isPlaying)
        {
            stopAudio = true;
        }
    }
}
