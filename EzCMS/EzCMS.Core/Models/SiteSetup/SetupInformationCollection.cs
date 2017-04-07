using EzCMS.Entity.Core.SiteInitialize;

namespace EzCMS.Core.Models.SiteSetup
{
    public class SetupInformationCollection
    {
        public SiteConfiguration SiteConfiguration { get; set; }

        public UserSetupModel InitialUser { get; set; }

        public string StarterKit { get; set; }
    }
}