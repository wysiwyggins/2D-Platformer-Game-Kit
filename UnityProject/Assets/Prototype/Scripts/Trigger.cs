using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(AudioSource), typeof(Collider2D))]
public class Trigger : MonoBehaviour
{
    static List<Trigger> triggers = new List<Trigger>();

    public enum CameraZoom { NONE, ZOOM_IN, ZOOM_OUT, RESET_ZOOM }
    public enum Exposure { NONE, BRIGHTEN, DARKEN, RESET }
    public enum HueShift { NONE, UP, DOWN, RESET }
    public enum Saturation { NONE, DESATURATE, SUPERSATURATE, GRAYSCALE, RESET }
    public enum Vignette { NONE, ON, RESET }

    [Header("Settings")]
    [Tooltip("Enable to deactivate trigger after one use.")]
    public bool singleUse = false;
    public bool showLinesAndLabels = true;
    public bool hideInGame = true;

    [Header("Camera Control")]
    public CameraZoom cameraZoom;

    [Header("Post Processing")]
    public Exposure exposure;
    public HueShift hueShift;
    public Saturation saturation;
    public Vignette vignette;

    [Header("Change Object Size")]
    public List<ResizeObject> objectsToGrow;
    public List<ResizeObject> objectsToShrink;
    public List<ResizeObject> objectsToReset;

    [Header("Change Object Color")]
    public Color newColor = new Color(0, 0, 0, 1);
    public bool glow;
    public List<SpriteColor> objectsToColor;
    public List<SpriteColor> objectsToResetColor;

    [Header("Show or Hide Objects")]
    public List<GameObject> objectsToShow;
    public List<GameObject> objectsToHide;

    [Header("Trigger on Other Objects")]
    public List<GameObject> triggerObjects;

    [Header("Audio")]
    [Tooltip("Play a short sound effect.")]
    public AudioClip soundFX;

    [Tooltip("Play music or ambient soundscape. This will pause other triggered ambient audio.")]
    public AudioClip ambientSound;
    [Tooltip("Pause ambient sounds. Only affects other audio sources.")]
    public bool pauseOtherSounds;

    [Range(0f, 1f)]
    public float volume = 1;
    public bool loop = true;

    [Header("Game")]
    [Tooltip("End game when trigger is activated.")]
    public bool endGame;

    bool pauseAudio;
    AudioSource audioSource;
    WaitForSeconds wait;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        GetComponent<Collider2D>().isTrigger = true;

        // Register trigger
        if (!triggers.Contains(this))
        {
            triggers.Add(this);
        }

