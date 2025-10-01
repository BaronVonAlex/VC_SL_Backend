namespace VC_SL.Exceptions;

public class ValidationException : Exception
{
    public Dictionary<string, List<string>> Errors { get; }

    public ValidationException(Dictionary<string, List<string>> errors)
        : base("Validation failed")
    {
        Errors = errors;
    }
}