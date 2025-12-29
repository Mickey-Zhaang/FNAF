using UnityEngine;

/// <summary>
/// Bare bones Freddy template - self-contained with no external dependencies.
/// </summary>
public class FreddyAI : AnimatronicBase
{
    protected override void Start()
    {
        base.Start();
        animatronicName = "Freddy";
        currentLocation = AnimatronicLocation.ShowStage;
    }

    protected override void MoveToNextLocation()
    {
        // TODO: Implement Freddy's movement logic here
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

    public override void ResetPosition()
    {
        base.ResetPosition();
        currentLocation = AnimatronicLocation.ShowStage;
    }
}
