using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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
    private LocationManager locationManager;
    private BonnieAI bonnieAI;
    private List<LocationWaypoint> bonnieWaypoints = new List<LocationWaypoint>();
    private int currentWaypointIndex = -1;

    private void Start()
    {
        Debug.Log("QuickTest: Start() called");

        // Find all systems
        gameManager = FindFirstObjectByType<GameManager>();
        powerSystem = FindFirstObjectByType<PowerSystem>();
        doorSystem = FindFirstObjectByType<DoorSystem>();
        lightSystem = FindFirstObjectByType<LightSystem>();
        cameraSystem = FindFirstObjectByType<CameraSystem>();
        locationManager = LocationManager.Instance;
        bonnieAI = FindFirstObjectByType<BonnieAI>();

        // Get waypoints for Bonnie
        if (locationManager != null && bonnieAI != null)
        {
            bonnieWaypoints = locationManager.GetWaypointsForAnimatronic("Bonnie");
            Debug.Log($"QuickTest: Found {bonnieWaypoints.Count} waypoints for Bonnie");
            foreach (var waypoint in bonnieWaypoints)
            {
                Debug.Log($"  - {waypoint.GetWaypointName()}");
            }
        }

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

        // Cycle Bonnie between waypoints (K)
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            TestCycleBonnieWaypoints();
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

    #region Waypoint System Tests

    /// <summary>
    /// Cycles Bonnie between available waypoints when K is pressed
    /// </summary>
    private void TestCycleBonnieWaypoints()
    {
        if (bonnieAI == null)
        {
            Debug.LogError("✗ BonnieAI not found! Make sure Bonnie is in the scene with BonnieAI component.");
            return;
        }

        if (locationManager == null)
        {
            Debug.LogError("✗ LocationManager not found!");
            return;
        }

        bonnieWaypoints = locationManager.GetWaypointsForAnimatronic("Bonnie");

        // Find current waypoint index
        LocationWaypoint currentWaypoint = bonnieAI.GetCurrentWaypoint();
        if (currentWaypoint != null)
        {
            currentWaypointIndex = bonnieWaypoints.IndexOf(currentWaypoint);
        }

        // Cycle to next waypoint
        currentWaypointIndex = (currentWaypointIndex + 1) % bonnieWaypoints.Count;
        LocationWaypoint nextWaypoint = bonnieWaypoints[currentWaypointIndex];

        // Release current waypoint if occupied
        if (currentWaypoint != null)
        {
            currentWaypoint.Release();
        }

        if (nextWaypoint != null)
        {
            // Try to occupy the waypoint (public method)
            if (nextWaypoint.TryOccupy(bonnieAI))
            {
                // Update Bonnie's internal state using reflection (only for private fields)
                System.Reflection.FieldInfo waypointField = typeof(AnimatronicBase).GetField("currentWaypoint",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                System.Reflection.FieldInfo isMovingField = typeof(AnimatronicBase).GetField("isMovingToWaypoint",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                System.Reflection.FieldInfo stateField = typeof(AnimatronicBase).GetField("currentState",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                System.Reflection.FieldInfo waypointNameField = typeof(AnimatronicBase).GetField("currentWaypointName",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (waypointField != null) waypointField.SetValue(bonnieAI, nextWaypoint);
                if (isMovingField != null) isMovingField.SetValue(bonnieAI, false);
                if (stateField != null) stateField.SetValue(bonnieAI, AnimatronicState.Idle);
                if (waypointNameField != null) waypointNameField.SetValue(bonnieAI, nextWaypoint.GetWaypointName());

                // Teleport Bonnie to the waypoint position
                bonnieAI.transform.position = nextWaypoint.GetPosition();

                // Check if Bonnie entered the office (game over condition)
                string waypointName = nextWaypoint.GetWaypointName();
                if (string.Equals(waypointName, "Office", System.StringComparison.OrdinalIgnoreCase))
                {
                    Debug.LogWarning($"Bonnie entered the Office! Triggering jumpscare and game over!");
                    if (gameManager != null)
                    {
                        gameManager.TriggerJumpscare("Bonnie");
                    }
                    else
                    {
                        Debug.LogError("GameManager is NULL! Cannot trigger jumpscare.");
                    }
                }

                Debug.Log($"✓ Bonnie teleported to waypoint: {nextWaypoint.GetWaypointName()} ({currentWaypointIndex + 1}/{bonnieWaypoints.Count})");
            }
            else
            {
                Debug.LogWarning($"✗ Could not teleport Bonnie to waypoint: {nextWaypoint.GetWaypointName()} (may be occupied)");
            }
        }
        else
        {
            Debug.LogWarning($"✗ Could not move Bonnie to waypoint: {nextWaypoint?.GetWaypointName() ?? "Unknown"} (may be occupied)");
        }
    }

    #endregion

    private void OnGUI()
    {
        // Check if game is over and show game over screen
        if (gameManager != null)
        {
            // Debug: Show current game state
            GUI.color = Color.yellow;
            GUI.Label(new Rect(10, Screen.height - 30, 300, 20), $"Game State: {gameManager.currentState}");
            GUI.color = Color.white;

            if (gameManager.currentState == GameState.GameOver)
            {
                Debug.Log("OnGUI: Game state is GameOver, calling ShowGameOverScreen()");
                ShowGameOverScreen();
                return; // Don't show other UI when game is over
            }
        }

        // Display on-screen status
        GUI.Label(new Rect(10, 10, 800, 20), "QUICK TEST - (PRESS) SPACE: start night | P: power status | T: door status | L: light status | V: camera status");
        GUI.Label(new Rect(10, 30, 800, 20), "N: Cycle Cameras (1A -> 5C -> 1A) | M: Main Camera | K: Cycle Bonnie Waypoints");

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

        GUI.color = locationManager != null ? Color.green : Color.red;
        GUI.Label(new Rect(10, yOffset + 120, 300, 20), $"LocationManager: {(locationManager != null ? "✓" : "✗")}");

        GUI.color = bonnieAI != null ? Color.green : Color.red;
        GUI.Label(new Rect(10, yOffset + 140, 300, 20), $"BonnieAI: {(bonnieAI != null ? "✓" : "✗")}");

        GUI.color = Color.white;
    }

    /// <summary>
    /// Displays game over screen when game state is GameOver
    /// </summary>
    private void ShowGameOverScreen()
    {
        Debug.Log("ShowGameOverScreen() called - rendering game over UI");

        // Dark background overlay
        GUI.color = new Color(0, 0, 0, 0.8f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);

        // Game Over text
        GUI.color = Color.red;
        GUIStyle gameOverStyle = new GUIStyle(GUI.skin.label);
        gameOverStyle.fontSize = 48;
        gameOverStyle.alignment = TextAnchor.MiddleCenter;
        gameOverStyle.fontStyle = FontStyle.Bold;
        GUI.Label(new Rect(0, Screen.height / 2 - 100, Screen.width, 60), "GAME OVER", gameOverStyle);

        // Reason text
        GUI.color = Color.white;
        GUIStyle reasonStyle = new GUIStyle(GUI.skin.label);
        reasonStyle.fontSize = 24;
        reasonStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(0, Screen.height / 2 - 20, Screen.width, 30), "Jumpscare", reasonStyle);

        // Instructions
        GUI.color = Color.yellow;
        GUIStyle instructionStyle = new GUIStyle(GUI.skin.label);
        instructionStyle.fontSize = 18;
        instructionStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(0, Screen.height / 2 + 50, Screen.width, 30), "Press R to Restart | Press ESC to Return to Menu", instructionStyle);

        // Handle restart input
        if (Keyboard.current != null)
        {
            if (Keyboard.current.rKey.wasPressedThisFrame && gameManager != null)
            {
                gameManager.ResetGameState();
            }
            else if (Keyboard.current.escapeKey.wasPressedThisFrame && gameManager != null)
            {
                gameManager.ReturnToMenu();
            }
        }

        GUI.color = Color.white;
    }
}

