namespace Games.Domain.Entities;

public class ILoggerEnt
{

    public int Id { get; private set; }

    public string Message { get; private set; }

    public string Error { get; private set; }

    public DateTime DataCriacao { get; private set; }

    public string Hostname { get; private set; }

    public int CodeIdentify { get; private set; }

    private ILoggerEnt() { }


    private ILoggerEnt(string message, string error, int codeIdentify) {
        Message = message;
        Error = error;
        DataCriacao = DateTime.UtcNow;
        Hostname = Environment.MachineName;
        CodeIdentify = codeIdentify;
    }

    public static ILoggerEnt Criar(string message, string error, int codeIdentify) {
        return new ILoggerEnt(message, error, codeIdentify);
    }
}

