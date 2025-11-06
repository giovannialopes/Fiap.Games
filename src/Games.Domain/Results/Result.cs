namespace Games.Domain.Results;

public struct Result<T>
{
    private Error _error;
    private T _value;

    public Error Error {
        get {
            if (IsSuccess)
                throw new InvalidOperationException("Não há erro em um resultado de sucesso.");
            return _error;
        }
        private set => _error = value;
    }

    public bool IsSuccess { get; private set; }

    public T Value {
        get {
            if (!IsSuccess)
                throw new InvalidOperationException("Não há valor em um resultado de falha.");
            return _value;
        }
        private set => _value = value;
    }

    internal Result(bool isSuccess, Error error, T value) {
        IsSuccess = isSuccess;
        _error = error;
        _value = value;
    }
}

public static class Result
{
    public static Result<T> Success<T>(T value)
        => new(true, default, value);

    public static Result<T> Failure<T>(string errorMessage, string code)
        => new(false, new Error(errorMessage, code), default);

}
