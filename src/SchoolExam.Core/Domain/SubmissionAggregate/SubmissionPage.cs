using System;
using SchoolExam.Util;

namespace SchoolExam.Core.Domain.SubmissionAggregate
{
    public class SubmissionPage : EntityBase<Guid>
    {
        public int Page { get; set; }
        public byte[] ScanData { get; set; }
        
        protected SubmissionPage(Guid id) : base(id)
        {
        }

        public SubmissionPage(Guid id, int page, byte[] scanData) : this(id)
        {
            Page = page;
            ScanData = scanData;
        }
    }
}