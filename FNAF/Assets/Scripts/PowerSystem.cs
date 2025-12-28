using UnityEngine;
using UnityEngine.Events;

public class PowerSystem : MonoBehaviour
{
    [Header("Power Settings")]
    [SerializeField] private float maxPower = 100f;
    [SerializeField] private float currentPower = 100f;

    [Header("Power Drain Rates (per second)")]
    [SerializeField] private float baseDrainRate = 1f; // Base drain per second
    [SerializeField] private float doorDrainRate = 1f; // Per door per second
    [SerializeField] private float lightDrainRate = 1f; // Per light per second
    [SerializeField] private float cameraDrainRate = 0.5f; // Per camera active per second

    [Header("Events")]
    public UnityEvent<float> OnPowerChanged;
    public UnityEvent OnPowerDepleted;

    private DoorSystem doorSystem;
    private LightSystem lightSystem;
    private CameraSystem cameraSystem;

    private void Start()
    {
        // Get system references
        doorSystem = FindObjectOfType<DoorSystem>();
        lightSystem = FindObjectOfType<LightSystem>();
        cameraSystem = FindObjectOfType<CameraSystem>();

        ResetPower();
    }

    private void Update()
    {
        if (currentPower > 0f)
        {
            CalculatePowerDrain();
            UpdatePower();
        }
    }

    private void CalculatePowerDrain()
    {
        float drainRate = baseDrainRate;

        // Add door drain
        if (doorSystem != null)
        {
            if (doorSystem.IsLeftDoorClosed())
                drainRate += doorDrainRate;
            if (doorSystem.IsRightDoorClosed())
                drainRate += doorDrainRate;
        }

        // Add light drain
        if (lightSystem != null)
        {
            if (lightSystem.IsLeftLightOn())
                drainRate += lightDrainRate;
            if (lightSystem.IsRightLightOn())
                drainRate += lightDrainRate;
        }

        // Add camera drain
        if (cameraSystem != null && cameraSystem.IsCameraActive())
        {
            drainRate += cameraDrainRate;
        }

        // Apply drain
        currentPower -= drainRate * Time.deltaTime;
        currentPower = Mathf.Clamp(currentPower, 0f, maxPower);
    }

    private void UpdatePower()
    {
        OnPowerChanged?.Invoke(currentPower);

        if (currentPower <= 0f)
        {
            OnPowerDepleted?.Invoke();
        }
    }

    public void ResetPower()
    {
        currentPower = maxPower;
        OnPowerChanged?.Invoke(currentPower);
    }

    public float GetPowerLevel()
    {
        return currentPower;
    }

    public float GetPowerPercentage()
    {
        return (currentPower / maxPower) * 100f;
    }

    public bool HasPower()
    {
        return currentPower > 0f;
    }

    public void SetPower(float power)
    {
        currentPower = Mathf.Clamp(power, 0f, maxPower);
        OnPowerChanged?.Invoke(currentPower);
    }

    public void AddPower(float amount)
    {
        currentPower = Mathf.Clamp(currentPower + amount, 0f, maxPower);
        OnPowerChanged?.Invoke(currentPower);
    }

    public void DrainPower(float amount)
    {
        currentPower = Mathf.Clamp(currentPower - amount, 0f, maxPower);
        OnPowerChanged?.Invoke(currentPower);
    }
}

