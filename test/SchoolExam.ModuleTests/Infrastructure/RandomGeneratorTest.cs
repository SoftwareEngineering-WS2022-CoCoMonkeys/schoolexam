using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SchoolExam.Infrastructure.RandomGenerator;

namespace SchoolExam.ModuleTests.Infrastructure;

public class RandomGeneratorTest
{
    [Test]
    [TestCase(true, "AAAA")]
    [TestCase(false, "aaaa")]
    public void RandomGenerator_GenerateHexString_Success(bool caps, string expected)
    {
        var randomMock = new Mock<Random>();
        int hexDigitCount = 16, length = 4;
        randomMock.Setup(x => x.Next(hexDigitCount)).Returns(10);
        
        var randomGenerator = new RandomGenerator(randomMock.Object);
        var result = randomGenerator.GenerateHexString(length, caps);
        
        randomMock.Verify(x => x.Next(hexDigitCount), Times.Exactly(length));

        result.Should().BeEquivalentTo(expected);
    }
    
    [Test]
    public void RandomGenerator_GenerateHexString_NoCaps_Success()
    {
        var randomMock = new Mock<Random>();
        int hexDigitCount = 16, length = -1;
        randomMock.Setup(x => x.Next(hexDigitCount)).Returns(10);
        
        var randomGenerator = new RandomGenerator(randomMock.Object);
        var action = () => randomGenerator.GenerateHexString(length, true);

        action.Should().ThrowExactly<ArgumentException>().WithMessage("Length of result cannot be negative.");
    }
}