using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public enum DoorSide
{
    Left,
    Right
}

public class DoorSystem : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private bool leftDoorClosed = false;
    [SerializeField] private bool rightDoorClosed = false;

    [Header("Door Objects")]
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;
    [SerializeField] private GameObject leftDoorButton;
    [SerializeField] private GameObject rightDoorButton;

    [Header("Events")]
    public UnityEvent<DoorSide, bool> OnDoorStateChanged;

    private PowerSystem powerSystem;
    private bool leftButtonPressed = false;
    private bool rightButtonPressed = false;

    private void Start()
    {
        powerSystem = FindObjectOfType<PowerSystem>();
    }

    private void Update()
    {
        HandleInput();
        UpdateDoorStates();
    }

    private void HandleInput()
    {
        if (Keyboard.current == null) return;

        // Left door (Q key)
        if (Keyboard.current.qKey.isPressed)
        {
            if (!leftButtonPressed)
            {
                leftButtonPressed = true;
                ToggleLeftDoor();
            }
        }
        else
        {
            leftButtonPressed = false;
        }

        // Right door (E key)
        if (Keyboard.current.eKey.isPressed)
        {
            if (!rightButtonPressed)
            {
                rightButtonPressed = true;
                ToggleRightDoor();
            }
        }
        else
        {
            rightButtonPressed = false;
        }
    }

    private void UpdateDoorStates()
    {
        // Update door visual states
        if (leftDoor != null)
        {
            leftDoor.SetActive(leftDoorClosed);
        }

        if (rightDoor != null)
        {
            rightDoor.SetActive(rightDoorClosed);
        }
    }

    public void ToggleLeftDoor()
    {
        if (powerSystem != null && !powerSystem.HasPower())
            return;

        leftDoorClosed = !leftDoorClosed;
        OnDoorStateChanged?.Invoke(DoorSide.Left, leftDoorClosed);
    }

    public void ToggleRightDoor()
    {
        if (powerSystem != null && !powerSystem.HasPower())
            return;

        rightDoorClosed = !rightDoorClosed;
        OnDoorStateChanged?.Invoke(DoorSide.Right, rightDoorClosed);
    }

    public void CloseLeftDoor()
    {
        if (powerSystem != null && !powerSystem.HasPower())
            return;

        leftDoorClosed = true;
        OnDoorStateChanged?.Invoke(DoorSide.Left, true);
    }

    public void CloseRightDoor()
    {
        if (powerSystem != null && !powerSystem.HasPower())
            return;

        rightDoorClosed = true;
        OnDoorStateChanged?.Invoke(DoorSide.Right, true);
    }

    public void OpenLeftDoor()
    {
        leftDoorClosed = false;
        OnDoorStateChanged?.Invoke(DoorSide.Left, false);
    }

    public void OpenRightDoor()
    {
        rightDoorClosed = false;
        OnDoorStateChanged?.Invoke(DoorSide.Right, false);
    }

    public bool IsLeftDoorClosed()
    {
        return leftDoorClosed;
    }

    public bool IsRightDoorClosed()
    {
        return rightDoorClosed;
    }

    public bool IsDoorClosed(DoorSide side)
    {
        return side == DoorSide.Left ? leftDoorClosed : rightDoorClosed;
    }

    public bool CanAnimatronicEnter(DoorSide side)
    {
        return !IsDoorClosed(side);
    }

    public void ResetDoors()
    {
        leftDoorClosed = false;
        rightDoorClosed = false;
        OnDoorStateChanged?.Invoke(DoorSide.Left, false);
        OnDoorStateChanged?.Invoke(DoorSide.Right, false);
    }

    public void BlockAnimatronic(DoorSide side)
    {
        // Called when animatronic tries to enter but door is closed
        // This can trigger door attack mechanics
        if (side == DoorSide.Left)
        {
            // Left door attack (Bonnie)
        }
        else
        {
            // Right door attack (Chica)
        }
    }
}

