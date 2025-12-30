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
    }

    protected override void MoveToNextLocation()
    {
        // TODO: Implement Freddy's movement logic here
        string currentWaypointName = currentWaypoint != null ? currentWaypoint.GetWaypointName() : null;
        Debug.Log($"{animatronicName} moving from waypoint: {(currentWaypointName ?? "None")}");

        if (locationManager == null)
        {
            Debug.LogWarning($"{animatronicName}: LocationManager not available. Cannot move.");
            return;
        }

        // Simple example: get any available waypoint
        LocationWaypoint waypoint = locationManager.GetAvailableWaypoint(animatronicName);
        if (waypoint != null)
        {
            if (TryOccupyWaypoint(waypoint))
            {
                moveTimer = 0f;
                nextMoveTime = Random.Range(minMoveDelay, maxMoveDelay);
            }
        }
    }

    public override void ResetPosition()
    {
        base.ResetPosition();
        // Try to find starting waypoint if needed
        if (locationManager != null && currentWaypoint == null)
        {
            LocationWaypoint startWaypoint = locationManager.GetAvailableWaypoint(animatronicName, "ShowStage");
            if (startWaypoint != null)
            {
                TryOccupyWaypoint(startWaypoint);
            }
        }
    }
}
