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
    }

    protected override void MoveToNextLocation()
    {
        // TODO: Implement movement logic here
        // Example: Move to next waypoint

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

    protected override void UpdateMoving()
    {
        // Customize movement behavior here
        base.UpdateMoving();

        string currentWaypointName = currentWaypoint != null ? currentWaypoint.GetWaypointName() : null;
        Debug.Log($"{animatronicName} arrived at waypoint: {(currentWaypointName ?? "Unknown")}");
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
        // Try to find starting waypoint if needed
        if (locationManager != null && currentWaypoint == null)
        {
            LocationWaypoint startWaypoint = locationManager.GetAvailableWaypoint(animatronicName);
            if (startWaypoint != null)
            {
                TryOccupyWaypoint(startWaypoint);
            }
        }
    }
}

