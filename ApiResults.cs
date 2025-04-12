using System.Text.Json;

namespace result_pattern;

public static class ApiResults {
  public static ProblemDetail problem(Result result) {
    if (result.IsSuccess)
      throw new InvalidOperationException();
    var problemDetails = new ProblemDetail {
      Title = getTitle(result.Error),
      Detail = getDetail(result.Error),
      Type = getType(result.Error.Type),
      Status = getStatusCode(result.Error)
    };

    var errors = getErrors(result);
    if (errors is null) return problemDetails;
    foreach (var (key, value) in errors)
      problemDetails.Detail += ("errors", JsonSerializer.Serialize((key, value)));
    return problemDetails;
  }

  private static string getTitle(Error error) {
    return error.Type switch {
      ErrorType.BadRequest => error.Code,
      ErrorType.Validation => error.Code,
      ErrorType.NotFound => error.Code,
      ErrorType.Conflict => error.Code,
      ErrorType.Unauthorized => error.Code,
      ErrorType.Forbidden => error.Code,
      _ => "Internal Server Error"
    };
  }

  private static string getDetail(Error error) {
    var descriptionHeader = error.Type switch {
      ErrorType.BadRequest => error.Description,
      ErrorType.Validation => error.Description,
      ErrorType.NotFound => error.Description,
      ErrorType.Conflict => error.Description,
      ErrorType.Unauthorized => error.Description,
      ErrorType.Forbidden => error.Description,
      _ => "An error occurred while processing your request."
    };
    return descriptionHeader;
  }

  private static string getType(ErrorType type) {
    return type switch {
      ErrorType.BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
      ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
      ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
      ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
      ErrorType.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
      ErrorType.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
      _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
    };
  }

  private static int getStatusCode(Error error) {
    return error.Type switch {
      ErrorType.BadRequest => 400,
      ErrorType.Validation => 400,
      ErrorType.NotFound => 404,
      ErrorType.Conflict => 409,
      ErrorType.Unauthorized => 401,
      ErrorType.Forbidden => 403,
      _ => 500
    };
  }

  private static Dictionary<string, object>? getErrors(Result result) {
    if (result.Error is not ValidationError validationError) return null;

    return new() {
      { "errors", validationError.Errors.ToArray() }
    };
  }
}