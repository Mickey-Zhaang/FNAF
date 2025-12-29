using UnityEngine;

/// <summary>
/// Bare bones animatronic template - self-contained with no external dependencies.
/// Use this as a base class for creating custom animatronics.
/// </summary>
public enum AnimatronicState
{
    Idle,
    Moving,
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
    [SerializeField] protected string animatronicName = "Animatronic";
    [SerializeField] protected int aiLevel = 0; // 0-20 difficulty
    [SerializeField] protected AnimatronicState currentState = AnimatronicState.Idle;
    [SerializeField] protected AnimatronicLocation currentLocation = AnimatronicLocation.StartingPosition;

    [Header("Movement Settings")]
    [SerializeField] protected float moveCooldown = 2f;
    [SerializeField] protected float minMoveDelay = 5f;
    [SerializeField] protected float maxMoveDelay = 20f;

    protected float moveTimer = 0f;
    protected float nextMoveTime = 0f;

    protected virtual void Start()
    {
        ResetPosition();
    }

    protected virtual void Update()
    {
        UpdateState();
    }

    protected virtual void UpdateState()
    {
        moveTimer += Time.deltaTime;

        switch (currentState)
        {
            case AnimatronicState.Idle:
                UpdateIdle();
                break;
            case AnimatronicState.Moving:
                UpdateMoving();
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
        // Movement logic - override in derived classes
        moveTimer = 0f;
        currentState = AnimatronicState.Idle;
    }

    protected virtual void UpdateAttacking()
    {
        // Attack logic - override in derived classes
        Debug.Log($"{animatronicName} is attacking!");
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

    /// <summary>
    /// Override this to define movement behavior
    /// </summary>
    protected abstract void MoveToNextLocation();

    protected virtual void Attack()
    {
        currentState = AnimatronicState.Attacking;
        Debug.Log($"{animatronicName} attacks!");
    }

    public virtual void ResetPosition()
    {
        currentState = AnimatronicState.Idle;
        currentLocation = AnimatronicLocation.StartingPosition;
        moveTimer = 0f;
        nextMoveTime = Random.Range(minMoveDelay, maxMoveDelay);
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

    public string GetAnimatronicName()
    {
        return animatronicName;
    }
}
