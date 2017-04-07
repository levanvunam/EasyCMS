namespace EzCMS.Core.Models.Contacts
{
    public class ContactStatisticModel
    {
        #region Public Properties

        public int TotalContacts { get; set; }

        public int NewContacts { get; set; }

        public int ExistedContacts { get; set; }

        public ContactSearchDetailsModel ContactSearchDetailsModel { get; set; }

        public string ContactNotificationSearchPartial { get; set; }

        #endregion
    }
}
