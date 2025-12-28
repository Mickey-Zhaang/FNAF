using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public enum LightSide
{
    Left,
    Right
}

public class LightSystem : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private bool leftLightOn = false;
    [SerializeField] private bool rightLightOn = false;

    [Header("Light Objects")]
    [SerializeField] private Light leftLight;
    [SerializeField] private Light rightLight;
    [SerializeField] private GameObject leftLightButton;
    [SerializeField] private GameObject rightLightButton;

    [Header("Light Flicker")]
    [SerializeField] private bool enableFlicker = true;
    [SerializeField] private float flickerChance = 0.1f;
    [SerializeField] private float flickerDuration = 0.1f;

    [Header("Events")]
    public UnityEvent<LightSide, bool> OnLightStateChanged;

    private PowerSystem powerSystem;
    private bool leftButtonPressed = false;
    private bool rightButtonPressed = false;
    private float leftFlickerTimer = 0f;
    private float rightFlickerTimer = 0f;

    private void Start()
    {
        powerSystem = FindObjectOfType<PowerSystem>();
        InitializeLights();
    }

    private void Update()
    {
        HandleInput();
        UpdateLightStates();

        if (enableFlicker)
        {
            UpdateFlicker();
        }
    }

    private void InitializeLights()
    {
        if (leftLight != null)
            leftLight.enabled = false;
        if (rightLight != null)
            rightLight.enabled = false;
    }

    private void HandleInput()
    {
        // Left light (A key or left mouse button)
        bool leftInput = (Keyboard.current != null && Keyboard.current.aKey.isPressed) ||
                         (Mouse.current != null && Mouse.current.leftButton.isPressed);

        if (leftInput)
        {
            if (!leftButtonPressed)
            {
                leftButtonPressed = true;
                ToggleLeftLight();
            }
        }
        else
        {
            leftButtonPressed = false;
        }

        // Right light (D key or right mouse button)
        bool rightInput = (Keyboard.current != null && Keyboard.current.dKey.isPressed) ||
                          (Mouse.current != null && Mouse.current.rightButton.isPressed);

        if (rightInput)
        {
            if (!rightButtonPressed)
            {
                rightButtonPressed = true;
                ToggleRightLight();
            }
        }
        else
        {
            rightButtonPressed = false;
        }
    }

    private void UpdateLightStates()
    {
        // Update light visual states
        if (leftLight != null)
        {
            leftLight.enabled = leftLightOn && (leftFlickerTimer <= 0f);
        }

        if (rightLight != null)
        {
            rightLight.enabled = rightLightOn && (rightFlickerTimer <= 0f);
        }
    }

    private void UpdateFlicker()
    {
        // Left light flicker
        if (leftLightOn)
        {
            if (leftFlickerTimer > 0f)
            {
                leftFlickerTimer -= Time.deltaTime;
            }
            else if (Random.value < flickerChance * Time.deltaTime)
            {
                leftFlickerTimer = flickerDuration;
            }
        }

        // Right light flicker
        if (rightLightOn)
        {
            if (rightFlickerTimer > 0f)
            {
                rightFlickerTimer -= Time.deltaTime;
            }
            else if (Random.value < flickerChance * Time.deltaTime)
            {
                rightFlickerTimer = flickerDuration;
            }
        }
    }

    public void ToggleLeftLight()
    {
        if (powerSystem != null && !powerSystem.HasPower())
            return;

        leftLightOn = !leftLightOn;
        OnLightStateChanged?.Invoke(LightSide.Left, leftLightOn);
    }

    public void ToggleRightLight()
    {
        if (powerSystem != null && !powerSystem.HasPower())
            return;

        rightLightOn = !rightLightOn;
        OnLightStateChanged?.Invoke(LightSide.Right, rightLightOn);
    }

    public void TurnOnLeftLight()
    {
        if (powerSystem != null && !powerSystem.HasPower())
            return;

        leftLightOn = true;
        OnLightStateChanged?.Invoke(LightSide.Left, true);
    }

    public void TurnOnRightLight()
    {
        if (powerSystem != null && !powerSystem.HasPower())
            return;

        rightLightOn = true;
        OnLightStateChanged?.Invoke(LightSide.Right, true);
    }

    public void TurnOffLeftLight()
    {
        leftLightOn = false;
        OnLightStateChanged?.Invoke(LightSide.Left, false);
    }

    public void TurnOffRightLight()
    {
        rightLightOn = false;
        OnLightStateChanged?.Invoke(LightSide.Right, false);
    }

    public bool IsLeftLightOn()
    {
        return leftLightOn && leftFlickerTimer <= 0f;
    }

    public bool IsRightLightOn()
    {
        return rightLightOn && rightFlickerTimer <= 0f;
    }

    public bool IsLightOn(LightSide side)
    {
        return side == LightSide.Left ? IsLeftLightOn() : IsRightLightOn();
    }

    public bool CanSeeAnimatronic(LightSide side)
    {
        if (!IsLightOn(side))
            return false;

        // Check if animatronic is in the hallway
        AnimatronicBase[] animatronics = FindObjectsOfType<AnimatronicBase>();
        foreach (var animatronic in animatronics)
        {
            if (animatronic.IsInHallway(side))
            {
                return true;
            }
        }
        return false;
    }

    public void ResetLights()
    {
        leftLightOn = false;
        rightLightOn = false;
        leftFlickerTimer = 0f;
        rightFlickerTimer = 0f;
        OnLightStateChanged?.Invoke(LightSide.Left, false);
        OnLightStateChanged?.Invoke(LightSide.Right, false);
    }
}

