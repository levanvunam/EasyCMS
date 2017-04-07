namespace EzCMS.Core.Models.LocalizedResources
{
    public  class LocalizeDictionaryItem
    {
        public string Language { get; set; }

        public string Key { get; set; }

        public bool HasClientSetup { get; set; }

        public string Value { get; set; }
    }
}
