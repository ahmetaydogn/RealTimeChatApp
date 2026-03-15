namespace Core.CrossCuttingConcers.Validation
{
    public static class ValidationTool
    {
        public static void Validate<T>(IValidator<T> validator, T entity) where T : class, new()
        {
            ValidationResult validationResult = validator.Validate(entity);

            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(Environment.NewLine, validationResult.Errors);
                throw new ArgumentException($"Validation failed for {typeof(T).Name}:\n{errorMessages}");
            }
            return;
        }
    }
}
