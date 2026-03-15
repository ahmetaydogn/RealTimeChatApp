namespace Core.CrossCuttingConcers.Validation
{
    public class ValidationResult
    {
        private readonly List<string> errors = new List<string>();
        public IReadOnlyCollection<string> Errors { get => errors; }

        public bool IsValid => !errors.Any(); // That means if there is no error, then it's valid

        public void AddError(string errorMessage) => errors.Add(errorMessage);
    }
}
