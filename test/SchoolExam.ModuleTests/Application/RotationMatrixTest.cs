using FluentAssertions;
using NUnit.Framework;
using SchoolExam.Application.Pdf;

namespace SchoolExam.ModuleTests.Application;

[TestFixture]
public class RotationMatrixTest
{
    [Test]
    public void RotationMatrix_Constructor_FromAngle_Success()
    {
        var expectedRotationMatrix = new RotationMatrix(1, 0, 0, 1);
        var rotationMatrix = new RotationMatrix(0);

        rotationMatrix.Should().BeEquivalentTo(expectedRotationMatrix);
    }

    [Test]
    public void RotationMatrix_TransformX_Success()
    {
        float x = 2.0f, y = 3.0f;
        var rotationMatrix = new RotationMatrix(1, 1, 1, 1);
        var result = rotationMatrix.TransformX(x, y);
        result.Should().Be(5);
    }
    
    [Test]
    public void RotationMatrix_TransformY_Success()
    {
        float x = 5.0f, y = 27.0f;
        var rotationMatrix = new RotationMatrix(1, 1, 1, 1);
        var result = rotationMatrix.TransformY(x, y);
        result.Should().Be(32);
    }

    [Test]
    public void RotationMatrix_Multiplication_Success()
    {
        var a = new RotationMatrix(1, 2, 3, 4);
        var b = new RotationMatrix(5, 6, 7, 8);
        
        var result = a * b;
        
        var expectedRotationMatrix = new RotationMatrix(19, 22, 43, 50);
        result.Should().BeEquivalentTo(expectedRotationMatrix);
    }
}