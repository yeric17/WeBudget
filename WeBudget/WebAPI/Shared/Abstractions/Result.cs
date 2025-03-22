﻿namespace WebAPI.Shared.Abstractions;

public class Result
{

    protected internal Result(bool isSuccess,Error mainError)
    {
        if (isSuccess && mainError != Error.None)
        {
            throw new InvalidOperationException();
        }

        if (!isSuccess && mainError == Error.None )
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = mainError;
        Errors = [];
    }

    protected internal Result(bool isSuccess, List<Error> errors)
    {
        if (isSuccess &&  errors.Count > 0)
        {
            throw new InvalidOperationException();
        }

        if (!isSuccess && errors.Count == 0)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = errors.First();
        Errors = errors;
    }


    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; protected set; }
    public List<Error> Errors { get; protected set; }
    public static Result Success() => new(true, Error.None);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    public static Result Failure(Error error) => new(false, error);
    public static Result Failure(List<Error> errors) => new(false, errors);

    public static implicit operator Result(List<Error> errors) =>
    new(false,errors);

    public static implicit operator Result(Error error) =>
    new(false, error);

    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
    public static Result<TValue> Failure<TValue>(List<Error> errors) => new(default, false, errors);

    public static Result<TValue> Create<TValue>(TValue? value) => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}
