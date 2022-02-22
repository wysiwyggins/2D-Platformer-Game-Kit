using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Trigger : MonoBehaviour
{
    static List<Trigger> triggers = new List<Trigger>();

    public enum CameraZoom { NONE, ZOOM_IN, ZOOM_OUT, RESET_ZOOM }
    public enum CameraShake { NONE, SHAKE }
    public enum PostProcessingEffect { NONE, BRIGHTEN, DARKEN, HUESHIFT_UP, HUESHIFT_DOWN, DESATURATE, SUPERSATURATE, GRAYSCALE, VIGNETTE, RESET }

    [Header("Settings")]
    [Tooltip("Enable to deactivate trigger after one use.")]
    public bool singleUse = false;
    public bool showLinesAndLabels = true;
    public bool hideInGame = true;

    [Header("Camera Control")]
    public CameraZoom cameraZoom;
    public CameraShake cameraShake;

    [Header("Post Processing")]
    public PostProcessingEffect effect;

    [Header("Change Object Size")]
    public List<ResizeObject> growObjects;
    public List<ResizeObject> shrinkObjects;
    public List<ResizeObject> resetObjects;

    [Header("Change Object Color")]
    public Color newColor = new Color(0, 0, 0, 1);
    public bool glow;
    public List<SpriteColor> colorObjects;

    [Header("Show or Hide Objects")]
    public List<GameObject> showObjects;
    public List<GameObject> hideObjects;

    [Header("Audio")]
    [Tooltip("Play a short sound effect.")]
    public AudioClip soundFX;

    [Tooltip("Play music or ambient soundscape. This will cancel other triggered audio.")]
    public AudioClip ambientSound;
    [Range(0f, 1f)]
    public float volume = 1;
    public bool loop = true;

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

        if (hideInGame)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }

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
        if (collision.gameObject.CompareTag("Player"))
        {
            // Camera Zoom
            switch (cameraZoom)
            {
                case CameraZoom.NONE:
                    break;

                case CameraZoom.ZOOM_IN:
                    FindObjectOfType<CameraController>()?.ZoomIn();
                    break;

                case CameraZoom.ZOOM_OUT:
                    FindObjectOfType<CameraController>()?.ZoomOut();
                    break;

                case CameraZoom.RESET_ZOOM:
                    FindObjectOfType<CameraController>()?.ZoomReset();
                    break;
            }

            // Camera Shake
            switch (cameraShake)
            {
                case CameraShake.SHAKE:
                    FindObjectOfType<CameraController>()?.Shake();
                    break;

            }

            // Post Processing
            switch (effect)
            {
                case PostProcessingEffect.BRIGHTEN:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.EXPOSURE, 1);
                    break;

                case PostProcessingEffect.DARKEN:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.EXPOSURE, -1);
                    break;

                case PostProcessingEffect.HUESHIFT_UP:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.HUE, 45);
                    break;

                case PostProcessingEffect.HUESHIFT_DOWN:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.HUE, -45);
                    break;

                case PostProcessingEffect.DESATURATE:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.SATURATION, -50);
                    break;

                case PostProcessingEffect.SUPERSATURATE:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.SATURATION, 50);
                    break;

                case PostProcessingEffect.GRAYSCALE:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.SATURATION, -100);
                    break;

                case PostProcessingEffect.VIGNETTE:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.VIGNETTE, 0.45f);
                    break;

                case PostProcessingEffect.RESET:
                    FindObjectOfType<CameraController>()?.ResetAllEffects();
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

            // Show Objects
            for (var i = 0; i < showObjects.Count; i++)
            {
                if (showObjects[i] != null)
                {
                    showObjects[i].SetActive(true);
                }
            }

            // Hide Objects
            for (var i = 0; i < hideObjects.Count; i++)
            {
                if (hideObjects[i] != null)
                {
                    hideObjects[i].SetActive(false);
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
                    if (t != null && t != this)
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

            // Single Use
            if (singleUse)
            {
                var collider = GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = false;
                }
            }
        }

        yield return wait;
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
        showObjects.RemoveAll(x => x == null);
        hideObjects.RemoveAll(x => x == null);
        colorObjects.RemoveAll(x => x == null);
    }

    void OnDrawGizmos()
    {
        //var localScale = transform.localScale;
        //Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.5f);
        //Gizmos.DrawCube(transform.position, new Vector3(localScale.x, localScale.y, 0));

        if (showLinesAndLabels)
        {
            var position = transform.position;

            string info = "Trigger";
            info += "\nSingle Use: " + singleUse;
            info += "\nHide In Game: " + hideInGame;

            if (cameraZoom != CameraZoom.NONE)
            {
                info += "\nCamera Zoom: " + cameraZoom.ToString();
            }

            if (soundFX != null)
            {
                info += "\nSound FX: " + soundFX.name;
            }

            if (ambientSound != null)
            {
                info += "\nAmbient Sound: " + ambientSound.name;
            }

            Handles.Label(position + Vector3.up * transform.localScale.y / 2, info);

            Gizmos.color = Color.white;
            foreach (var o in growObjects)
            {
                Gizmos.DrawLine(o.transform.position, position);
                Handles.Label(Vector3.Lerp(o.transform.position, position, 0.55f), "Grow");
            }

            Gizmos.color = Color.gray;
            foreach (var o in shrinkObjects)
            {
                Gizmos.DrawLine(o.transform.position, position);
                Handles.Label(Vector3.Lerp(o.transform.position, position, 0.55f), "Shrink");
            }

            Gizmos.color = Color.black;
            foreach (var o in resetObjects)
            {
                Gizmos.DrawLine(o.transform.position, position);
                Handles.Label(Vector3.Lerp(o.transform.position, position, 0.55f), "Reset Size");
            }

            Gizmos.color = Color.green;
            foreach (var o in showObjects)
            {
                Gizmos.DrawLine(o.transform.position, position);
                Handles.Label(Vector3.Lerp(o.transform.position, position, 0.55f), "Show");
            }

            Gizmos.color = Color.red;
            foreach (var o in hideObjects)
            {
                Gizmos.DrawLine(o.transform.position, position);
                Handles.Label(Vector3.Lerp(o.transform.position, position, 0.55f), "Hide");
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
