using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Singleton that manages all waypoints in the scene.
/// Auto-discovers waypoints and provides lookup methods for animatronics.
/// </summary>
public class LocationManager : MonoBehaviour
{
    private static LocationManager instance;

    [Header("Debug Settings")]
    [SerializeField] private bool logWaypointDiscovery = true;

    private List<LocationWaypoint> allWaypoints = new List<LocationWaypoint>();

    public static LocationManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<LocationManager>();
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("LocationManager");
                    instance = managerObject.AddComponent<LocationManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            DiscoverWaypoints();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ensure waypoints are discovered
        if (allWaypoints.Count == 0)
        {
            DiscoverWaypoints();
        }
    }

    /// <summary>
    /// Auto-discover all LocationWaypoint components in the scene.
    /// </summary>
    [ContextMenu("Refresh Waypoints")]
    public void DiscoverWaypoints()
    {
        allWaypoints.Clear();

        LocationWaypoint[] foundWaypoints = FindObjectsByType<LocationWaypoint>(FindObjectsSortMode.None);

        foreach (LocationWaypoint waypoint in foundWaypoints)
        {
            if (waypoint != null)
            {
                allWaypoints.Add(waypoint);
            }
        }

        if (logWaypointDiscovery)
        {
            Debug.Log($"LocationManager: Discovered {allWaypoints.Count} waypoints.");
            foreach (LocationWaypoint waypoint in allWaypoints)
            {
                Debug.Log($"  - {waypoint.GetWaypointName()} (GameObject: {waypoint.name})");
            }
        }
    }

    /// <summary>
    /// Get an available waypoint for a specific animatronic, optionally preferring a waypoint by name.
    /// Returns null if no available waypoint is found.
    /// </summary>
    public LocationWaypoint GetAvailableWaypoint(string animatronicName, string preferredWaypointName = null)
    {
        List<LocationWaypoint> candidates = new List<LocationWaypoint>();

        // If preferred waypoint name is specified, try that first
        if (!string.IsNullOrEmpty(preferredWaypointName))
        {
            foreach (LocationWaypoint waypoint in allWaypoints)
            {
                if (waypoint.GetWaypointName() == preferredWaypointName &&
                    waypoint.CanAnimatronicUse(animatronicName) &&
                    waypoint.IsAvailable())
                {
                    candidates.Add(waypoint);
                }
            }
        }

        // If no preferred waypoint candidates found, search all waypoints
        if (candidates.Count == 0)
        {
            foreach (LocationWaypoint waypoint in allWaypoints)
            {
                if (waypoint.CanAnimatronicUse(animatronicName) && waypoint.IsAvailable())
                {
                    candidates.Add(waypoint);
                }
            }
        }

        // Return first available candidate (or null if none found)
        return candidates.Count > 0 ? candidates[0] : null;
    }

    /// <summary>
    /// Get the position for a waypoint by name for a specific animatronic.
    /// Returns null if no available waypoint is found.
    /// </summary>
    public Vector3? GetWaypointPosition(string waypointName, string animatronicName)
    {
        LocationWaypoint waypoint = GetAvailableWaypoint(animatronicName, waypointName);
        return waypoint != null ? waypoint.GetPosition() : (Vector3?)null;
    }

    /// <summary>
    /// Get all waypoints that a specific animatronic can use.
    /// </summary>
    public List<LocationWaypoint> GetWaypointsForAnimatronic(string animatronicName)
    {
        List<LocationWaypoint> validWaypoints = new List<LocationWaypoint>();

        foreach (LocationWaypoint waypoint in allWaypoints)
        {
            if (waypoint.CanAnimatronicUse(animatronicName))
            {
                validWaypoints.Add(waypoint);
            }
        }

        return validWaypoints;
    }

    /// <summary>
    /// Check if a waypoint exists with the given name for a specific animatronic.
    /// </summary>
    public bool HasWaypoint(string waypointName, string animatronicName)
    {
        foreach (LocationWaypoint waypoint in allWaypoints)
        {
            if (waypoint.GetWaypointName() == waypointName && waypoint.CanAnimatronicUse(animatronicName))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Get all waypoints with a specific name.
    /// </summary>
    public List<LocationWaypoint> GetWaypointsByName(string waypointName)
    {
        List<LocationWaypoint> matchingWaypoints = new List<LocationWaypoint>();
        foreach (LocationWaypoint waypoint in allWaypoints)
        {
            if (waypoint.GetWaypointName() == waypointName)
            {
                matchingWaypoints.Add(waypoint);
            }
        }
        return matchingWaypoints;
    }

    /// <summary>
    /// Get the total count of waypoints.
    /// </summary>
    public int GetWaypointCount()
    {
        return allWaypoints.Count;
    }

    /// <summary>
    /// Force release all waypoints (useful for reset scenarios).
    /// </summary>
    public void ReleaseAllWaypoints()
    {
        foreach (LocationWaypoint waypoint in allWaypoints)
        {
            waypoint.Release();
        }
    }
}

