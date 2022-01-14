using System.Linq.Expressions;
using SchoolExam.Extensions;

namespace SchoolExam.Application.Specifications;

public class EntitySpecification<TEntity> : ISpecification<TEntity> where TEntity : class
{
    private readonly ICollection<Expression<Func<TEntity, object?>>> _includes;
    private readonly ICollection<string> _includeStrings;

    public Expression<Func<TEntity, bool>> Criteria { get; }
    public IReadOnlyCollection<Expression<Func<TEntity, object?>>> Includes => _includes.AsReadOnly();
    public IReadOnlyCollection<string> IncludeStrings => _includeStrings.AsReadOnly();

    public EntitySpecification() : this(_ => true)
    {
    }

    public EntitySpecification(Expression<Func<TEntity, bool>> criteria)
    {
        Criteria = criteria;
        _includes = new List<Expression<Func<TEntity, object?>>>();
        _includeStrings = new List<string>();
    }

    protected void AddInclude(Expression<Func<TEntity, object?>> includeExpression)
    {
        _includes.Add(includeExpression);
    }

    protected void AddInclude(string includeString)
    {
        _includeStrings.Add(includeString);
    }
}