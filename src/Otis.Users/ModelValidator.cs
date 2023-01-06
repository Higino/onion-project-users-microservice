using System.ComponentModel.DataAnnotations;

namespace Otis.Users;

public static class ModelValidator
{
    public static bool IsValid(object obj, out IEnumerable<ValidationResult> errors)
    {
        if (obj is null)
        {
            errors = new List<ValidationResult> { new ValidationResult("Input is null") };
            return false;
        }

        ValidationContext validationContext = new(obj);
        var validationResults = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(obj, validationContext, validationResults, validateAllProperties: true);

        errors = isValid ? Enumerable.Empty<ValidationResult>() : validationResults;

        return isValid;
    }
}