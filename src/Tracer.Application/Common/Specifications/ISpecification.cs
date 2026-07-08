using System.Linq.Expressions;

namespace Tracer.Application.Common.Specifications;

/// <summary>
/// Ardalis-style specification (query criteria + includes + ordering + paging) encapsulated as
/// a composable object. Keeps filtering/paging out of fat repository methods.
/// (User-requested "Specification" deliverable; aligns with the repository abstraction in Doc 10 §4.3.)
/// </summary>
public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }

    int? Skip { get; }
    int? Take { get; }
    bool IsPagingEnabled { get; }

    /// <summary>When true, the evaluator emits AsNoTracking() (read-only query paths).</summary>
    bool AsNoTracking { get; }
}
