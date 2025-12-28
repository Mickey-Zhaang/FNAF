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

    private GameManager gameManager;
    private PowerSystem powerSystem;
    private DoorSystem doorSystem;
    private LightSystem lightSystem;

    private void Start()
    {
        // Find all systems
        gameManager = FindObjectOfType<GameManager>();
        powerSystem = FindObjectOfType<PowerSystem>();
        doorSystem = FindObjectOfType<DoorSystem>();
        lightSystem = FindObjectOfType<LightSystem>();

        // Print status
        Debug.Log("=== QUICK TEST SCRIPT LOADED ===");
        Debug.Log("Press SPACE to start Night 1");
        Debug.Log("Press P to check power level");
        Debug.Log("Press T to test doors");
        Debug.Log("Press L to test lights");
        Debug.Log("================================");

        // Check if systems are found
        if (gameManager == null) Debug.LogWarning("GameManager not found!");
        if (powerSystem == null) Debug.LogWarning("PowerSystem not found!");
        if (doorSystem == null) Debug.LogWarning("DoorSystem not found!");
        if (lightSystem == null) Debug.LogWarning("LightSystem not found!");
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
    }

    private void OnGUI()
    {
        // Display on-screen instructions
        GUI.Label(new Rect(10, 10, 400, 20), "QUICK TEST - Press SPACE to start night, P for power, T for doors, L for lights");
    }
}