        if (hideInGame)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }

        wait = new WaitForSeconds(0.1f);
    }

    void Update()
    {
        if (pauseAudio)
        {
            audioSource.volume -= Time.unscaledDeltaTime; // Fade out

            if (audioSource.volume < Mathf.Epsilon)
            {
                audioSource.Pause();
                pauseAudio = false;
            }
        }
    }

    IEnumerator OnTriggerEnter2D(Collider2D collider)
    {
        bool go = false;
        var colliderObject = collider.gameObject;

        for (var i = 0; i < triggerObjects.Count; i++)
        {
            if (triggerObjects[i] == colliderObject)
            {
                go = true;
                break;
            }
        }

        if (colliderObject.CompareTag("Player") || go)
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
            //switch (cameraShake)
            //{
            //    case CameraShake.SHAKE:
            //        FindObjectOfType<CameraController>()?.Shake();
            //        break;

            //}

            // Post Processing
            switch (exposure)
            {
                case Exposure.BRIGHTEN:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.EXPOSURE, 1);
                    break;

                case Exposure.DARKEN:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.EXPOSURE, -1);
                    break;

                case Exposure.RESET:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.EXPOSURE, 0);
                    break;
            }

            switch (hueShift)
            {
                case HueShift.UP:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.HUE, 45);
                    break;

                case HueShift.DOWN:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.HUE, -45);
                    break;

                case HueShift.RESET:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.HUE, 0);
                    break;
            }

            switch (saturation)
            {
                case Saturation.DESATURATE:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.SATURATION, -50);
                    break;

                case Saturation.SUPERSATURATE:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.SATURATION, 50);
                    break;

                case Saturation.GRAYSCALE:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.SATURATION, -100);
                    break;

                case Saturation.RESET:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.SATURATION, 0);
                    break;
            }

            switch (vignette)
            {
                case Vignette.ON:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.VIGNETTE, 0.45f);
                    break;

                case Vignette.RESET:
                    FindObjectOfType<CameraController>()?.SetEffect(CameraController.Type.VIGNETTE, 0);
                    break;
            }

            // Grow Objects
            for (var i = 0; i < objectsToGrow.Count; i++)
            {
                if (objectsToGrow[i] != null)
                {
                    objectsToGrow[i].Grow();
                }
            }

            // Shrink Objects
            for (var i = 0; i < objectsToShrink.Count; i++)
            {
                if (objectsToShrink[i] != null)
                {
                    objectsToShrink[i].Shrink();
                }
            }

            // Reset Objects
            for (var i = 0; i < objectsToReset.Count; i++)
            {
                if (objectsToReset[i] != null)
                {
                    objectsToReset[i].ResetSize();
                }
            }

            // Show Objects
            for (var i = 0; i < objectsToShow.Count; i++)
            {
                if (objectsToShow[i] != null)
                {
                    objectsToShow[i].SetActive(true);
                }
            }

            // Hide Objects
            for (var i = 0; i < objectsToHide.Count; i++)
            {
                if (objectsToHide[i] != null)
                {
                    objectsToHide[i].SetActive(false);
                }
            }

            // Color Objects
            for (var i = 0; i < objectsToColor.Count; i++)
            {
                if (objectsToColor[i] != null)
                {
                    objectsToColor[i].SetColor(newColor, glow);
                }
            }

            // Reset Color
            for (var i = 0; i < objectsToResetColor.Count; i++)
            {
                if (objectsToResetColor[i] != null)
                {
                    objectsToResetColor[i].ResetColor();
                }
            }

            // Audio
            audioSource.volume = volume;
            audioSource.loop = loop;

            if (soundFX != null)
            {
                audioSource.PlayOneShot(soundFX);
            }

            if (ambientSound != null || pauseOtherSounds)
            {
                // Stop ambient audio on other triggers
                foreach (var t in triggers)
                {
                    if (t != null && t != this)
                    {
                        t.PauseAudio();
                    }
                }

                if (!audioSource.isPlaying)
                {
                    audioSource.clip = ambientSound;
                    audioSource.Play();
                }
            }

            // End Game
            if (endGame)
            {
                GameManager.GameOver();
            }

            // Single Use
            if (singleUse)
            {
                var c = GetComponent<Collider2D>();
                if (c != null)
                {
                    c.enabled = false;
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
        // Remove null elements
        //growObjects.RemoveAll(x => x == null);
        //shrinkObjects.RemoveAll(x => x == null);
        //resetObjects.RemoveAll(x => x == null);
        //showObjects.RemoveAll(x => x == null);
        //hideObjects.RemoveAll(x => x == null);
        //colorObjects.RemoveAll(x => x == null);
    }

    void OnDrawGizmos()
    {
        //var localScale = transform.localScale;
        //Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.5f);
        //Gizmos.DrawCube(transform.position, new Vector3(localScale.x, localScale.y, 0));

        if (showLinesAndLabels)
        {
            var position = transform.position;
            var scale = transform.localScale;

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
            foreach (var o in objectsToGrow)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position, position);
                    Handles.Label(Vector3.Lerp(o.transform.position, position, 0.55f), "Grow " + o.name);
                }
            }

            Gizmos.color = Color.gray;
            foreach (var o in objectsToShrink)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position, position);
                    Handles.Label(Vector3.Lerp(o.transform.position, position, 0.55f), "Shrink " + o.name);
                }
            }

            Gizmos.color = Color.black;
            foreach (var o in objectsToReset)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position, position);
                    Handles.Label(Vector3.Lerp(o.transform.position, position, 0.55f), "Reset Size " + o.name);
                }
            }

            Gizmos.color = Color.green;
            foreach (var o in objectsToShow)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position, position);
                    Handles.Label(Vector3.Lerp(o.transform.position, position, 0.55f), "Show " + o.name);
                }
            }

            Gizmos.color = Color.red;
            foreach (var o in objectsToHide)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position, position);
                    Handles.Label(Vector3.Lerp(o.transform.position, position, 0.55f), "Hide " + o.name);
                }
            }

            Gizmos.color = Color.cyan;
            foreach (var o in triggerObjects)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position, position);
                    Handles.Label(Vector3.Lerp(o.transform.position, position, 0.55f), "Trigger " + o.name);
                }
            }

            Gizmos.color = newColor;
            var offset = Vector3.right * 0.25f;
            foreach (var o in objectsToColor)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position + offset, position + offset);
                    Handles.Label(Vector3.Lerp(o.transform.position + offset, position + offset, 0.45f), "Color " + o.name);
                }
            }

            Gizmos.color = new Color(1, 0.5f, 0);
            foreach (var o in objectsToResetColor)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position + offset, position + offset);
                    Handles.Label(Vector3.Lerp(o.transform.position + offset, position + offset, 0.45f), "Reset Color " + o.name);
                }
            }
        }
    }
#endif

    public void PauseAudio()
    {
        if (audioSource.isPlaying)
        {
            pauseAudio = true;
        }
    }
}
