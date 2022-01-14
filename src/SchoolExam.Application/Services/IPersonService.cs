using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Application.Services;

public interface IPersonService
{
    Person GetById(Guid id);
}