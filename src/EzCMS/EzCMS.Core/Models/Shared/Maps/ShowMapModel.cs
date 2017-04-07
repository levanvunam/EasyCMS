namespace EzCMS.Core.Models.Shared.Maps
{
    public class ShowMapModel
    {
        #region Public Properties

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string FullAddress { get; set; }

        public string FullAddressFromAddressData
        {
            get
            {
                return string.Format("{0}, {1} {2} {3}, {4}", AddressLine, Suburb, State, Postcode, Country);
            }
        }

        public string AddressLine { get; set; }

        public string Suburb { get; set; }

        public string State { get; set; }

        public string Postcode { get; set; }

        public string Country { get; set; }

        #endregion
    }
}
