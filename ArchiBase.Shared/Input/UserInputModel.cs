namespace ArchiBase.Shared.Input;

public class UserRegisterInputModel
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public Guid? LocationId { get; set; }

    public string? Bio { get; set; }
}

public class UserLoginInputModel
{
    public string UserName { get; set; }
    public string Password { get; set; }

    public string? TwoFactorCode { get; set; }
    public string? TwoFactorRecoveryCode { get; set; }
}