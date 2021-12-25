using SchoolExam.Domain.Entities.PersonAggregate;

namespace SchoolExam.Application.Repositories;

public interface IPersonRepository
{
    Person GetById(Guid id);
}