using FluentValidation.Results;

namespace Ecommerce.Application.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public IDictionary<string, string[]> errors { get; }
        public ValidationException() : base("Errors occurred from validation")
        {
            errors= new Dictionary<string, string[]>(); 
        }

        public ValidationException(IEnumerable<ValidationFailure> failures):this() {
            errors = failures.GroupBy(e => e.PropertyName, e => e.ErrorMessage).ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }
    }
}
