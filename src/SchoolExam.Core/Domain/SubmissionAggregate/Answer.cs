using System;
using SchoolExam.SharedKernel;

namespace SchoolExam.Core.Domain.SubmissionAggregate
{
    public class Answer : Correctable
    {
        public Guid ExamTaskId { get; set; }
        public Guid SubmissionId { get; set; }

        protected Answer(Guid id) : base(id)
        {
        }

        public Answer(Guid id, int? achievedPoints, CorrectableState state, Guid examTaskId, Guid submissionId) : base(
            id, achievedPoints, state)
        {
            ExamTaskId = examTaskId;
            SubmissionId = submissionId;
        }
    }
}