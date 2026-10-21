using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class RacingLineTests
{
    private readonly List<RacingLine> builtLines = new();

    [TearDown]
    public void TearDown()
    {
        foreach (RacingLine line in builtLines) line.Destroy();
        builtLines.Clear();
    }

    // Waypoints every 5 units along +x, two in each checkpoint segment.
    private static List<NavigationWaypointData> StraightLineData(float z = 0f, int count = 6)
    {
        List<NavigationWaypointData> data = new();
        for (int i = 0; i < count; i++)
        {
            data.Add(new NavigationWaypointData(new Vector3(i * 5f, 0f, z), i / 2));
        }
        return data;
    }

    private static Vector3? ThreeCheckpoints(int checkpointID)
    {
        return checkpointID < 3 ? new Vector3(checkpointID * 100f, 0f, 0f) : null;
    }

    private RacingLine Build(int index, List<NavigationWaypointData> data)
    {
        RacingLine line = RacingLine.Build(index, data, ThreeCheckpoints);
        builtLines.Add(line);
        return line;
    }

    [Test]
    public void Build_LinksWaypointsInOrderAndLoops()
    {
        RacingLine line = Build(0, StraightLineData());

        Assert.AreEqual(6, line.Waypoints.Count);
        Assert.AreSame(line.Waypoints[0], line.Head);
        for (int i = 0; i < line.Waypoints.Count; i++)
        {
            NavigationWaypoint expectedNext = line.Waypoints[(i + 1) % line.Waypoints.Count];
            Assert.AreEqual(1, line.Waypoints[i].Next.Length);
            Assert.AreSame(expectedNext, line.Waypoints[i].Next[0]);
        }
    }

    [Test]
    public void Build_StoresLineIndexAndCheckpointID()
    {
        RacingLine line = Build(3, StraightLineData());

        Assert.AreEqual(3, line.Index);
        CollectionAssert.AreEqual(new[] { 0, 0, 1, 1, 2, 2 }, ToCheckpointIDs(line.Waypoints));
        foreach (NavigationWaypoint waypoint in line.Waypoints) Assert.AreEqual(3, waypoint.lineIndex);
    }

    [Test]
    public void Build_CheckpointWaypointsRejoinAfterEachSegmentStart()
    {
        RacingLine line = Build(0, StraightLineData());

        Assert.AreEqual(3, line.CheckpointWaypoints.Count);
        Assert.AreSame(line.Waypoints[1], line.GetCheckpointWaypoint(0).Next[0]);
        Assert.AreSame(line.Waypoints[3], line.GetCheckpointWaypoint(1).Next[0]);
        Assert.AreSame(line.Waypoints[5], line.GetCheckpointWaypoint(2).Next[0]);
        Assert.AreEqual(new Vector3(100f, 0f, 0f), line.GetCheckpointWaypoint(1).position);
    }

    [Test]
    public void Build_SkipsCheckpointWaypointsForMissingCheckpoints()
    {
        List<NavigationWaypointData> data = StraightLineData();
        data.Add(new NavigationWaypointData(new Vector3(30f, 0f, 0f), 3));  // crossing into checkpoint 3, which doesn't exist

        RacingLine line = Build(0, data);

        Assert.AreEqual(3, line.CheckpointWaypoints.Count);
        Assert.IsNull(line.GetCheckpointWaypoint(3));
    }

    [Test]
    public void Build_WithNoData_HasNoHead()
    {
        Assert.IsNull(Build(0, new List<NavigationWaypointData>()).Head);
        Assert.IsNull(Build(0, null).Head);
    }

    [Test]
    public void ToWaypointData_RoundTripsPositionsAndCheckpointIDs()
    {
        List<NavigationWaypointData> data = StraightLineData();

        List<NavigationWaypointData> saved = Build(0, data).ToWaypointData();

        Assert.AreEqual(data.Count, saved.Count);
        for (int i = 0; i < data.Count; i++)
        {
            Assert.AreEqual(data[i].Position, saved[i].Position);
            Assert.AreEqual(data[i].CheckpointID, saved[i].CheckpointID);
        }
    }

    // Line A runs along z = 0 and line B along z = 5, both heading in +x with a waypoint every 5 units.
    private RacingLine[] ParallelLines(int countB = 8)
    {
        return new[] { Build(0, StraightLineData()), Build(1, StraightLineData(z: 5f, count: countB)) };
    }

    [Test]
    public void Link_KeepsSameLineSuccessorFirst()
    {
        RacingLine[] lines = ParallelLines();

        RacingLineLinker.Link(lines, 90f, 1000f);

        foreach (RacingLine line in lines)
        {
            for (int i = 0; i < line.Waypoints.Count; i++)
            {
                Assert.AreSame(line.Waypoints[(i + 1) % line.Waypoints.Count], line.Waypoints[i].Next[0]);
            }
        }
    }

    [Test]
    public void Link_ConnectsToTheFirstWaypointWithinMaxAngle()
    {
        RacingLine[] lines = ParallelLines();

        // From A0 at (0, 0, 0): B1 at (5, 0, 5) is 45 degrees off, B2 at (10, 0, 5) is 26.6 degrees and B3 at (15, 0, 5) is 18.4 degrees.
        RacingLineLinker.Link(lines, 30f, 30f);

        Assert.AreEqual(2, lines[0].Waypoints[0].Next.Length);
        Assert.AreSame(lines[1].Waypoints[2], lines[0].Waypoints[0].Next[1]);
    }

    [Test]
    public void Link_LinksInBothDirections()
    {
        RacingLine[] lines = ParallelLines(countB: 6);

        RacingLineLinker.Link(lines, 30f, 30f);

        Assert.AreSame(lines[0].Waypoints[2], lines[1].Waypoints[0].Next[1]);
    }

    [Test]
    public void Link_NoLinkWhenNothingIsWithinMaxAngleBeforeMaxDistance()
    {
        RacingLine[] lines = ParallelLines();

        // B1 is within 10 units but 45 degrees off, and B2 is 11.2 units away.
        RacingLineLinker.Link(lines, 30f, 10f);

        Assert.AreEqual(1, lines[0].Waypoints[0].Next.Length);
    }

    [Test]
    public void Link_AnchorMustBeInTheSameCheckpointSegment()
    {
        RacingLine lineA = Build(0, StraightLineData());
        List<NavigationWaypointData> otherSegmentData = StraightLineData(z: 5f, count: 8);
        for (int i = 0; i < otherSegmentData.Count; i++)
        {
            otherSegmentData[i] = new NavigationWaypointData(otherSegmentData[i].Position, 9);
        }
        RacingLine lineB = Build(1, otherSegmentData);

        RacingLineLinker.Link(new[] { lineA, lineB }, 90f, 1000f);

        foreach (NavigationWaypoint waypoint in lineA.Waypoints) Assert.AreEqual(1, waypoint.Next.Length);
    }

    [Test]
    public void Link_SkipsDuplicateWaypointsWhenFindingTheDirectionOfTravel()
    {
        // A1 and A2 are at the same position, like the double recorded finish line crossing.
        List<NavigationWaypointData> dataA = new();
        foreach (float x in new[] { 0f, 5f, 5f, 10f, 15f, 20f }) dataA.Add(new NavigationWaypointData(new Vector3(x, 0f, 0f), 0));
        List<NavigationWaypointData> dataB = new();
        for (int i = 0; i < 8; i++) dataB.Add(new NavigationWaypointData(new Vector3(i * 5f, 0f, 5f), 0));
        RacingLine lineA = Build(0, dataA);
        RacingLine lineB = Build(1, dataB);

        // A1's direction of travel comes from A3, so from (5, 0, 0): B2 is 45 degrees off and B3 is 26.6 degrees.
        RacingLineLinker.Link(new[] { lineA, lineB }, 30f, 30f);

        Assert.AreSame(lineB.Waypoints[3], lineA.Waypoints[1].Next[1]);
    }

    [Test]
    public void Link_RelinkingDoesNotDuplicateLinks()
    {
        RacingLine[] lines = ParallelLines();

        RacingLineLinker.Link(lines, 30f, 30f);
        RacingLineLinker.Link(lines, 30f, 30f);

        Assert.AreEqual(2, lines[0].Waypoints[0].Next.Length);
    }

    [Test]
    public void Link_DoesNotLinkCheckpointWaypoints()
    {
        RacingLine[] lines = ParallelLines();

        RacingLineLinker.Link(lines, 90f, 1000f);

        foreach (NavigationWaypoint waypoint in lines[0].CheckpointWaypoints) Assert.AreEqual(1, waypoint.Next.Length);
    }

    private static List<int> ToCheckpointIDs(IReadOnlyList<NavigationWaypoint> waypoints)
    {
        List<int> ids = new();
        foreach (NavigationWaypoint waypoint in waypoints) ids.Add(waypoint.checkpointID);
        return ids;
    }
}
