using System.ComponentModel;

namespace EzCMS.Core.Framework.Enums
{
    public class LoginEnums
    {
        public enum LoginTemplateConfiguration
        {
            [Description("Login")]
            Login = 1,
            [Description("Login Success")]
            LoginSuccess,
            [Description("Change Password After Login")]
            ChangePasswordAfterLogin,

            [Description("Register")]
            Register,
            [Description("Register Success")]
            RegisterSuccess,

            [Description("Forgot Password")]
            ForgotPassword,
            [Description("Forgot Password Success")]
            ForgotPasswordSuccess,

            [Description("Reset Password")]
            ResetPassword,
            [Description("Reset Password Success")]
            ResetPasswordSuccess
        }
    }
}
