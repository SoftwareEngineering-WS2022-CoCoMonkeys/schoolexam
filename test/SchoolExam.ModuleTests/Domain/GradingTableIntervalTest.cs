using System;
using FluentAssertions;
using NUnit.Framework;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.ModuleTests.Domain;

public class GradingTableIntervalTest
{
    [Test]
    public void GradingTableInterval_Overlaps_OverlappingIntervals_ReturnsTrue()
    {
        // setup first interval
        var start1 = new GradingTableIntervalBound(40, GradingTableIntervalBoundType.Inclusive);
        var end1 = new GradingTableIntervalBound(45, GradingTableIntervalBoundType.Exclusive);
        var interval1 = new GradingTableInterval(start1, end1, "2.0", GradingTableLowerBoundType.Points, Guid.Empty);
            
        // setup second interval
        var start2 = new GradingTableIntervalBound(42, GradingTableIntervalBoundType.Inclusive);
        var end2 = new GradingTableIntervalBound(47, GradingTableIntervalBoundType.Exclusive);
        var interval2 = new GradingTableInterval(start2, end2, "1.7", GradingTableLowerBoundType.Points, Guid.Empty);

        var result = interval1.Overlaps(interval2);

        result.Should().BeTrue();
    }
    
    [Test]
    public void GradingTableInterval_Overlaps_AdjacentIntervals_ReturnsFalse()
    {
        // setup first interval
        var start1 = new GradingTableIntervalBound(40, GradingTableIntervalBoundType.Inclusive);
        var end1 = new GradingTableIntervalBound(45, GradingTableIntervalBoundType.Exclusive);
        var interval1 = new GradingTableInterval(start1, end1, "2.0", GradingTableLowerBoundType.Points, Guid.Empty);
            
        // setup second interval
        var start2 = new GradingTableIntervalBound(45, GradingTableIntervalBoundType.Inclusive);
        var end2 = new GradingTableIntervalBound(47, GradingTableIntervalBoundType.Exclusive);
        var interval2 = new GradingTableInterval(start2, end2, "1.7", GradingTableLowerBoundType.Points, Guid.Empty);

        var result = interval1.Overlaps(interval2);

        result.Should().BeFalse();
    }
    
    [Test]
    public void GradingTableInterval_Overlaps_NonAdjacentAndOverlappingIntervals_ReturnsFalse()
    {
        // setup first interval
        var start1 = new GradingTableIntervalBound(40, GradingTableIntervalBoundType.Inclusive);
        var end1 = new GradingTableIntervalBound(45, GradingTableIntervalBoundType.Exclusive);
        var interval1 = new GradingTableInterval(start1, end1, "2.0", GradingTableLowerBoundType.Points, Guid.Empty);
            
        // setup second interval
        var start2 = new GradingTableIntervalBound(47, GradingTableIntervalBoundType.Inclusive);
        var end2 = new GradingTableIntervalBound(50, GradingTableIntervalBoundType.Exclusive);
        var interval2 = new GradingTableInterval(start2, end2, "1.7", GradingTableLowerBoundType.Points, Guid.Empty);

        var result = interval1.Overlaps(interval2);

        result.Should().BeFalse();
    }
    
    [Test]
    public void GradingTableInterval_Includes_PointsInInterval_ReturnsTrue()
    {
        // setup interval
        var start = new GradingTableIntervalBound(40, GradingTableIntervalBoundType.Inclusive);
        var end = new GradingTableIntervalBound(45, GradingTableIntervalBoundType.Exclusive);
        var interval = new GradingTableInterval(start, end, "2.0", GradingTableLowerBoundType.Points, Guid.Empty);
        var points = 42;

        var result = interval.Includes(points);

        result.Should().BeTrue();
    }
    
    [Test]
    public void GradingTableInterval_Includes_PointsOutsideInterval_ReturnsFalse()
    {
        // setup interval
        var start = new GradingTableIntervalBound(40, GradingTableIntervalBoundType.Inclusive);
        var end = new GradingTableIntervalBound(45, GradingTableIntervalBoundType.Exclusive);
        var interval = new GradingTableInterval(start, end, "2.0", GradingTableLowerBoundType.Points, Guid.Empty);
        var points = 42;

        var result = interval.Includes(points);

        result.Should().BeTrue();
    }
    
    [Test]
    public void GradingTableInterval_Includes_PointsAtExclusiveLowerBound_ReturnsFalse()
    {
        // setup interval
        var start = new GradingTableIntervalBound(40, GradingTableIntervalBoundType.Exclusive);
        var end = new GradingTableIntervalBound(45, GradingTableIntervalBoundType.Inclusive);
        var interval1 = new GradingTableInterval(start, end, "2.0", GradingTableLowerBoundType.Points, Guid.Empty);
        var points = 40;

        var result = interval1.Includes(points);

        result.Should().BeFalse();
    }
    
    [Test]
    public void GradingTableInterval_Includes_PointsAtExclusiveUpperBound_ReturnsFalse()
    {
        // setup interval
        var start = new GradingTableIntervalBound(40, GradingTableIntervalBoundType.Inclusive);
        var end = new GradingTableIntervalBound(45, GradingTableIntervalBoundType.Exclusive);
        var interval = new GradingTableInterval(start, end, "2.0", GradingTableLowerBoundType.Points, Guid.Empty);
        var points = 45;

        var result = interval.Includes(points);

        result.Should().BeFalse();
    }
    
    [Test]
    public void GradingTableInterval_Includes_PointsAtInclusiveLowerBound_ReturnsFalse()
    {
        // setup interval
        var start = new GradingTableIntervalBound(40, GradingTableIntervalBoundType.Inclusive);
        var end = new GradingTableIntervalBound(45, GradingTableIntervalBoundType.Exclusive);
        var interval1 = new GradingTableInterval(start, end, "2.0", GradingTableLowerBoundType.Points, Guid.Empty);
        var points = 40;

        var result = interval1.Includes(points);

        result.Should().BeTrue();
    }
    
    [Test]
    public void GradingTableInterval_Includes_PointsAtInclusiveUpperBound_ReturnsFalse()
    {
        // setup interval
        var start = new GradingTableIntervalBound(40, GradingTableIntervalBoundType.Exclusive);
        var end = new GradingTableIntervalBound(45, GradingTableIntervalBoundType.Inclusive);
        var interval = new GradingTableInterval(start, end, "2.0", GradingTableLowerBoundType.Points, Guid.Empty);
        var points = 45;

        var result = interval.Includes(points);

        result.Should().BeTrue();
    }
}