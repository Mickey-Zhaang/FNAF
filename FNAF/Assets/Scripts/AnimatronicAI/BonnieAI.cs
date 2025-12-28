using UnityEngine;

public class BonnieAI : AnimatronicBase
{
    [Header("Bonnie Specific")]
    [SerializeField] private AnimatronicLocation[] movementPath;

    protected override void Start()
    {
        base.Start();
        animatronicName = "Bonnie";

        // Bonnie's movement path: Show Stage -> Dining Area -> West Hall -> West Hall Corner -> Left Door
        movementPath = new AnimatronicLocation[]
        {
            AnimatronicLocation.ShowStage,
            AnimatronicLocation.DiningArea,
            AnimatronicLocation.WestHall,
            AnimatronicLocation.WestHallCorner,
            AnimatronicLocation.LeftDoor
        };
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
            if (currentLocation == AnimatronicLocation.WestHall ||
                currentLocation == AnimatronicLocation.WestHallCorner)
            {
                currentState = AnimatronicState.InHallway;
            }
            // If reached door, start attack timer
            else if (currentLocation == AnimatronicLocation.LeftDoor)
            {
                MoveToDoor(DoorSide.Left);
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
        return DoorSide.Left;
    }

    protected override LightSide GetLightSide()
    {
        return LightSide.Left;
    }

    protected override CameraLocation ConvertLocationToCamera(AnimatronicLocation location)
    {
        switch (location)
        {
            case AnimatronicLocation.ShowStage:
                return CameraLocation.CAM_1A;
            case AnimatronicLocation.DiningArea:
                return CameraLocation.CAM_1B;
            case AnimatronicLocation.WestHall:
                return CameraLocation.CAM_2A;
            case AnimatronicLocation.WestHallCorner:
                return CameraLocation.CAM_2B;
            default:
                return CameraLocation.None;
        }
    }

    protected override bool ShouldPauseWhenViewed()
    {
        // Bonnie pauses when viewed on camera
        return true;
    }

    protected override void UpdateInHallway()
    {
        base.UpdateInHallway();

        // If light is off, continue moving
        if (lightSystem != null && !lightSystem.IsLeftLightOn())
        {
            // Continue to next location
            if (currentLocation == AnimatronicLocation.WestHall)
            {
                currentLocation = AnimatronicLocation.WestHallCorner;
            }
            else if (currentLocation == AnimatronicLocation.WestHallCorner)
            {
                MoveToDoor(DoorSide.Left);
            }
        }
    }
}

