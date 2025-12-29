using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public enum CameraLocation
{
    None,
    CAM_1A,
    CAM_1B,
    CAM_2A,
    CAM_2B,
    CAM_3A,
    CAM_3C,
    CAM_4A,
    CAM_4B,
    CAM_4C,
    CAM_5A,
    CAM_5B,
    CAM_5C,
    CAM_6
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
    //==========================================
    //
    [Header("Camera Settings")]
    // 
    //==========================================
    [SerializeField] private List<CameraData> cameras = new List<CameraData>();
    [SerializeField] private CameraLocation currentCamera = CameraLocation.None;
    [SerializeField] private bool isTabletUp = false;

    //==========================================
    //
    [Header("UI References")]
    //
    //==========================================
    [SerializeField] private UnityEngine.UI.RawImage cameraDisplay;
    [SerializeField] private GameObject tabletUI;

    //==========================================
    //
    [Header("Debug - Viewpoint Switching")]
    //
    //==========================================
    [SerializeField] private bool debugViewpointMode = false;
    [SerializeField] private Camera mainCamera;

    #endregion

    #region Private Fields

    private PowerSystem powerSystem;
    private Dictionary<CameraLocation, CameraData> cameraDict = new Dictionary<CameraLocation, CameraData>();
    private bool isViewingSecurityCamera = false;
    private Vector3 originalMainCameraPosition;
    private Quaternion originalMainCameraRotation;
    private int currentDebugCameraIndex = -1;

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
        if (Keyboard.current.cKey.wasPressedThisFrame)
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
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SwitchCamera(CameraLocation.CAM_3A);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SwitchCamera(CameraLocation.CAM_4A);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SwitchCamera(CameraLocation.CAM_5A);
        if (Keyboard.current.digit6Key.wasPressedThisFrame) SwitchCamera(CameraLocation.CAM_6);
        if (Keyboard.current.digit7Key.wasPressedThisFrame) SwitchCamera(CameraLocation.CAM_4B);
        if (Keyboard.current.digit8Key.wasPressedThisFrame) SwitchCamera(CameraLocation.CAM_4C);
        if (Keyboard.current.digit9Key.wasPressedThisFrame) SwitchCamera(CameraLocation.CAM_5B);
        if (Keyboard.current.digit0Key.wasPressedThisFrame) SwitchCamera(CameraLocation.CAM_5C);
    }

    private void HandleDebugViewpointInput()
    {
        if (!debugViewpointMode) return;

        // Number keys to switch viewpoint
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SwitchToCameraViewpoint(CameraLocation.CAM_1A);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SwitchToCameraViewpoint(CameraLocation.CAM_2A);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SwitchToCameraViewpoint(CameraLocation.CAM_3A);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SwitchToCameraViewpoint(CameraLocation.CAM_4A);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SwitchToCameraViewpoint(CameraLocation.CAM_5A);
        if (Keyboard.current.digit6Key.wasPressedThisFrame) SwitchToCameraViewpoint(CameraLocation.CAM_6);
        if (Keyboard.current.digit7Key.wasPressedThisFrame) SwitchToCameraViewpoint(CameraLocation.CAM_4B);
        if (Keyboard.current.digit8Key.wasPressedThisFrame) SwitchToCameraViewpoint(CameraLocation.CAM_4C);
        if (Keyboard.current.digit9Key.wasPressedThisFrame) SwitchToCameraViewpoint(CameraLocation.CAM_5B);
        if (Keyboard.current.digit0Key.wasPressedThisFrame) SwitchToCameraViewpoint(CameraLocation.CAM_5C);

        // Cycle through cameras
        if (Keyboard.current.nKey.wasPressedThisFrame) CycleToNextDebugCamera();

        // Return to main camera view
        if (Keyboard.current.digit0Key.wasPressedThisFrame) ReturnToMainCameraView();
    }

    #endregion

    #region Tablet Management

    public void ToggleTablet()
    {
        // toggle
        isTabletUp = !isTabletUp;

        // UI ELEMENT
        if (tabletUI != null)
            tabletUI.SetActive(isTabletUp);

        if (!isTabletUp)
        {
            currentCamera = CameraLocation.None;
            if (cameraDisplay != null)
                cameraDisplay.texture = null;
            ReturnToMainCameraView();
        }
        else
        {
            SwitchToCameraViewpoint(cameras[0].location);
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
        ReturnToMainCameraView();
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

        // Store original view before switching (only if not already viewing a security camera)
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
