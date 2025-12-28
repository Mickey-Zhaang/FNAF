using UnityEngine;

public enum AnimatronicState
{
    Idle,
    Moving,
    AtDoor,
    InHallway,
    Attacking
}

public enum AnimatronicLocation
{
    StartingPosition,
    ShowStage,
    DiningArea,
    PirateCove,
    WestHall,
    WestHallCorner,
    SupplyCloset,
    EastHall,
    EastHallCorner,
    Backstage,
    Kitchen,
    Restrooms,
    LeftDoor,
    RightDoor,
    Office
}

public abstract class AnimatronicBase : MonoBehaviour
{
    [Header("Animatronic Settings")]
    [SerializeField] protected string animatronicName;
    [SerializeField] protected int aiLevel = 0; // 0-20 difficulty
    [SerializeField] protected AnimatronicState currentState = AnimatronicState.Idle;
    [SerializeField] protected AnimatronicLocation currentLocation = AnimatronicLocation.StartingPosition;

    [Header("Movement Settings")]
    [SerializeField] protected float moveCooldown = 2f;
    [SerializeField] protected float attackTimer = 5f;
    [SerializeField] protected float minMoveDelay = 5f;
    [SerializeField] protected float maxMoveDelay = 20f;

    [Header("References")]
    protected GameManager gameManager;
    protected CameraSystem cameraSystem;
    protected DoorSystem doorSystem;
    protected LightSystem lightSystem;
    protected AudioManager audioManager;

    protected float moveTimer = 0f;
    protected float nextMoveTime = 0f;
    protected float attackTimeRemaining = 0f;
    protected bool isPausedByCamera = false;

    protected virtual void Start()
    {
        InitializeReferences();
        ResetPosition();
    }

    protected virtual void Update()
    {
        if (gameManager != null && gameManager.currentState != GameState.Playing)
            return;

        UpdateState();
        CheckCameraPause();
    }

