using UnityEngine;

/// <summary>
/// Bare bones animatronic template - completely self-contained.
/// Copy this file and rename it to create a new animatronic.
/// </summary>
public class AnimatronicTemplate : AnimatronicBase
{
    [Header("Template Specific Settings")]
    [SerializeField] private float customValue = 1f;

    protected override void Start()
    {
        base.Start();
        animatronicName = "Template";
        currentLocation = AnimatronicLocation.StartingPosition;
    }

    protected override void MoveToNextLocation()
    {
        // TODO: Implement movement logic here
        // Example: Move to next location in sequence

        Debug.Log($"{animatronicName} moving from {currentLocation}");

        // Simple example: cycle through locations
        int locationCount = System.Enum.GetValues(typeof(AnimatronicLocation)).Length;
        int currentIndex = (int)currentLocation;
        int nextIndex = (currentIndex + 1) % locationCount;

        currentLocation = (AnimatronicLocation)nextIndex;
        currentState = AnimatronicState.Moving;

        moveTimer = 0f;
        nextMoveTime = Random.Range(minMoveDelay, maxMoveDelay);
    }

    protected override void UpdateMoving()
    {
        // Customize movement behavior here
        base.UpdateMoving();

        Debug.Log($"{animatronicName} arrived at {currentLocation}");
    }

    protected override void UpdateAttacking()
    {
        base.UpdateAttacking();

        // Customize attack behavior here
        Debug.Log($"{animatronicName} performs attack!");
    }

    public override void ResetPosition()
    {
        base.ResetPosition();
        currentLocation = AnimatronicLocation.StartingPosition;
    }
}

