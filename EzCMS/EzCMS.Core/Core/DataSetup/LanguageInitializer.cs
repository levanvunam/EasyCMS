using System.Data.Entity;
using Ez.Framework.Core.Entity.Extensions;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Entity.Intialize;

namespace EzCMS.Core.Core.DataSetup
{
    public class LanguageInitializer : IDataInitializer
    {
        /// <summary>
        /// Priority of initializer
        /// </summary>
        /// <returns></returns>
        public DataInitializerPriority Priority()
        {
            return DataInitializerPriority.High;
        }

        #region Initialize

        /// <summary>
        /// Initialize default languages
        /// </summary>
        public void Initialize(DbContext ezContext)
        {
            var context = ezContext as EzCMSEntities;
            if (context != null)
            {
                #region Add countries

                if (!context.Countries.Any())
                {

                    var countryData = DataInitializeHelper.GetResourceContent("Country.Countries.txt",
                        DataSetupResourceType.Others);
                    List<string> countryList =
                        countryData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    int i = 0;
                    var countries = new List<Country>();
                    foreach (var item in countryList)
                    {
                        i = i + 1;
                        var country = new Country
                        {
                            Name = item,
                            RecordOrder = i * 10
                        };

                        countries.Add(country);
                    }

                    var dbContext = new EzCMSEntities();
                    dbContext.Countries.AddIfNotExist(country => country.Name, countries.ToArray());
                    dbContext.SaveChanges();

                }

                #endregion

                #region Add Languages

                var languages = new[]
                {
                    new Language
                    {
                        Name = "English",
                        Culture = "United States",
                        Key = "en-US",
                        IsDefault = false,
                        RecordOrder = 20
                    }
                };
                context.Languages.AddIfNotExist(l => l.Name, languages);

                #endregion
            }
        }
        #endregion
    }
}
