using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Connects racing lines so racers can switch between them without swerving.
/// </summary>
public static class RacingLineLinker
{
    private const float MinSqrDistance = 0.0001f;

    /// <summary>
    /// Rebuilds all links between racing lines. For each waypoint, the nearest waypoint in the same checkpoint segment on
    /// each other line is used as an anchor, then the other line is followed forward from there. The waypoint links to
    /// the first one whose direction is within maxLinkAngle of the waypoint's own direction of travel, so switching
    /// lines is a gradual merge. No link is made if that waypoint would be further than maxLinkDistance away.
    /// </summary>
    public static void Link(IReadOnlyList<RacingLine> lines, float maxLinkAngle, float maxLinkDistance)
    {
        foreach (RacingLine line in lines)
        {
            line.ResetLinks();
        }

        float sqrMaxLinkDistance = maxLinkDistance * maxLinkDistance;

        foreach (RacingLine line in lines)
        {
            for (int i = 0; i < line.Waypoints.Count; i++)
            {
                NavigationWaypoint waypoint = line.Waypoints[i];
                Vector3? forward = GetForward(line, i);
                if (forward == null) continue;

                foreach (RacingLine otherLine in lines)
                {
                    if (otherLine == line) continue;

                    int anchorIndex = FindNearestIndex(waypoint, otherLine);
                    if (anchorIndex < 0) continue;

                    NavigationWaypoint target = FindTarget(waypoint, forward.Value, otherLine, anchorIndex, maxLinkAngle, sqrMaxLinkDistance);
                    if (target != null) waypoint.AddNext(target);
                }
            }
        }
    }

    /// <summary>
    /// Direction of travel from a waypoint, skipping any following waypoints recorded at the same position.
    /// </summary>
    private static Vector3? GetForward(RacingLine line, int index)
    {
        Vector3 position = line.Waypoints[index].position;
        for (int step = 1; step < line.Waypoints.Count; step++)
        {
            Vector3 forward = line.Waypoints[(index + step) % line.Waypoints.Count].position - position;
            if (forward.sqrMagnitude > MinSqrDistance) return forward;
        }

        return null;
    }

    private static int FindNearestIndex(NavigationWaypoint waypoint, RacingLine otherLine)
    {
        int nearestIndex = -1;
        float nearestSqrDistance = float.MaxValue;

        for (int i = 0; i < otherLine.Waypoints.Count; i++)
        {
            NavigationWaypoint candidate = otherLine.Waypoints[i];
            if (candidate.checkpointID != waypoint.checkpointID) continue;

            float sqrDistance = (candidate.position - waypoint.position).sqrMagnitude;
            if (sqrDistance >= nearestSqrDistance) continue;

            nearestIndex = i;
            nearestSqrDistance = sqrDistance;
        }

        return nearestIndex;
    }

    private static NavigationWaypoint FindTarget(NavigationWaypoint waypoint, Vector3 forward, RacingLine otherLine, int anchorIndex, float maxLinkAngle, float sqrMaxLinkDistance)
    {
        for (int step = 1; step < otherLine.Waypoints.Count; step++)
        {
            NavigationWaypoint candidate = otherLine.Waypoints[(anchorIndex + step) % otherLine.Waypoints.Count];
            Vector3 toCandidate = candidate.position - waypoint.position;

            float sqrDistance = toCandidate.sqrMagnitude;
            if (sqrDistance > sqrMaxLinkDistance) return null;
            if (sqrDistance <= MinSqrDistance) continue;

            if (Vector3.Angle(forward, toCandidate) <= maxLinkAngle) return candidate;
        }

        return null;
    }
}
