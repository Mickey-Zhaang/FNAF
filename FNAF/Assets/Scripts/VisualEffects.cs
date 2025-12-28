using UnityEngine;
using System.Collections;

public class VisualEffects : MonoBehaviour
{
    [Header("Camera Static")]
    [SerializeField] private Material staticMaterial;
    [SerializeField] private float staticNoiseSpeed = 10f;
    [SerializeField] private float staticIntensity = 0.5f;

    [Header("Distortion")]
    [SerializeField] private Material distortionMaterial;
    [SerializeField] private float distortionAmount = 0.1f;

    [Header("Light Flicker")]
    [SerializeField] private Light[] flickerLights;
    [SerializeField] private float flickerMinIntensity = 0.5f;
    [SerializeField] private float flickerMaxIntensity = 1f;
    [SerializeField] private float flickerSpeed = 5f;

    [Header("Screen Effects")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Material screenEffectMaterial;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    public void ApplyCameraStatic(RenderTexture source, RenderTexture destination, CameraLocation location)
    {
        if (staticMaterial != null)
        {
            // Check if animatronic is near this camera
            bool hasStatic = CheckForAnimatronicStatic(location);
            
            if (hasStatic)
            {
                staticMaterial.SetFloat("_NoiseSpeed", staticNoiseSpeed);
                staticMaterial.SetFloat("_Intensity", staticIntensity);
                Graphics.Blit(source, destination, staticMaterial);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    private bool CheckForAnimatronicStatic(CameraLocation location)
    {
        AnimatronicBase[] animatronics = FindObjectsOfType<AnimatronicBase>();
        foreach (var animatronic in animatronics)
        {
            if (animatronic.IsNearCameraLocation(location))
            {
                return true;
            }
        }
        return false;
    }

    public void StartLightFlicker(Light light, float duration = -1f)
    {
        StartCoroutine(FlickerLight(light, duration));
    }

    private IEnumerator FlickerLight(Light light, float duration)
    {
        float elapsed = 0f;
        float originalIntensity = light.intensity;

        while (duration < 0f || elapsed < duration)
        {
            light.intensity = Random.Range(flickerMinIntensity, flickerMaxIntensity) * originalIntensity;
            yield return new WaitForSeconds(1f / flickerSpeed);
            elapsed += 1f / flickerSpeed;
        }

        light.intensity = originalIntensity;
    }

    public void ApplyScreenDistortion(float intensity)
    {
        if (screenEffectMaterial != null && mainCamera != null)
        {
            screenEffectMaterial.SetFloat("_DistortionAmount", intensity * distortionAmount);
        }
    }

    public void StartScreenShake(float intensity, float duration)
    {
        StartCoroutine(ScreenShakeCoroutine(intensity, duration));
    }

    private IEnumerator ScreenShakeCoroutine(float intensity, float duration)
    {
        Vector3 originalPosition = mainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            mainCamera.transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPosition;
    }

    public void ApplyPowerOutageEffect()
    {
        // Darken screen when power is out
        if (mainCamera != null)
        {
            // Could use post-processing or overlay
            StartCoroutine(PowerOutageCoroutine());
        }
    }

    private IEnumerator PowerOutageCoroutine()
    {
        // Gradually darken screen
        float elapsed = 0f;
        float duration = 2f;

        while (elapsed < duration)
        {
            // Apply darkening effect (would need post-processing or overlay)
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}

