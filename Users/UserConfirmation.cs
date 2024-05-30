using Microsoft.AspNetCore.Identity;

namespace ArchiBase.Users;

public class UserConfirmation : IUserConfirmation<ArchiBaseUser>
{
    public Task<bool> IsConfirmedAsync(UserManager<ArchiBaseUser> manager, ArchiBaseUser user) =>
        Task.FromResult(user.IsEnabled);
}