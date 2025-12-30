using UnityEngine;

/// <summary>
/// Bare bones Bonnie template - self-contained with no external dependencies.
/// </summary>
public class BonnieAI : AnimatronicBase
{
    protected override void Start()
    {
        base.Start();
        animatronicName = "Bonnie";
    }

    protected override void MoveToNextLocation()
    {
        if (locationManager == null)
        {
            Debug.LogWarning($"{animatronicName}: LocationManager not available. Cannot move.");
            return;
        }

        string currentWaypointName = currentWaypoint != null ? currentWaypoint.GetWaypointName() : null;
        Debug.Log($"{animatronicName} attempting to move from waypoint: {(currentWaypointName ?? "None")}");

        // Define Bonnie's movement path by waypoint names (can be customized in Unity Inspector)
        // These should match the waypoint names you set in the scene
        string[] bonniePath = new string[]
        {
            "ShowStage",
            "DiningArea",
            "WestHall",
            "WestHallCorner",
            "SupplyCloset",
            "LeftDoor",
            "Office"
        };

        // Try to find the next location in Bonnie's path
        string targetWaypointName = null;
        int currentIndex = -1;

        if (!string.IsNullOrEmpty(currentWaypointName))
        {
            currentIndex = System.Array.IndexOf(bonniePath, currentWaypointName);
        }

        if (currentIndex >= 0 && currentIndex < bonniePath.Length - 1)
        {
            // Move to next location in path
            targetWaypointName = bonniePath[currentIndex + 1];
        }
        else if (currentIndex < 0)
        {
            // Not in path, start from beginning
            targetWaypointName = bonniePath[0];
        }
        else
        {
            // At end of path, try to find any available waypoint
            // This allows Bonnie to explore other locations if path is blocked
            LocationWaypoint availableWaypoint = locationManager.GetAvailableWaypoint(animatronicName);
            if (availableWaypoint != null)
            {
                targetWaypointName = availableWaypoint.GetWaypointName();
            }
        }

        // Try to get an available waypoint for the target location
        if (!string.IsNullOrEmpty(targetWaypointName))
        {
            LocationWaypoint waypoint = locationManager.GetAvailableWaypoint(animatronicName, targetWaypointName);

            if (waypoint != null)
            {
                if (TryOccupyWaypoint(waypoint))
                {
                    Debug.Log($"{animatronicName} moving to waypoint: {targetWaypointName}");
                    moveTimer = 0f;
                    nextMoveTime = Random.Range(minMoveDelay, maxMoveDelay);
                }
                else
                {
                    Debug.LogWarning($"{animatronicName} could not occupy waypoint: {targetWaypointName}");
                    // Try to find any available waypoint as fallback
                    LocationWaypoint fallbackWaypoint = locationManager.GetAvailableWaypoint(animatronicName);
                    if (fallbackWaypoint != null)
                    {
                        TryOccupyWaypoint(fallbackWaypoint);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"{animatronicName} could not find available waypoint: {targetWaypointName}");
            }
        }
        else
        {
            Debug.LogWarning($"{animatronicName} could not determine next location");
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
