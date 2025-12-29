using UnityEngine;

/// <summary>
/// Bare bones Chica template - self-contained with no external dependencies.
/// </summary>
public class ChicaAI : AnimatronicBase
{
    protected override void Start()
    {
        base.Start();
        animatronicName = "Chica";
        currentLocation = AnimatronicLocation.ShowStage;
    }

    protected override void MoveToNextLocation()
    {
        // TODO: Implement Chica's movement logic here
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
