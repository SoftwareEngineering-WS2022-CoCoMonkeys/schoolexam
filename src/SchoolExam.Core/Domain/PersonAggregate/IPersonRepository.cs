namespace SchoolExam.Core.Domain.PersonAggregate;

public interface IPersonRepository
{
    Person GetById(Guid id);
}