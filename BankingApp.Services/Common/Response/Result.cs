namespace BankingApp.Services.Common.Response
{   
    public class Result<T>
    {
        private Result(bool isSuccess, Error error, T? data)
        {
            ValidateArguments(isSuccess, error);
            IsSuccess = isSuccess;
            Error = error;
            Data = data;
        }
        
        private Result(bool isSuccess, Error error)
        {
            ValidateArguments(isSuccess, error);
            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }

        public Error Error { get; }

        public T? Data { get; }

        public bool IsError => !IsSuccess;

        public static Result<T> Success(T data) => new(true, Error.None, data);

        public static Result<T> Success() => new(true, Error.None);

        public static Result<T> Failure(Error error) => new(false, error);

        private static void ValidateArguments(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
            {
                throw new ArgumentException("Invalid error state. Success results must have no errors, and failure results must have an error.", nameof(error));
            }
        }
    }
}
