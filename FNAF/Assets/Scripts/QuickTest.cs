using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Quick test script to verify all systems are working.
/// Attach this to any GameObject in your scene.
/// </summary>
public class QuickTest : MonoBehaviour
{
    [Header("Test Controls")]
    [SerializeField] private KeyCode startNightKey = KeyCode.Space;
    [SerializeField] private KeyCode checkPowerKey = KeyCode.P;
    [SerializeField] private KeyCode testDoorKey = KeyCode.T;
    [SerializeField] private KeyCode testLightKey = KeyCode.L;
    [SerializeField] private KeyCode testCameraKey = KeyCode.C;
    [SerializeField] private KeyCode toggleTabletKey = KeyCode.Tab;
    [SerializeField] private KeyCode listCamerasKey = KeyCode.V;
    [SerializeField] private KeyCode cycleCamerasKey = KeyCode.N;
    [SerializeField] private KeyCode toggleDebugViewKey = KeyCode.F5;
    [SerializeField] private KeyCode returnToMainViewKey = KeyCode.F6;

    private GameManager gameManager;
    private PowerSystem powerSystem;
    private DoorSystem doorSystem;
    private LightSystem lightSystem;
    private CameraSystem cameraSystem;

    private void Start()
    {
        // Find all systems
        gameManager = FindFirstObjectByType<GameManager>();
        powerSystem = FindFirstObjectByType<PowerSystem>();
        doorSystem = FindFirstObjectByType<DoorSystem>();
        lightSystem = FindFirstObjectByType<LightSystem>();
        cameraSystem = FindFirstObjectByType<CameraSystem>();

        // Print status
        Debug.Log("=== QUICK TEST SCRIPT LOADED ===");
        Debug.Log("Press SPACE to start Night 1");
        Debug.Log("Press P to check power level");
        Debug.Log("Press T to test doors");
        Debug.Log("Press L to test lights");
        Debug.Log("Press C to test camera system");
        Debug.Log("Press TAB to toggle tablet");
        Debug.Log("Press V to list all cameras");
        Debug.Log("Press N to cycle through cameras");
        Debug.Log("Press F5 to switch to camera viewpoint (debug mode)");
        Debug.Log("Press F6 to return to main camera view");
        Debug.Log("Press 1-7 to switch camera viewpoints (when in debug mode)");
        Debug.Log("Press 0 to return to main view");
        Debug.Log("================================");

        // Check if systems are found
        if (gameManager == null) Debug.LogWarning("GameManager not found!");
        if (powerSystem == null) Debug.LogWarning("PowerSystem not found!");
        if (doorSystem == null) Debug.LogWarning("DoorSystem not found!");
        if (lightSystem == null) Debug.LogWarning("LightSystem not found!");
        if (cameraSystem == null) Debug.LogWarning("CameraSystem not found!");
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        // Start night (Space)
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (gameManager != null)
            {
                gameManager.StartNight(1);
                Debug.Log("✓ Night 1 Started!");
            }
            else
            {
                Debug.LogError("GameManager not found! Make sure it's in the scene.");
            }
        }

