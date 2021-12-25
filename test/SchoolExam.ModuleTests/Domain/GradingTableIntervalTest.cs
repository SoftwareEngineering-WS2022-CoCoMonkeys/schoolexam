using FluentAssertions;
using NUnit.Framework;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.ModuleTests.Domain
{
    public class GradingTableIntervalTest
    {
        [Test]
        public void GradingTableInterval_Overlaps_OverlappingIntervals_ReturnsTrue()
        {
            // setup first interval
            var start1 = new GradingTableIntervalBound(40, GradingTableIntervalBoundType.Inclusive);
            var end1 = new GradingTableIntervalBound(45, GradingTableIntervalBoundType.Exclusive);
            var interval1 = new GradingTableInterval(start1, end1, 2.0);
            
            // setup second interval
            var start2 = new GradingTableIntervalBound(42, GradingTableIntervalBoundType.Inclusive);
            var end2 = new GradingTableIntervalBound(47, GradingTableIntervalBoundType.Exclusive);
            var interval2 = new GradingTableInterval(start2, end2, 1.7);

            var result = interval1.Overlaps(interval2);

            result.Should().BeTrue();
        }
    }
}