using System;
using FluentAssertions;
using NUnit.Framework;
using SchoolExam.Domain.Entities.SubmissionAggregate;
using SchoolExam.Domain.Extensions;
using SchoolExam.Domain.ValueObjects;

namespace SchoolExam.ModuleTests.Domain;

[TestFixture]
public class SubmissionExtensionsTest
{
    private Submission _submission = null!;
    
    [SetUp]
    public void SetUp()
    {
        _submission = new Submission(Guid.Empty, Guid.Empty, Guid.Empty, DateTime.MinValue);
    }
    
    [Test]
    public void SubmissionExtensions_GetCorrectionState_NoAnswers_ReturnsPending()
    {
        var result = _submission.GetCorrectionState();
        result.Should().Be(CorrectionState.Pending);
    }
    
    [Test]
    public void SubmissionExtensions_GetCorrectionState_AllAnswersPending_ReturnsPending()
    {
        var count = 5;
        for (int i = 0; i < count; i++)
        {
            var answer = new Answer(Guid.Empty, Guid.Empty, Guid.Empty, AnswerState.Pending, null, DateTime.MinValue);
            _submission.Answers.Add(answer);
        }
        
        var result = _submission.GetCorrectionState();
        
        result.Should().Be(CorrectionState.Pending);
    }
    
    [Test]
    public void SubmissionExtensions_GetCorrectionState_AllButOneAnswersPending_ReturnsInCorrection()
    {
        var count = 5;
        for (int i = 1; i < count; i++)
        {
            var answer = new Answer(Guid.Empty, Guid.Empty, Guid.Empty, AnswerState.Pending, null, DateTime.MinValue);
            _submission.Answers.Add(answer);
        }

        var correctedAnswer =
            new Answer(Guid.Empty, Guid.Empty, Guid.Empty, AnswerState.Corrected, 0, DateTime.MinValue);
        _submission.Answers.Add(correctedAnswer);
        
        var result = _submission.GetCorrectionState();
        
        result.Should().Be(CorrectionState.InProgress);
    }
    
    [Test]
    public void SubmissionExtensions_GetCorrectionState_AllButOneAnswersCorrected_ReturnsInCorrection()
    {
        var count = 5;
        for (int i = 1; i < count; i++)
        {
            var answer = new Answer(Guid.Empty, Guid.Empty, Guid.Empty, AnswerState.Corrected, i, DateTime.MinValue);
            _submission.Answers.Add(answer);
        }

        var pendingAnswer =
            new Answer(Guid.Empty, Guid.Empty, Guid.Empty, AnswerState.Pending, null, DateTime.MinValue);
        _submission.Answers.Add(pendingAnswer);
        
        var result = _submission.GetCorrectionState();
        
        result.Should().Be(CorrectionState.InProgress);
    }
    
    [Test]
    public void SubmissionExtensions_GetCorrectionState_AllAnswersCorrected_ReturnsCorrected()
    {
        var count = 5;
        for (int i = 0; i < count; i++)
        {
            var answer = new Answer(Guid.Empty, Guid.Empty, Guid.Empty, AnswerState.Corrected, null, DateTime.MinValue);
            _submission.Answers.Add(answer);
        }
        
        var result = _submission.GetCorrectionState();
        
        result.Should().Be(CorrectionState.Corrected);
    }
}