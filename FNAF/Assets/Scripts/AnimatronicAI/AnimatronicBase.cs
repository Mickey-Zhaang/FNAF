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

public abstract class AnimatronicBase : MonoBehaviour
{
    [Header("Animatronic Settings")]
    [SerializeField] protected string animatronicName = "Animatronic";
    [SerializeField] protected int aiLevel = 0; // 0-20 difficulty
    [SerializeField] protected AnimatronicState currentState = AnimatronicState.Idle;

    [Header("Current Location (Read-Only)")]
    [SerializeField, Tooltip("Current waypoint name - updated automatically")]
    private string currentWaypointName = "None";

    [Header("Movement Settings")]
    [SerializeField] protected float moveCooldown = 2f;
    [SerializeField] protected float minMoveDelay = 5f;
    [SerializeField] protected float maxMoveDelay = 20f;
    [SerializeField] protected float moveSpeed = 5f; // Speed for moving to waypoint positions

    protected float moveTimer = 0f;
    protected float nextMoveTime = 0f;
    protected LocationManager locationManager = null;
    protected LocationWaypoint currentWaypoint = null;
    protected Vector3 targetPosition;
    protected bool isMovingToWaypoint = false;
    protected GameManager gameManager = null;

    protected virtual void Start()
    {
        InitializeReferences();
        ResetPosition();
    }

    /// <summary>
    /// Initialize references to external systems.
    /// </summary>
    protected virtual void InitializeReferences()
    {
        locationManager = LocationManager.Instance;
        if (locationManager == null)
        {
            Debug.LogWarning($"{animatronicName}: LocationManager not found! Waypoint system will not work.");
        }

        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
    }

    protected virtual void Update()
    {
        UpdateState();
        UpdateWaypointDisplay();
    }

    /// <summary>
    /// Updates the waypoint name display in the Inspector
    /// </summary>
    private void UpdateWaypointDisplay()
    {
        if (currentWaypoint != null)
        {
            currentWaypointName = currentWaypoint.GetWaypointName();
        }
        else
        {
            currentWaypointName = "None";
        }
    }

    protected virtual void UpdateState()
    {
        // Don't update if game is over or paused
        if (gameManager != null && (gameManager.currentState == GameState.GameOver || gameManager.currentState == GameState.NightComplete))
        {
            return;
        }

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
        if (isMovingToWaypoint && currentWaypoint != null)
        {
            // Teleport instantly to waypoint position
            transform.position = currentWaypoint.GetPosition();
            string waypointName = currentWaypoint.GetWaypointName();
            isMovingToWaypoint = false;
            moveTimer = 0f;
            currentState = AnimatronicState.Idle;
            Debug.Log($"{animatronicName} teleported to waypoint: {waypointName}");

            // Check if animatronic entered the office (game over condition)
            CheckOfficeEntry(waypointName);
        }
        else
        {
            // No waypoint to move to, just finish moving
            isMovingToWaypoint = false;
            moveTimer = 0f;
            currentState = AnimatronicState.Idle;
        }
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
        ReleaseCurrentWaypoint();
        currentState = AnimatronicState.Idle;
        moveTimer = 0f;
        nextMoveTime = Random.Range(minMoveDelay, maxMoveDelay);
        isMovingToWaypoint = false;
    }

    /// <summary>
    /// Release the current waypoint when changing locations or resetting.
    /// </summary>
    protected virtual void ReleaseCurrentWaypoint()
    {
        if (currentWaypoint != null)
        {
            currentWaypoint.Release();
            currentWaypoint = null;
            currentWaypointName = "None"; // Update display field
        }
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

    public string GetAnimatronicName()
    {
        return animatronicName;
    }

    /// <summary>
    /// Get the current waypoint this animatronic is occupying.
    /// </summary>
    public LocationWaypoint GetCurrentWaypoint()
    {
        return currentWaypoint;
    }

    /// <summary>
    /// Try to occupy a waypoint and teleport to it instantly.
    /// Returns true if successful.
    /// </summary>
    protected virtual bool TryOccupyWaypoint(LocationWaypoint waypoint)
    {
        if (waypoint == null)
        {
            return false;
        }

        // Release current waypoint if moving to a new one
        if (currentWaypoint != null && currentWaypoint != waypoint)
        {
            ReleaseCurrentWaypoint();
        }

        // Try to occupy the new waypoint
        if (waypoint.TryOccupy(this))
        {
            currentWaypoint = waypoint;
            string waypointName = waypoint.GetWaypointName();
            currentWaypointName = waypointName; // Update display field

            // Teleport instantly to waypoint position
            transform.position = waypoint.GetPosition();
            isMovingToWaypoint = false;
            currentState = AnimatronicState.Idle;

            // Check if animatronic entered the office (game over condition)
            CheckOfficeEntry(waypointName);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if the animatronic entered the office and trigger game over if so.
    /// </summary>
    protected virtual void CheckOfficeEntry(string waypointName)
    {
        // Check if waypoint name is "Office" (case-insensitive)
        if (string.Equals(waypointName, "Office", System.StringComparison.OrdinalIgnoreCase))
        {
            // Only trigger if game is still playing (prevent multiple triggers)
            if (gameManager != null && gameManager.currentState == GameState.Playing)
            {
                Debug.LogWarning($"{animatronicName} entered the office! Game Over!");

                // Trigger jumpscare and game over
                gameManager.TriggerJumpscare(animatronicName);
            }
        }
    }
}
