using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Component attached to GameObjects in scene to mark waypoint positions.
/// Defines which animatronics can use this waypoint and tracks occupancy.
/// </summary>
public class LocationWaypoint : MonoBehaviour
{
    [Header("Waypoint Settings")]
    [SerializeField] private string waypointName = "Waypoint"; // Optional name for identification
    [SerializeField] private bool allowAllAnimatronics = true; // If true, all animatronics can use this waypoint
    [SerializeField] private string[] allowedAnimatronicNames = new string[0];

    [Header("Visual Settings")]
    [SerializeField] private Color gizmoColor = Color.yellow;
    [SerializeField] private bool showLabel = true;
    [SerializeField] private float gizmoSize = 0.5f;

    private AnimatronicBase currentOccupant = null;

    /// <summary>
    /// Check if a specific animatronic is allowed to use this waypoint.
    /// </summary>
    public bool CanAnimatronicUse(string animatronicName)
    {
        // If "Allow All" is enabled, everyone can use it
        if (allowAllAnimatronics)
        {
            return true;
        }

        // If no specific names are set, deny access (unless allowAllAnimatronics is true)
        if (allowedAnimatronicNames == null || allowedAnimatronicNames.Length == 0)
        {
            return false;
        }

        // Check if given animatronicName is allowed
        foreach (string allowedName in allowedAnimatronicNames)
        {
            if (string.Equals(allowedName, animatronicName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Check if this waypoint is currently available (not occupied).
    /// </summary>
    public bool IsAvailable()
    {
        return currentOccupant == null;
    }

    /// <summary>
    /// Attempt to occupy this waypoint. Returns true if successful.
    /// </summary>
    public bool TryOccupy(AnimatronicBase animatronic)
    {
        if (currentOccupant != null)
        {
            return false; // Already occupied
        }

        if (animatronic == null)
        {
            Debug.LogWarning($"LocationWaypoint: Attempted to occupy with null animatronic at {waypointName}");
            return false;
        }

        if (!CanAnimatronicUse(animatronic.GetAnimatronicName()))
        {
            Debug.LogWarning($"LocationWaypoint: {animatronic.GetAnimatronicName()} is not allowed to use waypoint {waypointName}");
            return false;
        }

        currentOccupant = animatronic;
        return true;
    }

    /// <summary>
    /// Release this waypoint when an animatronic leaves.
    /// </summary>
    public void Release()
    {
        if (currentOccupant != null)
        {
            currentOccupant = null;
        }
    }

    /// <summary>
    /// Get the name of this waypoint.
    /// </summary>
    public string GetWaypointName()
    {
        return waypointName;
    }

    /// <summary>
    /// Get the world position of this waypoint.
    /// </summary>
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Get the current occupant of this waypoint (null if available).
    /// </summary>
    public AnimatronicBase GetCurrentOccupant()
    {
        return currentOccupant;
    }

    /// <summary>
    /// Force release if a specific animatronic is occupying (for cleanup).
    /// </summary>
    public void ForceRelease(AnimatronicBase animatronic)
    {
        if (currentOccupant == animatronic)
        {
            currentOccupant = null;
        }
    }

    /// <summary>
    /// Draw gizmos in the Scene view for visual feedback.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Draw a colored sphere at the waypoint position
        Gizmos.color = currentOccupant != null ? Color.red : gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);

        // Draw a line to show the "up" direction
        Gizmos.color = gizmoColor;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * gizmoSize * 2f);

        if (showLabel)
        {
            // Draw label with waypoint name and allowed animatronics
            string label = waypointName;
            if (allowedAnimatronicNames != null && allowedAnimatronicNames.Length > 0)
            {
                label += $"\n[{string.Join(", ", allowedAnimatronicNames)}]";
            }
            if (currentOccupant != null)
            {
                label += $"\n[OCCUPIED: {currentOccupant.GetAnimatronicName()}]";
            }

#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * (gizmoSize * 2.5f), label);
#endif
        }
    }

    /// <summary>
    /// Draw selected gizmos (more visible when selected).
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, gizmoSize * 1.5f);
    }
}

