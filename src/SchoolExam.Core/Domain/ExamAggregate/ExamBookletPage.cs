using System;
using SchoolExam.SharedKernel;

namespace SchoolExam.Core.Domain.ExamAggregate
{
    public class ExamBookletPage : EntityBase<Guid>
    {
        public int Page { get; set; }
        public Guid ExamBookletId { get; set; }
        public byte[] QrCode { get; set; }
        
        protected ExamBookletPage(Guid id) : base(id)
        {
        }

        public ExamBookletPage(Guid id, int page, Guid examBookletId, byte[] qrCode) : this(id)
        {
            Page = page;
            ExamBookletId = examBookletId;
            QrCode = qrCode;
        }
    }
}