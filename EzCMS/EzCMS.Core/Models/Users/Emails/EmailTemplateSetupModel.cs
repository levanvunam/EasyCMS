namespace EzCMS.Core.Models.Users.Emails
{
    public abstract class EmailTemplateSetupModel<T> where T : class
    {
        public abstract T GetMockData();
    }
}
