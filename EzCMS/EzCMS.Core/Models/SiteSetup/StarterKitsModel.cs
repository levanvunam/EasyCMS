using Ez.Framework.Configurations;
using Ez.Framework.Utilities.Reflection;
using EzCMS.Core.Framework.StarterKits.Attributes;
using EzCMS.Core.Framework.StarterKits.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace EzCMS.Core.Models.SiteSetup
{
    public class StarterKitsModel
    {
        public StarterKitsModel()
        {
            StarterKits = typeof(IStarterKit).GetAllImplementTypesOf(FrameworkConstants.EzSolution)
                .Select(s =>
                new StarterKitModel
                {
                    Name = s.GetAttribute<StarterKitAttribute>().Name,
                    Value = s.FullName,
                    Description = s.GetAttribute<StarterKitAttribute>().Description,
                    ImageName = string.Format("/Images/StarterKits/{0}", s.GetAttribute<StarterKitAttribute>().ImageName)
                }).ToList();
        }

        #region Public Properties
        public List<StarterKitModel> StarterKits { get; set; }
        #endregion
    }

    public class StarterKitModel
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }

        public string ImageName { get; set; }
    }
}