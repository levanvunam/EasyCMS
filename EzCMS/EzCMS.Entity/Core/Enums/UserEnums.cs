namespace EzCMS.Entity.Core.Enums
{
    public class UserEnums
    {
        public enum UserStatus
        {
            Active = 1,
            Disabled = 2
        }

        public enum Gender
        {
            Male = 1,
            Female = 2,
            None = 3
        }

        public enum ExtendExpirationDateResponseCode
        {
            UserNotFound = 1,
            InvalidUrl = 2,
            NotActive = 3,
            NoNeedToExtend = 4,
            Success = 5,
            Fail = 6
        }

    }
}
