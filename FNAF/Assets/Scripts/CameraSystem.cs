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
    #region Serialized Fields

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

    [Header("Debug - Viewpoint Switching")]
    [SerializeField] private bool debugViewpointMode = false;
    [SerializeField] private Camera mainCamera;

    #endregion

    #region Private Fields

    private PowerSystem powerSystem;
    private Dictionary<CameraLocation, CameraData> cameraDict = new Dictionary<CameraLocation, CameraData>();

    // Debug viewpoint state
    private Vector3 originalMainCameraPosition;
    private Quaternion originalMainCameraRotation;
    private bool isViewingSecurityCamera = false;
    private int currentDebugCameraIndex = -1; // -1 means viewing main camera

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        powerSystem = FindFirstObjectByType<PowerSystem>();
        InitializeCameras();
        InitializeMainCamera();
    }

    private void Update()
    {
        HandleInput();
    }

    #endregion

    #region Initialization

    private void InitializeCameras()
    {
        cameraDict.Clear();
        foreach (var cam in cameras)
        {
            cameraDict[cam.location] = cam;
        }
    }

    private void InitializeMainCamera()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
        {
            originalMainCameraPosition = mainCamera.transform.position;
            originalMainCameraRotation = mainCamera.transform.rotation;
        }
    }

    #endregion

    #region Input Handling

    private void HandleInput()
    {
        if (Keyboard.current == null) return;

        HandleTabletInput();
        HandleCameraSwitching();
        HandleDebugViewpointInput();
    }

    private void HandleTabletInput()
    {
        // Toggle tablet (C key or mouse scroll up)
        if (Keyboard.current.cKey.wasPressedThisFrame ||
            (Mouse.current != null && Mouse.current.scroll.ReadValue().y > 0f))
        {
            ToggleTablet();
        }
    }

    private void HandleCameraSwitching()
    {
        if (!isTabletUp) return;

        // Number keys for direct camera selection
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

    private void HandleDebugViewpointInput()
    {
        if (!debugViewpointMode) return;

        // Number keys to switch viewpoint
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SwitchToCameraViewpoint(CameraLocation.CAM_1A);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SwitchToCameraViewpoint(CameraLocation.CAM_2A);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SwitchToCameraViewpoint(CameraLocation.CAM_3);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SwitchToCameraViewpoint(CameraLocation.CAM_4A);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SwitchToCameraViewpoint(CameraLocation.CAM_5);
        if (Keyboard.current.digit6Key.wasPressedThisFrame) SwitchToCameraViewpoint(CameraLocation.CAM_6);
        if (Keyboard.current.digit7Key.wasPressedThisFrame) SwitchToCameraViewpoint(CameraLocation.CAM_7);

        // Cycle through cameras
        if (Keyboard.current.nKey.wasPressedThisFrame) CycleToNextDebugCamera();

        // Return to main camera view
        if (Keyboard.current.digit0Key.wasPressedThisFrame) ReturnToMainCameraView();
    }

    #endregion

    #region Tablet Management

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
            if (cameras.Count > 0)
            {
                SwitchToCameraViewpoint(cameras[1].location);
            }
        }
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

    #endregion

    #region Camera Switching

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

    #endregion

    #region Static Detection

    private void CheckForStatic(CameraLocation location)
    {
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

        if (cameraDict.ContainsKey(location))
        {
            cameraDict[location].hasStatic = hasStatic;
        }
    }

    #endregion

    #region Public Getters

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

    public bool IsAnimatronicVisible(CameraLocation location, string animatronicName)
    {
        if (!cameraDict.ContainsKey(location))
            return false;

        AnimatronicBase[] animatronics = FindObjectsByType<AnimatronicBase>(FindObjectsSortMode.None);
        foreach (var animatronic in animatronics)
        {
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

    #endregion

    #region Camera Management

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

    #endregion

    #region Debug Viewpoint Switching

    /// <summary>
    /// Switches the main camera view to match a security camera (for debugging/viewing angles)
    /// </summary>
    public void SwitchToCameraViewpoint(CameraLocation location)
    {
        if (!cameraDict.ContainsKey(location))
        {
            Debug.LogWarning($"CameraSystem: Camera {location} not found!");
            return;
        }

        CameraData camData = cameraDict[location];
        if (camData.camera == null)
        {
            Debug.LogWarning($"CameraSystem: Camera GameObject for {location} is null!");
            return;
        }

        if (mainCamera == null)
        {
            Debug.LogWarning("CameraSystem: Main camera not found!");
            return;
        }

        // Store original view if we haven't already
        if (!isViewingSecurityCamera)
        {
            originalMainCameraPosition = mainCamera.transform.position;
            originalMainCameraRotation = mainCamera.transform.rotation;
        }

        // Restore previous camera's render texture if we were viewing one
        RestorePreviousCameraTexture();

        // Switch main camera to security camera's position and rotation
        mainCamera.transform.position = camData.camera.transform.position;
        mainCamera.transform.rotation = camData.camera.transform.rotation;
        mainCamera.fieldOfView = camData.camera.fieldOfView;

        // Temporarily disable the security camera's render texture so we see it directly
        camData.camera.targetTexture = null;
        camData.camera.enabled = true;

        // Update state
        currentDebugCameraIndex = GetCameraIndex(location);
        isViewingSecurityCamera = true;
        currentCamera = location;

        Debug.Log($"CameraSystem: Switched main camera view to {location} ({camData.displayName})");
    }

    /// <summary>
    /// Returns the main camera to its original viewpoint
    /// </summary>
    public void ReturnToMainCameraView()
    {
        if (mainCamera == null)
            return;

        // Restore previous camera's render texture
        RestorePreviousCameraTexture();

        // Restore original position and rotation
        mainCamera.transform.position = originalMainCameraPosition;
        mainCamera.transform.rotation = originalMainCameraRotation;

        // Restore all security cameras to render to their textures
        RestoreAllCameraTextures();

        // Reset state
        currentDebugCameraIndex = -1;
        isViewingSecurityCamera = false;
        currentCamera = CameraLocation.None;

        Debug.Log("CameraSystem: Returned to main camera view");
    }

    /// <summary>
    /// Cycles to the next camera in debug viewpoint mode (excludes main camera from cycle)
    /// </summary>
    private void CycleToNextDebugCamera()
    {
        if (cameras.Count == 0)
        {
            Debug.LogWarning("CameraSystem: No cameras available to cycle through!");
            return;
        }

        // If currently viewing main camera, start with first camera
        if (currentDebugCameraIndex < 0)
        {
            SwitchToCameraViewpoint(cameras[0].location);
            return;
        }

        // Move to next camera
        currentDebugCameraIndex++;

        // If we've reached the end, loop back to first camera (not main camera)
        if (currentDebugCameraIndex >= cameras.Count)
        {
            currentDebugCameraIndex = 0;
            SwitchToCameraViewpoint(cameras[0].location);
            Debug.Log("CameraSystem: Cycled back to first camera");
        }
        else
        {
            SwitchToCameraViewpoint(cameras[currentDebugCameraIndex].location);
        }
    }

    private void RestorePreviousCameraTexture()
    {
        if (isViewingSecurityCamera && currentCamera != CameraLocation.None && cameraDict.ContainsKey(currentCamera))
        {
            CameraData prevCam = cameraDict[currentCamera];
            if (prevCam.camera != null && prevCam.renderTexture != null)
            {
                prevCam.camera.targetTexture = prevCam.renderTexture;
            }
        }
    }

    private void RestoreAllCameraTextures()
    {
        foreach (var cam in cameras)
        {
            if (cam.camera != null && cam.renderTexture != null)
            {
                cam.camera.targetTexture = cam.renderTexture;
            }
        }
    }

    #endregion
}
