namespace PAWS.Api.Validation
{
    public class ValidationError
    {
        public string Field { get; set; } = string.Empty;
        public string Issue { get; set; } = string.Empty;
    }

    public class ErrorResponse
    {
        public ErrorBody Error { get; set; } = new();
    }

    public class ErrorBody
    {
        public string Code { get; set; } = "VALIDATION_ERROR";
        public string Message { get; set; } = string.Empty;
        public List<ValidationError> Details { get; set; } = new();
    }

    public static class ErrorResponses
    {
        public static ErrorResponse Validation(string message, params ValidationError[] details)
        {
            return new ErrorResponse
            {
                Error = new ErrorBody
                {
                    Code = "VALIDATION_ERROR",
                    Message = message,
                    Details = details.ToList()
                }
            };
        }

        public static ErrorResponse Forbidden(string message = "You do not have permission to perform this action.")
        {
            return new ErrorResponse
            {
                Error = new ErrorBody
                {
                    Code = "FORBIDDEN",
                    Message = message
                }
            };
        }
    }
}
