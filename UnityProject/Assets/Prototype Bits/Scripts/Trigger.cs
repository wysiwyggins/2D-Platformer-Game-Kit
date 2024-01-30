using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(AudioSource), typeof(Collider2D))]
public class Trigger : MonoBehaviour
{
    [HideInInspector]public static List<Trigger> triggers = new List<Trigger>();
    [HideInInspector]public static List<Trigger> triggerCheckpoints = new List<Trigger>();

    public enum CameraZoom { NONE, ZOOM_IN, ZOOM_OUT, RESET_ZOOM }
    public enum Exposure { NONE, BRIGHTEN, DARKEN, RESET }
    public enum HueShift { NONE, UP, DOWN, RESET }
    public enum Saturation { NONE, DESATURATE, SUPERSATURATE, GRAYSCALE, RESET }
    public enum Vignette { NONE, ON, RESET }

    [Header("Trigger Type Setting (mutually exclusive)")]
    [Tooltip("If toggled, resets the player's position when activated")]
    public bool isDeathTrigger = false;
    [Tooltip("If toggled, sets the trigger to the player's last checkpoint when activated")]
    public bool isCheckpoint = false;

    [Header("Settings")]
    [Tooltip("Enable Single Use to deactivate trigger after one use.")]
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

    [Header("Transform Objects")]
    [Tooltip("Setting this to 0 will immediately transform the object")]
    public float transformDuration;
    public List<TransformObject> objectsToMove;
    [Tooltip("Objects in Objects to Move list will move to this position")]
    public Vector3 moveOffset;

    public List<TransformObject> objectsToRotate;
    [Tooltip("Objects in Objects to Rotate list will rotate to this angle")]
    public float rotateOffset;

    public List<TransformObject> objectsToScale;
    [Tooltip("Objects in Objects to Scale list will scale to this size")]
    public float scaleOffset;

    [Tooltip("Reset all transforms (Move, Rotate, Scale) back to their original position.")]
    public List<TransformObject> objectsToReset;



    [Header("Change Object Color")]
    public Color newColor = new Color(0, 0, 0, 1);
    public bool glow;
    public List<SpriteColor> objectsToColor;
    public List<SpriteColor> objectsToResetColor;

    [Header("Show or Hide Objects")]
    public List<GameObject> objectsToShow;
    public List<GameObject> objectsToHide;

    [Header("Enable or Disable Physics")]
    public List<Rigidbody2D> objectsToEnablePhysics;
    public List<Rigidbody2D> objectsToDisablePhysics;

    [Header("Activate Trigger with Objects")]
    public bool ignorePlayer;
    public List<GameObject> objectsToActivateTrigger;

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

        // Register trigger as a checkpoint if set
        if (isCheckpoint && !triggerCheckpoints.Contains(this))
        {
            triggerCheckpoints.Add(this);
        }

        if (hideInGame)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }

        //ensures the trigger cannot be a checkpoint and a death trigger at the same time
        if (isCheckpoint)
        {
            isDeathTrigger = false;
        }

        if(isDeathTrigger)
        {
            isCheckpoint = false;
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

        for (var i = 0; i < objectsToActivateTrigger.Count; i++)
        {
            if (objectsToActivateTrigger[i] == colliderObject)
            {
                go = true;
                break;
            }
        }

        if ((colliderObject.CompareTag("Player") && !ignorePlayer) || go)
        {

            if (isCheckpoint) //updates player's last position if checkpoint is crossed
            {
                GameManager.SetCheckpoint(triggerCheckpoints.IndexOf(this));
            }

            if(isDeathTrigger)
            {
                GameManager.ResetLevel();
            }


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

            // Move objects
            for (var i = 0; i < objectsToMove.Count; i++)
            {
                if (objectsToMove[i] != null)
                {
                    objectsToMove[i].StartMove(objectsToMove[i].transform.position + moveOffset, transformDuration);
                }
            }

            // Rotate Objects
            for (var i = 0; i < objectsToRotate.Count; i++)
            {
                if (objectsToRotate[i] != null)
                {
                    objectsToRotate[i].StartRotate(objectsToRotate[i].transform.rotation.eulerAngles.z + rotateOffset, transformDuration);
                }
            }

            //Scale Objects
            for (var i = 0; i < objectsToScale.Count; i++)
            {
                if (objectsToScale[i] != null)
                {
                    objectsToScale[i].StartScale(objectsToScale[i].transform.localScale * scaleOffset, transformDuration);
                }
            }

            // Reset Objects
            for (var i = 0; i < objectsToReset.Count; i++)
            {
                if (objectsToReset[i] != null)
                {
                    objectsToReset[i].Reset(transformDuration);
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

            // Physics
            for (var i = 0; i < objectsToEnablePhysics.Count; i++)
            {
                if (objectsToEnablePhysics[i] != null)
                {
                    objectsToEnablePhysics[i].isKinematic = false;
                }
            }

            for (var i = 0; i < objectsToDisablePhysics.Count; i++)
            {
                if (objectsToDisablePhysics[i] != null)
                {
                    objectsToDisablePhysics[i].isKinematic = true;
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

            Gizmos.color = Color.green;
            foreach (var o in objectsToMove)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position, position);
                    Handles.Label(Vector3.Lerp(o.transform.position, position, 0.5f), "Move: " + o.name);

                    Gizmos.DrawWireCube(o.transform.position + moveOffset, Vector3.one);
                }
            }

            Gizmos.color = Color.cyan;
            foreach (var o in objectsToRotate)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position, position);
                    Handles.Label(Vector3.Lerp(o.transform.position, position, 0.5f), "Rotate: " + o.name);
                }
            }

            Gizmos.color = Color.red;
            foreach (var o in objectsToReset)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position, position);
                    Handles.Label(Vector3.Lerp(o.transform.position, position, 0.5f), "Reset Size: " + o.name);
                }
            }

            Gizmos.color = Color.white;
            foreach (var o in objectsToShow)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position, position);
                    Handles.Label(Vector3.Lerp(o.transform.position, position, 0.5f), "Show: " + o.name);
                }
            }

            Gizmos.color = Color.magenta;
            foreach (var o in objectsToHide)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position, position);
                    Handles.Label(Vector3.Lerp(o.transform.position, position, 0.5f), "Hide: " + o.name);
                }
            }

            Gizmos.color = Color.yellow;
            foreach (var o in objectsToEnablePhysics)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position, position);
                    Handles.Label(Vector3.Lerp(o.transform.position, position, 0.5f), "Enable Physics: " + o.name);
                }
            }

            Gizmos.color = Color.blue;
            foreach (var o in objectsToDisablePhysics)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position, position);
                    Handles.Label(Vector3.Lerp(o.transform.position, position, 0.5f), "Disable Physics: " + o.name);
                }
            }


            Gizmos.color = Color.grey;
            foreach (var o in objectsToActivateTrigger)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position, position);
                    Handles.Label(Vector3.Lerp(o.transform.position, position, 0.5f), "Trigger: " + o.name);
                }
            }

            Gizmos.color = newColor;
            var offset = Vector3.right * 0.25f;
            foreach (var o in objectsToColor)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position + offset, position + offset);
                    Handles.Label(Vector3.Lerp(o.transform.position + offset, position + offset, 0.4f), "Color: " + o.name);
                }
            }

            Gizmos.color = new Color(1, 0.5f, 0);
            foreach (var o in objectsToResetColor)
            {
                if (o != null)
                {
                    Gizmos.DrawLine(o.transform.position + offset, position + offset);
                    Handles.Label(Vector3.Lerp(o.transform.position + offset, position + offset, 0.4f), "Reset Color: " + o.name);
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
