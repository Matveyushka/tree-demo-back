namespace Structuralist.M1;

class SemanticErrorsException : Exception
{
    public SemanticErrorsException(List<string> errors) : base(CreateExceptionMessage(errors))
    {
    }

    public SemanticErrorsException(string message)
        : base(message)
    {
    }

    public SemanticErrorsException(string message, Exception inner)
        : base(message, inner)
    {
    }

    private static string CreateExceptionMessage(List<string> errors)
    {
        var message = $"M1 compilation error. There are {errors.Count} semantic error{(errors.Count > 1 ? "s" : "")}:";
        errors.ForEach(error => message += $"{Environment.NewLine}{error}");
        return message;
    }
}