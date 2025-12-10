using HomeNetCore.Data.Enums;
namespace HomeNetCore.Services.UsersServices
{
    public class ValidationResult
    {
        public TypeField Field { get; set; }
        public ValidationState State { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
