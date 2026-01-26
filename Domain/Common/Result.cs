using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public class Result<T>
    {
        public T Value { get; }
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string? ErrorMessage { get; }

        private Result(T value, bool isSuccess, string? errorMessage)
        {
            if (isSuccess && !string.IsNullOrEmpty(errorMessage))
                throw new InvalidOperationException();

            if (!isSuccess && string.IsNullOrEmpty(errorMessage))
                throw new InvalidOperationException();

            Value = value;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static Result<T> Success(T value)
            => new Result<T>(value, true, null);

        public static Result<T> Failure(string errorMessage)
            => new Result<T>(default!, false, errorMessage);
    }

}
