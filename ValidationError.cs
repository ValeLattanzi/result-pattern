namespace result_pattern;

public sealed record ValidationError(string Code, string Description, Error[] Errors)
	: Error(Code, Description, ErrorType.Validation);