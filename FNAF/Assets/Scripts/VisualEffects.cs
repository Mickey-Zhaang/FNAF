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
}

