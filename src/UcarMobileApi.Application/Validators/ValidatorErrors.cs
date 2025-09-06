namespace UcarMobileApi.Application.Validators;

public static class ValidatorErrors
{
    public const string IsRequired = "{PropertyName} is required.";
    public const string NoEmpty = "{PropertyName} cannot be empty.";
    public const string Duplicated = "There is already a value registered for {PropertyName}.";
    public const string MinLenght = "{{PropertyName}} must be {0} character min.";
    public const string MaxLengthExceeded = "{{PropertyName}} must be {0} character max.";
    public const string LengthMismatch = "{{PropertyName}} must be {0} character.";
    public const string GreaterThanZero = "{{PropertyName}} must be greater than zero.";
    public const string MustBeNonNegative = "{{PropertyName}} must be zero or greater.";
    public const string InvalidFileExtension = "Invalid file extension.";
    public const string InvalidValue = "{{PropertyName}} has an invalid value.";
    public const string DateInFuture = "{{PropertyName}} greater than the current date.";
}
