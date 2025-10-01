namespace VC_SL.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}