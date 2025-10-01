namespace VC_SL.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}