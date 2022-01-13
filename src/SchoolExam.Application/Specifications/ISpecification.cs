using System.Linq.Expressions;

namespace SchoolExam.Application.Specifications;

public interface ISpecification<TEntity> where TEntity : class
{
    Expression<Func<TEntity, bool>> Criteria { get; }
    IReadOnlyCollection<Expression<Func<TEntity, object?>>> Includes { get; }
    IReadOnlyCollection<string> IncludeStrings { get; }
}