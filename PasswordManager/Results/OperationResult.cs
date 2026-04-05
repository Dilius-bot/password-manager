namespace PasswordManager.Results;

class OperationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";

    public static OperationResult Ok(string message = "Успешно")
        => new OperationResult { Success = true, Message = message};
    public static OperationResult Fail(string message = "Ошибка")
        => new OperationResult { Success = false, Message = message};
}