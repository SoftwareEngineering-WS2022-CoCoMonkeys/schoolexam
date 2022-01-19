using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SchoolExam.Application.TagLayout;

namespace SchoolExam.ModuleTests.Application;

[TestFixture]
public class NoGapTagLayoutTest
{
    private class TestNoGapTagLayout : NoGapTagLayout<TestNoGapTagLayout>
    {
        public override int Rows => 3;
        public override int Columns => 2;
        public override TagSize TagSize => new(30, 20);
        public override PageSize PageSize => new(80, 100);
        public override float Padding => 5;
    }

    [Test]
    public void NoGapTagLayout_GetElements_Success()
    {
        var gapLayout = new TestNoGapTagLayout();
        var elements = gapLayout.GetElements();

        var expectedElements = new List<TagLayoutElement>
        {
            new(20, 10, 30, 20),
            new(20, 40, 30, 20),
            new(40, 10, 30, 20),
            new(40, 40, 30, 20),
            new(60, 10, 30, 20),
            new(60, 40, 30, 20)
        };

        elements.Should().BeEquivalentTo(expectedElements);
    }
}