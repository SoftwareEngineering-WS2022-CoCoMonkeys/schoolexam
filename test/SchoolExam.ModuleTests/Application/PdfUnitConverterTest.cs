using FluentAssertions;
using NUnit.Framework;
using SchoolExam.Application.TagLayout;

namespace SchoolExam.ModuleTests.Application;

[TestFixture]
public class PdfUnitConverterTest
{
    private const float Tolerance = 1e-6f;
    
    [Test]
    public void PdfUnitConverter_ConvertPointToInch_Success()
    {
        var point = 1.0f;
        var expectedResult = 0.0138888889f;

        var result = PdfUnitConverter.ConvertPointToInch(point);

        (result - expectedResult).Should().BeLessThan(Tolerance);
    }
    
    [Test]
    public void PdfUnitConverter_ConvertInchToPoint_Success()
    {
        var inch = 1.0f;
        var expectedResult = 72.0f;

        var result = PdfUnitConverter.ConvertInchToPoint(inch);

        (result - expectedResult).Should().BeLessThan(Tolerance);
    }
    
    [Test]
    public void PdfUnitConverter_ConvertPointToMm_Success()
    {
        var point = 1.0f;
        var expectedResult = 0.352778;

        var result = PdfUnitConverter.ConvertPointToMm(point);

        (result - expectedResult).Should().BeLessThan(Tolerance);
    }
    
    [Test]
    public void PdfUnitConverter_ConvertMmToPoint_Success()
    {
        var mm = 1.0f;
        var expectedResult = 2.83464567f;

        var result = PdfUnitConverter.ConvertMmToPoint(mm);

        (result - expectedResult).Should().BeLessThan(Tolerance);
    }
}