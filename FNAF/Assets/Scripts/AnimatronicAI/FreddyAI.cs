using UnityEngine;

public class FreddyAI : AnimatronicBase
{
    [Header("Freddy Specific")]
    [SerializeField] private AnimatronicLocation[] movementPath;
    [SerializeField] private float powerThreshold = 0f; // Freddy only moves when power is low

    protected override void Start()
    {
        base.Start();
        animatronicName = "Freddy";

        // Freddy's movement path: Show Stage -> Dining Area -> East Hall -> East Hall Corner -> Right Door
        // Freddy only moves when power is very low
        movementPath = new AnimatronicLocation[]
        {
            AnimatronicLocation.ShowStage,
            AnimatronicLocation.DiningArea,
            AnimatronicLocation.EastHall,
            AnimatronicLocation.EastHallCorner,
            AnimatronicLocation.RightDoor
        };
    }

    protected override void UpdateIdle()
    {
        // Freddy only moves when power is below threshold
        PowerSystem power = FindObjectOfType<PowerSystem>();
        if (power != null && power.GetPowerPercentage() > powerThreshold)
        {
            // Power too high, stay idle
            moveTimer = 0f;
            nextMoveTime = Random.Range(minMoveDelay, maxMoveDelay);
            return;
        }

        base.UpdateIdle();
    }

    protected override void MoveToNextLocation()
    {
        int currentIndex = GetCurrentPathIndex();

        if (currentIndex < 0)
        {
            // Start from beginning
            currentLocation = movementPath[0];
            currentState = AnimatronicState.Moving;
        }
        else if (currentIndex < movementPath.Length - 1)
        {
            // Move to next location in path
            currentLocation = movementPath[currentIndex + 1];
            currentState = AnimatronicState.Moving;

            // If reached hallway, check for light
            if (currentLocation == AnimatronicLocation.EastHall ||
                currentLocation == AnimatronicLocation.EastHallCorner)
            {
                currentState = AnimatronicState.InHallway;
            }
            // If reached door, start attack timer
            else if (currentLocation == AnimatronicLocation.RightDoor)
            {
                MoveToDoor(DoorSide.Right);
            }
        }

        moveTimer = 0f;
        nextMoveTime = Random.Range(minMoveDelay, maxMoveDelay);
    }

    private int GetCurrentPathIndex()
    {
        for (int i = 0; i < movementPath.Length; i++)
        {
            if (movementPath[i] == currentLocation)
                return i;
        }
        return -1;
    }

    protected override DoorSide GetDoorSide()
    {
        return DoorSide.Right;
    }

    protected override LightSide GetLightSide()
    {
        return LightSide.Right;
    }

    protected override CameraLocation ConvertLocationToCamera(AnimatronicLocation location)
    {
        switch (location)
        {
            case AnimatronicLocation.ShowStage:
                return CameraLocation.CAM_1A;
            case AnimatronicLocation.DiningArea:
                return CameraLocation.CAM_1B;
            case AnimatronicLocation.EastHall:
                return CameraLocation.CAM_4A;
            case AnimatronicLocation.EastHallCorner:
                return CameraLocation.CAM_4B;
            default:
                return CameraLocation.None;
        }
    }

    protected override bool ShouldPauseWhenViewed()
    {
        // Freddy pauses when viewed on camera
        return true;
    }

    protected override float CalculateMoveProbability()
    {
        // Freddy moves less frequently, only when power is low
        PowerSystem power = FindObjectOfType<PowerSystem>();
        if (power != null && power.GetPowerPercentage() <= powerThreshold)
        {
            return base.CalculateMoveProbability() * 0.5f; // Half the normal probability
        }
        return 0f;
    }

    protected override void UpdateInHallway()
    {
        base.UpdateInHallway();

        // If light is off, continue moving
        if (lightSystem != null && !lightSystem.IsRightLightOn())
        {
            // Continue to next location
            if (currentLocation == AnimatronicLocation.EastHall)
            {
                currentLocation = AnimatronicLocation.EastHallCorner;
            }
            else if (currentLocation == AnimatronicLocation.EastHallCorner)
            {
                MoveToDoor(DoorSide.Right);
            }
        }
    }
}

