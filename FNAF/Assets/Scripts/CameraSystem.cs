using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public enum CameraLocation
{
    None,
    CAM_1A, // Show Stage
    CAM_1B, // Dining Area
    CAM_1C, // Pirate Cove
    CAM_2A, // West Hall
    CAM_2B, // West Hall Corner
    CAM_3,  // Supply Closet
    CAM_4A, // East Hall
    CAM_4B, // East Hall Corner
    CAM_5,  // Backstage
    CAM_6,  // Kitchen
    CAM_7   // Restrooms
}

[System.Serializable]
public class CameraData
{
    public CameraLocation location;
    public string displayName;
    public Camera camera;
    public RenderTexture renderTexture;
    public bool hasStatic;
}

public class CameraSystem : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private List<CameraData> cameras = new List<CameraData>();
    [SerializeField] private CameraLocation currentCamera = CameraLocation.None;
    [SerializeField] private bool isTabletUp = false;

    [Header("UI References")]
    [SerializeField] private UnityEngine.UI.RawImage cameraDisplay;
    [SerializeField] private GameObject tabletUI;

    [Header("Static Effect")]
    [SerializeField] private Material staticMaterial;
    [SerializeField] private float staticIntensity = 0.5f;

    private PowerSystem powerSystem;
    private Dictionary<CameraLocation, CameraData> cameraDict = new Dictionary<CameraLocation, CameraData>();

    private void Start()
    {
        powerSystem = FindFirstObjectByType<PowerSystem>();
        InitializeCameras();
    }

    private void Update()
    {
        HandleInput();
    }

    private void InitializeCameras()
    {
        cameraDict.Clear();
        foreach (var cam in cameras)
        {
            cameraDict[cam.location] = cam;
        }
    }

    private void HandleInput()
    {
        // Toggle tablet (C key or mouse scroll)
        if (Keyboard.current != null && (Keyboard.current.cKey.wasPressedThisFrame ||
            (Mouse.current != null && Mouse.current.scroll.ReadValue().y > 0f)))
        {
            ToggleTablet();
        }

        // Camera switching (number keys 1-7 or arrow keys)
        if (isTabletUp && Keyboard.current != null)
        {
            // Number keys for cameras
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SwitchCamera(CameraLocation.CAM_1A);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) SwitchCamera(CameraLocation.CAM_2A);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) SwitchCamera(CameraLocation.CAM_3);
            if (Keyboard.current.digit4Key.wasPressedThisFrame) SwitchCamera(CameraLocation.CAM_4A);
            if (Keyboard.current.digit5Key.wasPressedThisFrame) SwitchCamera(CameraLocation.CAM_5);
            if (Keyboard.current.digit6Key.wasPressedThisFrame) SwitchCamera(CameraLocation.CAM_6);
            if (Keyboard.current.digit7Key.wasPressedThisFrame) SwitchCamera(CameraLocation.CAM_7);

            // Arrow keys for navigation
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame) SwitchToPreviousCamera();
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame) SwitchToNextCamera();
        }
    }

    public void ToggleTablet()
    {
        isTabletUp = !isTabletUp;

        if (tabletUI != null)
            tabletUI.SetActive(isTabletUp);

        if (!isTabletUp)
        {
            currentCamera = CameraLocation.None;
            if (cameraDisplay != null)
                cameraDisplay.texture = null;
        }
        else
        {
            // Default to first camera
            if (currentCamera == CameraLocation.None && cameras.Count > 0)
            {
                SwitchCamera(cameras[0].location);
            }
        }
    }

    public void SwitchCamera(CameraLocation location)
    {
        if (!isTabletUp || !cameraDict.ContainsKey(location))
            return;

        currentCamera = location;
        CameraData camData = cameraDict[location];

        if (cameraDisplay != null && camData.renderTexture != null)
        {
            cameraDisplay.texture = camData.renderTexture;
        }

        // Check for static (animatronics nearby)
        CheckForStatic(location);
    }

    private void SwitchToNextCamera()
    {
        int currentIndex = GetCameraIndex(currentCamera);
        if (currentIndex >= 0 && currentIndex < cameras.Count - 1)
        {
            SwitchCamera(cameras[currentIndex + 1].location);
        }
    }

    private void SwitchToPreviousCamera()
    {
        int currentIndex = GetCameraIndex(currentCamera);
        if (currentIndex > 0)
        {
            SwitchCamera(cameras[currentIndex - 1].location);
        }
    }

    private int GetCameraIndex(CameraLocation location)
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            if (cameras[i].location == location)
                return i;
        }
        return -1;
    }

    private void CheckForStatic(CameraLocation location)
    {
        // Check if any animatronics are near this camera location
        AnimatronicBase[] animatronics = FindObjectsByType<AnimatronicBase>(FindObjectsSortMode.None);
        bool hasStatic = false;

        foreach (var animatronic in animatronics)
        {
            if (animatronic.IsNearCameraLocation(location))
            {
                hasStatic = true;
                break;
            }
        }

        // Apply static effect
        if (cameraDict.ContainsKey(location))
        {
            cameraDict[location].hasStatic = hasStatic;
        }
    }

    public bool IsCameraActive()
    {
        return isTabletUp && currentCamera != CameraLocation.None;
    }

    public CameraLocation GetCurrentCamera()
    {
        return currentCamera;
    }

    public bool IsTabletUp()
    {
        return isTabletUp;
    }

    public void ResetCameras()
    {
        isTabletUp = false;
        currentCamera = CameraLocation.None;
        if (tabletUI != null)
            tabletUI.SetActive(false);
        if (cameraDisplay != null)
            cameraDisplay.texture = null;
    }

    public bool IsAnimatronicVisible(CameraLocation location, string animatronicName)
    {
        if (!cameraDict.ContainsKey(location))
            return false;

        AnimatronicBase[] animatronics = FindObjectsByType<AnimatronicBase>(FindObjectsSortMode.None);
        foreach (var animatronic in animatronics)
        {
            // Check if animatronic name matches (if specified) and if it's near the camera location
            if (string.IsNullOrEmpty(animatronicName) || animatronic.name.Contains(animatronicName))
            {
                if (animatronic.IsNearCameraLocation(location))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void AddCamera(CameraLocation location, string displayName, Camera camera, RenderTexture renderTexture)
    {
        CameraData newCam = new CameraData
        {
            location = location,
            displayName = displayName,
            camera = camera,
            renderTexture = renderTexture,
            hasStatic = false
        };
        cameras.Add(newCam);
        cameraDict[location] = newCam;
    }
}

