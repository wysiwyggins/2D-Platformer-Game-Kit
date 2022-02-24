using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FollowCam : MonoBehaviour
{
    public enum Type { EXPOSURE, HUE, SATURATION, VIGNETTE }

    public float speed = 1;

    Transform player;
    new Camera camera;
    ColorAdjustments colorAdjustments;
    Vignette vignette;
    Vector3 offset, shakeVector;
    float targetExposure = 0;
    float targetHue = 0;
    float targetSaturation = 0;
    float targetVignetteIntensity = 0;
    float lerp, origSize, targetSize, velocity, t, shakeMultiplier;

    void Start()
    {
        var volume = GetComponent<Volume>();
        if (volume != null && volume.isGlobal)
        {
            GetComponent<Volume>().profile.TryGet(out colorAdjustments);
            GetComponent<Volume>().profile.TryGet(out vignette);
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
        {
            transform.parent = null;
            offset = transform.position - player.position;
            camera = GetComponent<Camera>();
            origSize = camera.orthographicSize;
            targetSize = origSize;
        }
    }

    void LateUpdate()
    {
        var deltaTime = Time.deltaTime;

        // Camera Motion
        if (player != null)
        {
            // Compute offset and match camera x position to target x position
            var targetPosition = player.position + offset;
            targetPosition.x = player.position.x;

            // Speed up camera if target quickly moves away from center
            var s = speed;
            if (Vector3.Distance(targetPosition, transform.position) > 7)
            {
                s = speed * 5;
            }

            // Adjust lerp speed
            lerp = Mathf.Lerp(lerp, s, Time.deltaTime * 5);
            transform.position = Vector3.Lerp(transform.position, targetPosition, deltaTime * lerp);
            camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, targetSize, ref velocity, 0.5f);

            // Camera Shake
            shakeVector = Vector3.Lerp(shakeVector, Random.onUnitSphere.normalized, deltaTime * 30);
            shakeMultiplier = Mathf.Clamp01(shakeMultiplier - deltaTime / 2);
            transform.position += shakeVector * shakeMultiplier;
        }

        // Post processing
        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = Mathf.Lerp(colorAdjustments.saturation.value, targetSaturation, t);
            colorAdjustments.postExposure.value = Mathf.Lerp(colorAdjustments.postExposure.value, targetExposure, t);
            colorAdjustments.hueShift.value = Mathf.Lerp(colorAdjustments.hueShift.value, targetHue, t);
        }

        if (vignette != null)
        {
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, targetVignetteIntensity, t);
        }

        t = Mathf.Clamp01(t + deltaTime);
    }

    public void ZoomIn()
    {
        targetSize = origSize / 2;
    }

    public void ZoomOut()
    {
        targetSize = origSize * 2;
    }

    public void ZoomReset()
    {
        targetSize = origSize;
    }

    public void Shake()
    {
        shakeMultiplier = 0.25f;
    }

    public void SetEffect(Type type, float value)
    {
        switch (type)
        {
            case Type.EXPOSURE:
                targetExposure = value;
                break;

            case Type.HUE:
                targetHue = value;
                break;

            case Type.SATURATION:
                targetSaturation = value;
                break;

            case Type.VIGNETTE:
                targetVignetteIntensity = value;
                break;
        }

        t = 0;
    }
}
