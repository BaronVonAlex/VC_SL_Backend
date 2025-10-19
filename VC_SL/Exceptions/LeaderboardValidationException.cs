namespace VC_SL.Exceptions;

public class LeaderboardValidationException(Dictionary<string, List<string>> errors)
    : ValidationException(errors);