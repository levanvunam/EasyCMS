namespace EzCMS.Core.Models.Users.Remotes
{
    public class UserLoginResult
    {
        public bool IsValid { get; set; }

        public string Error { get; set; }

        public RemoteUser User { get; set; }
    }
}