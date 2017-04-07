using Ez.Framework.Core.Mvc.Helpers;
using EzCMS.Entity.Entities.Models;
using System;

namespace EzCMS.Core.Models.SlideInHelps
{
    public class SlideInHelpDictionaryItem
    {
        #region Constructors

        public SlideInHelpDictionaryItem()
        {

        }

        public SlideInHelpDictionaryItem(SlideInHelp slideInHelp)
            : this()
        {
            Id = slideInHelp.Id;
            Language = slideInHelp.Language.Key;
            TextKey = slideInHelp.TextKey;
            Key = ServiceHelper.BuildKey(slideInHelp.Language.Key, slideInHelp.TextKey);
            HelpTitle = slideInHelp.HelpTitle;
            MasterHelpContent = slideInHelp.MasterHelpContent;
            LocalHelpContent = slideInHelp.LocalHelpContent;
            LocalVersion = slideInHelp.LocalVersion;
            MasterVersion = slideInHelp.MasterVersion;
            Active = slideInHelp.Active;
            LastUpdate = slideInHelp.LastUpdate ?? slideInHelp.Created;
        }

        public SlideInHelpDictionaryItem(SlideInHelpModel slideInHelp)
            : this()
        {
            Id = slideInHelp.Id;
            Language = slideInHelp.Language;
            TextKey = slideInHelp.TextKey;
            Key = ServiceHelper.BuildKey(slideInHelp.Language, slideInHelp.TextKey);
            HelpTitle = slideInHelp.HelpTitle;
            MasterHelpContent = slideInHelp.MasterHelpContent;
            LocalHelpContent = slideInHelp.LocalHelpContent;
            LocalVersion = slideInHelp.LocalVersion;
            MasterVersion = slideInHelp.MasterVersion;
            Active = slideInHelp.Active;
            LastUpdate = slideInHelp.LastUpdate ?? slideInHelp.Created;
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        public string Language { get; set; }

        public string Key { get; set; }

        public string TextKey { get; set; }

        public string HelpTitle { get; set; }

        public string LocalHelpContent { get; set; }

        public string MasterHelpContent { get; set; }

        public int LocalVersion { get; set; }

        public int MasterVersion { get; set; }

        public bool Active { get; set; }

        public DateTime LastUpdate { get; set; }

        #endregion
    }
}