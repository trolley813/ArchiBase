namespace ArchiBase.Utils;

class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}

class StateContainer
{
    private ILogger<StateContainer> logger;
    private readonly Guid containerGuid = Guid.NewGuid();

    public StateContainer(ILogger<StateContainer> logger)
    {
        this.logger = logger;
    }
    private readonly Dictionary<Guid, LoginModel> data = [];

    public LoginModel? this[Guid key]
    {
        get
        {
            logger.LogInformation("Container {containerGuid}: Retrieving key {key}", containerGuid, key);
            data.Remove(key, out LoginModel? result);
            return result;
        }
        set
        {
            logger.LogInformation("Container {containerGuid}: Setting key {key}", containerGuid, key);
            if (value is not null) data[key] = value;
        }
    }
}