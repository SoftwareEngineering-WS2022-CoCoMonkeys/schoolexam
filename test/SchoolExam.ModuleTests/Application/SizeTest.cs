using System;
using FluentAssertions;
using NUnit.Framework;
using SchoolExam.Application.TagLayout;

namespace SchoolExam.ModuleTests.Application;

[TestFixture]
public class SizeTest
{
    [Test]
    public void Size_Constructor_NegativeWidth_ThrowsException()
    {
        Action action = () =>
        {
            var x = new Size(-1.0f, 12.0f);
        };

        action.Should().Throw<ArgumentException>().WithMessage("Width and height of size must be positive.");
    }
    
    [Test]
    public void Size_Constructor_NegativeHeight_ThrowsException()
    {
        Action action = () =>
        {
            var x = new Size(4.0f, -17.0f);
        };

        action.Should().Throw<ArgumentException>().WithMessage("Width and height of size must be positive.");
    }
}