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
    CAM_3B,
    CAM_3C,
    CAM_4A,
    CAM_4B,
    CAM_4C,
    CAM_5A,
    CAM_5B,
    CAM_5C,
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
    [SerializeField] private bool debugViewpointMode = true; // Always enabled for debugging - tablet mode disabled
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
            if (cam != null && cam.location != CameraLocation.None)
            {
                cameraDict[cam.location] = cam;
            }
        }

        Debug.Log($"CameraSystem: Initialized {cameras.Count} cameras. Available cameras:");
        for (int i = 0; i < cameras.Count; i++)
        {
            if (cameras[i] != null)
            {
                Debug.Log($"  [{i}] {cameras[i].location} - {cameras[i].displayName}");
            }
        }
    }

    private void InitializeMainCamera()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Store the original main camera position/rotation at startup
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

        // Only handle debug viewpoint mode - tablet mode disabled for debugging
        HandleDebugViewpointInput();

    }
    /// <summary>
    /// Cycles to the next camera in tablet mode (updates the tablet display)
    /// </summary>

    private void HandleDebugViewpointInput()
    {
        // Always enable debug mode for now (tablet mode disabled)
        // if (!debugViewpointMode) 
        // {
        //     Debug.LogWarning("CameraSystem: HandleDebugViewpointInput called but debugViewpointMode is false!");
        //     return;
        // }

        // Cycle through cameras (N key)
        if (Keyboard.current.nKey.wasPressedThisFrame) CycleToNextDebugCamera();

        // Return to main camera view (m key)
        if (Keyboard.current.mKey.wasPressedThisFrame) ReturnToMainCameraView();
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
            // When opening tablet, switch to first camera using SwitchCamera (for tablet display)
            // NOT SwitchToCameraViewpoint (which is for debug mode)
            if (cameras.Count > 0 && cameras[0] != null)
            {
                SwitchCamera(cameras[0].location);
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

        // Only restore main camera view if we were actually viewing a security camera
        if (isViewingSecurityCamera)
        {
            ReturnToMainCameraView();
        }
        else
        {
            // Just ensure all security cameras are rendering to their textures
            RestoreAllCameraTextures();
        }
    }

    #endregion

    #region Camera Switching

    public void SwitchCamera(CameraLocation location)
    {
        if (!isTabletUp || !cameraDict.ContainsKey(location))
        {
            Debug.LogWarning($"CameraSystem: Cannot switch to {location} - Tablet: {isTabletUp}, In Dict: {cameraDict.ContainsKey(location)}");
            return;
        }

        // Don't switch if we're in debug viewpoint mode
        if (debugViewpointMode)
        {
            Debug.LogWarning($"CameraSystem: Cannot use SwitchCamera in debug viewpoint mode. Use SwitchToCameraViewpoint instead.");
            return;
        }

        currentCamera = location;
        CameraData camData = cameraDict[location];

        Debug.Log($"CameraSystem: Switching tablet to {location} ({camData.displayName})");

        if (cameraDisplay != null)
        {
            if (camData.renderTexture != null)
            {
                cameraDisplay.texture = camData.renderTexture;
            }
            else
            {
                Debug.LogWarning($"CameraSystem: RenderTexture is null for {location}");
                cameraDisplay.texture = null;
            }
        }
        else
        {
            Debug.LogWarning("CameraSystem: cameraDisplay is null!");
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

    /// <summary>
    /// Returns the number of cameras in the system
    /// </summary>
    public int GetCameraCount()
    {
        return cameras.Count;
    }

    /// <summary>
    /// Logs all cameras in the system (for debugging)
    /// </summary>
    [ContextMenu("List All Cameras")]
    public void ListAllCameras()
    {
        Debug.Log($"=== CAMERA SYSTEM DEBUG ===");
        Debug.Log($"Total cameras in list: {cameras.Count}");
        Debug.Log($"Cameras in dictionary: {cameraDict.Count}");

        for (int i = 0; i < cameras.Count; i++)
        {
            if (cameras[i] != null)
            {
                Debug.Log($"  [{i}] {cameras[i].location} - {cameras[i].displayName} - Camera: {(cameras[i].camera != null ? cameras[i].camera.name : "NULL")}");
            }
            else
            {
                Debug.LogWarning($"  [{i}] NULL ENTRY");
            }
        }

        Debug.Log($"Current camera: {currentCamera}");
        Debug.Log($"Current debug index: {currentDebugCameraIndex}");
        Debug.Log("===========================");
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
    /// Cycles to the next camera in debug viewpoint mode (cycles through all cameras)
    /// </summary>
    private void CycleToNextDebugCamera()
    {
        if (cameras.Count == 0)
        {
            Debug.LogWarning("CameraSystem: No cameras available to cycle. Make sure cameras are added to the cameras list in the Inspector!");
            return;
        }

        // Filter out null cameras
        List<CameraData> validCameras = new List<CameraData>();
        foreach (var cam in cameras)
        {
            if (cam != null && cam.camera != null && cam.location != CameraLocation.None)
            {
                validCameras.Add(cam);
            }
        }

        if (validCameras.Count == 0)
        {
            Debug.LogWarning("CameraSystem: No valid cameras found in the cameras list!");
            return;
        }

        // Find current camera index in valid cameras list
        int currentValidIndex = -1;
        if (currentDebugCameraIndex >= 0 && currentDebugCameraIndex < cameras.Count)
        {
            CameraLocation currentLoc = cameras[currentDebugCameraIndex].location;
            for (int i = 0; i < validCameras.Count; i++)
            {
                if (validCameras[i].location == currentLoc)
                {
                    currentValidIndex = i;
                    break;
                }
            }
        }

        // Move to next camera
        currentValidIndex++;
        if (currentValidIndex >= validCameras.Count)
        {
            currentValidIndex = 0;
        }

        // Update the actual index in the cameras list
        CameraLocation nextLocation = validCameras[currentValidIndex].location;
        currentDebugCameraIndex = GetCameraIndex(nextLocation);

        SwitchToCameraViewpoint(nextLocation);
        Debug.Log($"CameraSystem: Cycling to camera {currentValidIndex + 1}/{validCameras.Count}: {nextLocation} ({validCameras[currentValidIndex].displayName})");
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
