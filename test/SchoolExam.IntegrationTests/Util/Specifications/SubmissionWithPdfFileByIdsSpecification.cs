using System;
using System.Collections.Generic;
using SchoolExam.Application.Specifications;
using SchoolExam.Domain.Entities.SubmissionAggregate;

namespace SchoolExam.IntegrationTests.Util.Specifications;

public class SubmissionWithPdfFileByIdsSpecification : EntityByIdsSpecification<Submission>
{
    public SubmissionWithPdfFileByIdsSpecification(HashSet<Guid> ids) : base(ids)
    {
        AddInclude(x => x.PdfFile);
    }
}