using Core.Entities;

namespace Core.CrossCuttingConcers.Validation
{
    public interface IValidator<T> where T : class, new()
    {
        ValidationResult Validate(T entity);
    }
}
