namespace EzCMS.Entity.Core.Enums
{
    public class EmailEnums
    {
        public enum EmailPriority
        {
            Low = 1,
            Medium = 2,
            High = 3
        }

        public enum EmailTemplateType
        {
            ForgotPassword = 1,
            RegisterUser = 2,
            UserCreatedNotification = 7,
            ResetPassword = 3,
            ContactForm = 4,
            ProtectedDocumentForm = 5,
            SubscribeNotification = 6,
            AccountExpiredNotification = 8,
            DeactivateAccountNotification = 9
        }
    }
}
