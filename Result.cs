using System.Diagnostics.CodeAnalysis;

namespace result_pattern;

public class Result
{
	protected Result(bool isSuccess, Error error)
	{
		if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
			throw new ArgumentException("Success result can't have an error.");

		IsSuccess = isSuccess;
		Error = error;
	}

	public Error Error { get; }

	public bool IsSuccess { get; }
	public bool IsFailure => !IsSuccess;

	public static Result success()
	{
		return new(true, Error.None);
	}

	public static Result<T> success<T>(T value)
	{
		return new(value, true, Error.None);
	}

	public static Result failure(Error error)
	{
		return new(false, error);
	}

	public static Result<T> failure<T>(Error error)
	{
		return new(default, false, error);
	}
}

public class Result<T> : Result
{
	private readonly T? _value;

	public Result(T? value, bool isSuccess, Error error) : base(isSuccess, error)
	{
		_value = value;
	}

	[NotNull]
	public T Value => IsSuccess
		? _value!
		: throw new InvalidOperationException("The value of a failure result can't be accessed.");

	public static Result<T> success(T value)
	{
		return new(value, true, Error.None);
	}

	public new static Result<T> failure(Error error)
	{
		return new(default, false, error);
	}

	public static Result<T> validationFailure(Error error)
	{
		return new(default, false, error);
	}

	public static implicit operator Result<T>(T? value)
	{
		return value is null ? failure<T>(Error.NullValue) : success(value);
	}
}