namespace Games.Domain.Results;

public class Error
{
    public string Message { get; }
    public string Code { get; }

    public Error(string message, string code = "GENERIC_ERROR") {
        Message = message;
        Code = code;
    }

    public override string ToString() => $"{Code}: {Message}";
}
