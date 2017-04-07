using System;
using EzCMS.Entity.Core.Enums;

namespace EzCMS.Core.Models.Users.ExtendExpirationDate
{
    public class ExtendExpirationDateResponseModel
    {
        #region Public Properties

        public UserEnums.ExtendExpirationDateResponseCode ResponseCode { get; set; }

        public string UserName { get; set; }

        public DateTime? ExpirationDate { get; set; }

        #endregion
          
    }
}
