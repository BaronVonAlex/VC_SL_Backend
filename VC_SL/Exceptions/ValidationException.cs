namespace VC_SL.Exceptions;

public abstract class ValidationException(Dictionary<string, List<string>> errors) : Exception("Validation failed")
{
    public Dictionary<string, List<string>> Errors { get; } = errors;
}