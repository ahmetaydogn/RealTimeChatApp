using Core.Entities;

namespace Core.CrossCuttingConcers.Validation
{
    public abstract class BaseValidator<T> : IValidator<T> where T : class, new()
    {
        protected abstract void SetupRules();

        internal readonly List<Func<T, string?>> validationRules = new List<Func<T, string?>>();

        protected BaseValidator()
        {
            SetupRules();
        }


        protected void AddRule(Func<T, string?> rule)
        {
            validationRules.Add(rule);
        }

        public ValidationResult Validate(T entity)
        {
            ValidationResult validationResult = new ValidationResult();

            foreach (var rule in validationRules)
            {
                // If the rule is valid, it will return null, otherwise it will return the error message
                string? errorMessage = rule.Invoke(entity);

                // If the error message is null or empty, it means the rule is valid,
                // so we don't need to add it to the validation result
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    validationResult.AddError(errorMessage);
                }
            }

            return validationResult;
        }
    }
}