        // Check power (P)
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (powerSystem != null)
            {
                float power = powerSystem.GetPowerPercentage();
                Debug.Log($"✓ Power Level: {power:F1}%");
            }
            else
            {
                Debug.LogError("PowerSystem not found!");
            }
        }

        // Test doors (T)
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            if (doorSystem != null)
            {
                bool leftClosed = doorSystem.IsLeftDoorClosed();
                bool rightClosed = doorSystem.IsRightDoorClosed();
                Debug.Log($"✓ Left Door: {(leftClosed ? "CLOSED" : "OPEN")}, Right Door: {(rightClosed ? "CLOSED" : "OPEN")}");
            }
            else
            {
                Debug.LogError("DoorSystem not found!");
            }
        }

        // Test lights (L)
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            if (lightSystem != null)
            {
                bool leftOn = lightSystem.IsLeftLightOn();
                bool rightOn = lightSystem.IsRightLightOn();
                Debug.Log($"✓ Left Light: {(leftOn ? "ON" : "OFF")}, Right Light: {(rightOn ? "ON" : "OFF")}");
            }
            else
            {
                Debug.LogError("LightSystem not found!");
            }
        }

        // Test camera system (C)
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            TestCameraSystem();
        }

        // Toggle tablet (TAB)
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            TestToggleTablet();
        }

        // List all cameras (V)
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            TestListCameras();
        }

        // Cycle through cameras (N)
        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            TestCycleCameras();
        }

        // Toggle debug viewpoint mode (F5)
        if (Keyboard.current.f5Key.wasPressedThisFrame)
        {
            TestToggleDebugViewpoint();
        }

        // Return to main camera view (F6)
        if (Keyboard.current.f6Key.wasPressedThisFrame)
        {
            TestReturnToMainView();
        }
    }

    #region Camera System Tests

    /// <summary>
    /// Tests the camera system status
    /// </summary>
    private void TestCameraSystem()
    {
        if (cameraSystem != null)
        {
            bool isTabletUp = cameraSystem.IsTabletUp();
            bool isActive = cameraSystem.IsCameraActive();
            CameraLocation currentCam = cameraSystem.GetCurrentCamera();

            Debug.Log("=== CAMERA SYSTEM STATUS ===");
            Debug.Log($"✓ Tablet: {(isTabletUp ? "UP" : "DOWN")}");
            Debug.Log($"✓ Camera Active: {isActive}");
            Debug.Log($"✓ Current Camera: {currentCam}");
            Debug.Log("============================");
        }
        else
        {
            Debug.LogError("✗ CameraSystem not found!");
        }
    }

    /// <summary>
    /// Toggles the camera tablet
    /// </summary>
    private void TestToggleTablet()
    {
        if (cameraSystem != null)
        {
            cameraSystem.ToggleTablet();
            bool isUp = cameraSystem.IsTabletUp();
            Debug.Log($"✓ Tablet Toggled: {(isUp ? "UP" : "DOWN")}");

            if (isUp)
            {
                CameraLocation currentCam = cameraSystem.GetCurrentCamera();
                Debug.Log($"  Current Camera: {currentCam}");
            }
        }
        else
        {
            Debug.LogError("✗ CameraSystem not found!");
        }
    }

    /// <summary>
    /// Lists all available cameras in the system
    /// </summary>
    private void TestListCameras()
    {
        if (cameraSystem != null)
        {
            // Use reflection to access the cameras list, or we can add a public method
            // For now, let's test by trying to switch to known cameras
            Debug.Log("=== AVAILABLE CAMERAS ===");
            Debug.Log("Testing camera locations...");

            // Test common camera locations
            CameraLocation[] testLocations = {
                CameraLocation.CAM_1A, CameraLocation.CAM_1B, CameraLocation.CAM_1C,
                CameraLocation.CAM_2A, CameraLocation.CAM_2B, CameraLocation.CAM_3,
                CameraLocation.CAM_4A, CameraLocation.CAM_4B, CameraLocation.CAM_5,
                CameraLocation.CAM_6, CameraLocation.CAM_7
            };

            int foundCount = 0;
            foreach (var location in testLocations)
            {
                // Try to switch to camera - if it works, it exists
                bool tabletWasUp = cameraSystem.IsTabletUp();
                if (!tabletWasUp)
                {
                    cameraSystem.ToggleTablet();
                }

                CameraLocation beforeSwitch = cameraSystem.GetCurrentCamera();
                cameraSystem.SwitchCamera(location);
                CameraLocation afterSwitch = cameraSystem.GetCurrentCamera();

                if (afterSwitch == location)
                {
                    Debug.Log($"  ✓ {location} - Available");
                    foundCount++;
                }

                if (!tabletWasUp)
                {
                    cameraSystem.ToggleTablet();
                }
            }

            Debug.Log($"Total cameras found: {foundCount}");
            Debug.Log("=========================");
        }
        else
        {
            Debug.LogError("✗ CameraSystem not found!");
        }
    }

    /// <summary>
    /// Cycles through all available cameras
    /// </summary>
    private void TestCycleCameras()
    {
        if (cameraSystem != null)
        {
            // Make sure tablet is up
            if (!cameraSystem.IsTabletUp())
            {
                cameraSystem.ToggleTablet();
            }

            CameraLocation currentCam = cameraSystem.GetCurrentCamera();
            Debug.Log($"Current Camera: {currentCam}");

            // Try to switch to next camera in sequence
            CameraLocation[] cameraSequence = {
                CameraLocation.CAM_1A, CameraLocation.CAM_1B, CameraLocation.CAM_1C,
                CameraLocation.CAM_2A, CameraLocation.CAM_2B, CameraLocation.CAM_3,
                CameraLocation.CAM_4A, CameraLocation.CAM_4B, CameraLocation.CAM_5,
                CameraLocation.CAM_6, CameraLocation.CAM_7
            };

            int currentIndex = -1;
            for (int i = 0; i < cameraSequence.Length; i++)
            {
                if (cameraSequence[i] == currentCam)
                {
                    currentIndex = i;
                    break;
                }
            }

            if (currentIndex >= 0)
            {
                // Try next camera
                int nextIndex = (currentIndex + 1) % cameraSequence.Length;
                CameraLocation nextCam = cameraSequence[nextIndex];
                cameraSystem.SwitchCamera(nextCam);
                Debug.Log($"✓ Switched to: {nextCam}");
            }
            else
            {
                // Start from first camera
                cameraSystem.SwitchCamera(CameraLocation.CAM_1A);
                Debug.Log($"✓ Switched to: CAM_1A");
            }
        }
        else
        {
            Debug.LogError("✗ CameraSystem not found!");
        }
    }

    /// <summary>
    /// Toggles debug viewpoint mode (switches main camera to security camera views)
    /// </summary>
    private void TestToggleDebugViewpoint()
    {
        if (cameraSystem != null)
        {
            // Switch to first available camera for debugging
            cameraSystem.SwitchToCameraViewpoint(CameraLocation.CAM_1A);
            Debug.Log("✓ Debug Viewpoint: Switched to CAM_1A");
            Debug.Log("  Press 1-7 to switch between cameras, 0 to return to main view");
        }
        else
        {
            Debug.LogError("✗ CameraSystem not found!");
        }
    }

    /// <summary>
    /// Returns to main camera view
    /// </summary>
    private void TestReturnToMainView()
    {
        if (cameraSystem != null)
        {
            cameraSystem.ReturnToMainCameraView();
            Debug.Log("✓ Returned to main camera view");
        }
        else
        {
            Debug.LogError("✗ CameraSystem not found!");
        }
    }

    /// <summary>
    /// Tests if animatronics are visible in cameras
    /// </summary>
    private void TestAnimatronicVisibility()
    {
        if (cameraSystem != null)
        {
            CameraLocation currentCam = cameraSystem.GetCurrentCamera();
            if (currentCam != CameraLocation.None)
            {
                AnimatronicBase[] animatronics = FindObjectsByType<AnimatronicBase>(FindObjectsSortMode.None);
                Debug.Log($"=== ANIMATRONIC VISIBILITY (Camera: {currentCam}) ===");

                foreach (var animatronic in animatronics)
                {
                    bool isVisible = cameraSystem.IsAnimatronicVisible(currentCam, animatronic.name);
                    Debug.Log($"  {animatronic.name}: {(isVisible ? "VISIBLE" : "Not visible")}");
                }

                Debug.Log("=========================================");
            }
            else
            {
                Debug.LogWarning("⚠ No camera is currently active. Toggle tablet first (TAB)");
            }
        }
        else
        {
            Debug.LogError("✗ CameraSystem not found!");
        }
    }

    #endregion

    private void OnGUI()
    {
        // Display on-screen instructions
        GUI.Label(new Rect(10, 10, 600, 40),
            "QUICK TEST - SPACE: start night | P: power | T: doors | L: lights | C: camera status | TAB: toggle tablet | V: list cameras | N: cycle cameras");
    }
}

