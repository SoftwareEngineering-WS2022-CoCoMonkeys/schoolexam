using System;

namespace SchoolExam.Core.Domain.SubmissionAggregate
{
    public class TextInput : Input
    {
        public string Text { get; set; }
        
        public TextInput(Guid id, string text) : base(id)
        {
            Text = text;
        }
    }
}