    protected virtual void InitializeReferences()
    {
        gameManager = FindObjectOfType<GameManager>();
        cameraSystem = FindObjectOfType<CameraSystem>();
        doorSystem = FindObjectOfType<DoorSystem>();
        lightSystem = FindObjectOfType<LightSystem>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    protected virtual void UpdateState()
    {
        if (isPausedByCamera)
            return;

        moveTimer += Time.deltaTime;

        switch (currentState)
        {
            case AnimatronicState.Idle:
                UpdateIdle();
                break;
            case AnimatronicState.Moving:
                UpdateMoving();
                break;
            case AnimatronicState.AtDoor:
                UpdateAtDoor();
                break;
            case AnimatronicState.InHallway:
                UpdateInHallway();
                break;
            case AnimatronicState.Attacking:
                UpdateAttacking();
                break;
        }
    }

    protected virtual void UpdateIdle()
    {
        if (moveTimer >= nextMoveTime)
        {
            TryMove();
        }
    }

    protected virtual void UpdateMoving()
    {
        // Movement logic handled by derived classes
        moveTimer = 0f;
        currentState = AnimatronicState.Idle;
    }

    protected virtual void UpdateAtDoor()
    {
        attackTimeRemaining -= Time.deltaTime;

        if (attackTimeRemaining <= 0f)
        {
            Attack();
        }
        else
        {
            // Check if door is closed
            DoorSide doorSide = GetDoorSide();
            if (doorSide != DoorSide.Left && doorSide != DoorSide.Right)
                return;

            if (doorSystem != null && doorSystem.IsDoorClosed(doorSide))
            {
                // Door is closed, animatronic is blocked
                BlockedByDoor(doorSide);
            }
        }
    }

    protected virtual void UpdateInHallway()
    {
        // Check if light is on
        LightSide lightSide = GetLightSide();
        if (lightSide == LightSide.Left || lightSide == LightSide.Right)
        {
            if (lightSystem != null && lightSystem.IsLightOn(lightSide))
            {
                // Visible in hallway, move to door
                MoveToDoor(lightSide == LightSide.Left ? DoorSide.Left : DoorSide.Right);
            }
        }
    }

    protected virtual void UpdateAttacking()
    {
        // Jumpscare is happening
        if (gameManager != null)
        {
            gameManager.TriggerJumpscare(animatronicName);
        }
    }

    protected virtual void CheckCameraPause()
    {
        // Some animatronics pause when viewed on camera
        if (cameraSystem != null && cameraSystem.IsCameraActive())
        {
            CameraLocation camLoc = ConvertLocationToCamera(currentLocation);
            if (camLoc != CameraLocation.None && cameraSystem.GetCurrentCamera() == camLoc)
            {
                isPausedByCamera = ShouldPauseWhenViewed();
            }
            else
            {
                isPausedByCamera = false;
            }
        }
        else
        {
            isPausedByCamera = false;
        }
    }

    protected virtual bool ShouldPauseWhenViewed()
    {
        // Override in derived classes
        return false;
    }

    protected virtual void TryMove()
    {
        if (aiLevel <= 0)
            return;

        // Calculate move probability based on AI level
        float moveProbability = CalculateMoveProbability();

        if (Random.value < moveProbability)
        {
            MoveToNextLocation();
        }
        else
        {
            // Stay idle, set next move time
            nextMoveTime = Random.Range(minMoveDelay, maxMoveDelay);
            moveTimer = 0f;
        }
    }

    protected virtual float CalculateMoveProbability()
    {
        // Base probability increases with AI level
        return Mathf.Clamp01(aiLevel / 20f * 0.3f); // Max 30% chance per check
    }

    protected abstract void MoveToNextLocation();
    protected abstract DoorSide GetDoorSide();
    protected abstract LightSide GetLightSide();
    protected abstract CameraLocation ConvertLocationToCamera(AnimatronicLocation location);

    protected virtual void MoveToDoor(DoorSide side)
    {
        currentState = AnimatronicState.AtDoor;
        currentLocation = side == DoorSide.Left ? AnimatronicLocation.LeftDoor : AnimatronicLocation.RightDoor;
        attackTimeRemaining = attackTimer;
        moveTimer = 0f;
    }

    protected virtual void BlockedByDoor(DoorSide side)
    {
        // Animatronic is blocked, wait
        attackTimeRemaining = attackTimer;
    }

    protected virtual void Attack()
    {
        currentState = AnimatronicState.Attacking;

        if (audioManager != null)
        {
            audioManager.PlayJumpscareSound(animatronicName);
        }
    }

    public virtual void ResetPosition()
    {
        currentState = AnimatronicState.Idle;
        currentLocation = AnimatronicLocation.StartingPosition;
        moveTimer = 0f;
        nextMoveTime = Random.Range(minMoveDelay, maxMoveDelay);
        attackTimeRemaining = 0f;
        isPausedByCamera = false;
    }

    public virtual void SetNightDifficulty(int night)
    {
        // Set AI level based on night (0-20 scale)
        switch (night)
        {
            case 1:
                aiLevel = Random.Range(0, 3);
                break;
            case 2:
                aiLevel = Random.Range(2, 5);
                break;
            case 3:
                aiLevel = Random.Range(4, 8);
                break;
            case 4:
                aiLevel = Random.Range(7, 12);
                break;
            case 5:
                aiLevel = Random.Range(10, 20);
                break;
            default:
                aiLevel = Mathf.Clamp(night * 2, 0, 20);
                break;
        }
    }

    public virtual void SetAILevel(int level)
    {
        aiLevel = Mathf.Clamp(level, 0, 20);
    }

    public AnimatronicState GetCurrentState()
    {
        return currentState;
    }

    public AnimatronicLocation GetCurrentLocation()
    {
        return currentLocation;
    }

    public bool IsNearCameraLocation(CameraLocation camLoc)
    {
        AnimatronicLocation animLoc = ConvertCameraToLocation(camLoc);
        return currentLocation == animLoc;
    }

    protected virtual AnimatronicLocation ConvertCameraToLocation(CameraLocation camLoc)
    {
        switch (camLoc)
        {
            case CameraLocation.CAM_1A: return AnimatronicLocation.ShowStage;
            case CameraLocation.CAM_1B: return AnimatronicLocation.DiningArea;
            case CameraLocation.CAM_1C: return AnimatronicLocation.PirateCove;
            case CameraLocation.CAM_2A: return AnimatronicLocation.WestHall;
            case CameraLocation.CAM_2B: return AnimatronicLocation.WestHallCorner;
            case CameraLocation.CAM_3: return AnimatronicLocation.SupplyCloset;
            case CameraLocation.CAM_4A: return AnimatronicLocation.EastHall;
            case CameraLocation.CAM_4B: return AnimatronicLocation.EastHallCorner;
            case CameraLocation.CAM_5: return AnimatronicLocation.Backstage;
            case CameraLocation.CAM_6: return AnimatronicLocation.Kitchen;
            case CameraLocation.CAM_7: return AnimatronicLocation.Restrooms;
            default: return AnimatronicLocation.StartingPosition;
        }
    }

    public bool IsInHallway(LightSide side)
    {
        if (side == LightSide.Left)
        {
            return currentLocation == AnimatronicLocation.WestHall ||
                   currentLocation == AnimatronicLocation.WestHallCorner;
        }
        else
        {
            return currentLocation == AnimatronicLocation.EastHall ||
                   currentLocation == AnimatronicLocation.EastHallCorner;
        }
    }
}

