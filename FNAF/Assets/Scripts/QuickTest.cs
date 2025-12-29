using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Quick test script to verify all systems are working.
/// Attach this to any GameObject in your scene.
/// </summary>
public class QuickTest : MonoBehaviour
{
    [Header("Test Controls")]

    private GameManager gameManager;
    private PowerSystem powerSystem;
    private DoorSystem doorSystem;
    private LightSystem lightSystem;
    private CameraSystem cameraSystem;

    private void Start()
    {
        Debug.Log("QuickTest: Start() called");

        // Find all systems
        gameManager = FindFirstObjectByType<GameManager>();
        powerSystem = FindFirstObjectByType<PowerSystem>();
        doorSystem = FindFirstObjectByType<DoorSystem>();
        lightSystem = FindFirstObjectByType<LightSystem>();
        cameraSystem = FindFirstObjectByType<CameraSystem>();

        // Check Input System
        if (Keyboard.current == null)
        {
            Debug.LogWarning("QuickTest: Keyboard.current is NULL! Input System may not be initialized.");
        }

        // Check if systems are found
        if (gameManager == null) Debug.LogWarning("QuickTest: GameManager not found!");
        else Debug.Log("QuickTest: GameManager found ✓");

        if (powerSystem == null) Debug.LogWarning("QuickTest: PowerSystem not found!");
        else Debug.Log("QuickTest: PowerSystem found ✓");

        if (doorSystem == null) Debug.LogWarning("QuickTest: DoorSystem not found!");
        else Debug.Log("QuickTest: DoorSystem found ✓");

        if (lightSystem == null) Debug.LogWarning("QuickTest: LightSystem not found!");
        else Debug.Log("QuickTest: LightSystem found ✓");

        if (cameraSystem == null) Debug.LogWarning("QuickTest: CameraSystem not found!");
        else Debug.Log("QuickTest: CameraSystem found ✓");
    }

    /// <summary>
    /// Button listeners run tests
    /// </summary>
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

        // Test camera system (V)
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            TestCameraSystem();
        }
    }

    #region Camera System Tests

    /// <summary>
    /// Tests the camera system status
    /// </summary>
    private void TestCameraSystem()
    {
        Debug.Log("QuickTest: TestCameraSystem() called");

        if (cameraSystem != null)
        {
            Debug.Log("QuickTest: CameraSystem reference is valid");
            bool isTabletUp = cameraSystem.IsTabletUp();
            bool isActive = cameraSystem.IsCameraActive();
            CameraLocation currentCam = cameraSystem.GetCurrentCamera();

            Debug.Log($"| CAMERA SYSTEM STATUS | TABLET: {(isTabletUp ? "UP" : "DOWN")} -- CAMERA is {isActive} and CURRENT CAMERA is {currentCam}");
        }
        else
        {
            Debug.LogError("✗ CameraSystem not found!");
        }
    }
    #endregion

    private void OnGUI()
    {
        // Display on-screen status
        GUI.Label(new Rect(10, 10, 800, 20), "QUICK TEST - (PRESS) SPACE: start night | P: power status | T: door status | L: light status | V: camera status");
        GUI.Label(new Rect(10, 30, 800, 20), "N: Cycle Cameras (1A -> 5C -> 1A) | M: Main Camera");

        // Show system status
        int yOffset = 50;
        GUI.color = gameManager != null ? Color.green : Color.red;
        GUI.Label(new Rect(10, yOffset, 300, 20), $"GameManager: {(gameManager != null ? "✓" : "✗")}");

        GUI.color = powerSystem != null ? Color.green : Color.red;
        GUI.Label(new Rect(10, yOffset + 20, 300, 20), $"PowerSystem: {(powerSystem != null ? "✓" : "✗")}");

        GUI.color = doorSystem != null ? Color.green : Color.red;
        GUI.Label(new Rect(10, yOffset + 40, 300, 20), $"DoorSystem: {(doorSystem != null ? "✓" : "✗")}");

        GUI.color = lightSystem != null ? Color.green : Color.red;
        GUI.Label(new Rect(10, yOffset + 60, 300, 20), $"LightSystem: {(lightSystem != null ? "✓" : "✗")}");

        GUI.color = cameraSystem != null ? Color.green : Color.red;
        GUI.Label(new Rect(10, yOffset + 80, 300, 20), $"CameraSystem: {(cameraSystem != null ? "✓" : "✗")}");

        GUI.color = Keyboard.current != null ? Color.green : Color.red;
        GUI.Label(new Rect(10, yOffset + 100, 300, 20), $"Input System: {(Keyboard.current != null ? "✓" : "✗")}");

        GUI.color = Color.white;
    }
}

