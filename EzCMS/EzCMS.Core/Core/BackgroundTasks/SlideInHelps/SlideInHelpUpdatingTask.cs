using Ez.Framework.Core.BackgroundTasks.Base;
using Ez.Framework.Core.BackgroundTasks.Models;
using Ez.Framework.Core.Logging;
using Ez.Framework.Utilities.Web;
using Ez.Framework.IoC;
using EzCMS.Core.Core.SiteSettings.ComplexSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Models.SlideInHelps.HelpServices;
using EzCMS.Core.Services.Languages;
using EzCMS.Core.Services.SiteSettings;
using EzCMS.Core.Services.SlideInHelps;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web.Routing;
using Ez.Framework.Core.IoC;

namespace EzCMS.Core.Core.BackgroundTasks.SlideInHelps
{
    public class SlideInHelpUpdatingTask : IBackgroundTask
    {
        private static int _hasActiveTask;
        public void Run(BackgroundTaskExecuteContext context)
        {
            var logger = HostContainer.GetInstance<ILogger>();
            if (Interlocked.CompareExchange(ref _hasActiveTask, 1, 0) == 0)
            {
                try
                {
                    logger.Info(string.Format("[{0}] Start slide in help updating task", EzCMSContants.SlideInHelpUpdatingTaskName));
                    var backgroundTaskService = HostContainer.GetInstance<IEzBackgroundTaskService>();
                    var task = backgroundTaskService.GetByType(GetType());
                    var lastRunningTime = task.LastRunningTime;

                    // Update the background task last running time
                    backgroundTaskService.UpdateLastRunningTimeTask(GetType());

                    var languageService = HostContainer.GetInstance<ILanguageService>();
                    var slideInHelpService = HostContainer.GetInstance<ISlideInHelpService>();
                    var siteSettingService = HostContainer.GetInstance<ISiteSettingService>();
                    var slideInHelpSetting = siteSettingService.LoadSetting<EzCMSHelpConfigurationSetting>();

                    if (!slideInHelpSetting.DisabledSlideInHelpService &&
                        !string.IsNullOrEmpty(slideInHelpSetting.HelpServiceUrl))
                    {
                        var languages = languageService.GetAll().ToList();
                        var slideInHelps = WebUtilities.SendApiRequest<List<SlideInHelpResponseModel>>(
                            slideInHelpSetting.HelpServiceUrl, slideInHelpSetting.AuthorizeCode,
                            "api/Helps/LoadSlideInHelps",
                            HttpMethod.Get, new RouteValueDictionary(new
                            {
                                lastUpdatingTime = lastRunningTime
                            }));

                        if (slideInHelps.Any())
                        {
                            var currentSlideInHelps = slideInHelpService.GetAll().ToList();

                            foreach (var item in slideInHelps)
                            {
                                var slideInHelp =
                                    currentSlideInHelps.FirstOrDefault(
                                        s => s.Language.Key.Equals(item.Language) && s.TextKey.Equals(item.TextKey));

                                if (slideInHelp != null)
                                {
                                    slideInHelp.HelpTitle = item.HelpTitle;
                                    slideInHelp.MasterHelpContent = item.HelpContent;
                                    slideInHelp.MasterVersion = item.Version;

                                    slideInHelpService.Update(slideInHelp);
                                }
                                else
                                {
                                    var language = languages.FirstOrDefault(l => l.Key.Equals(item.Language));
                                    if (language != null)
                                    {
                                        slideInHelp = new SlideInHelp
                                        {
                                            TextKey = item.TextKey,
                                            LanguageId = language.Id,
                                            LocalHelpContent = string.Empty,
                                            LocalVersion = 0,
                                            HelpTitle = item.HelpTitle,
                                            MasterHelpContent = item.HelpContent,
                                            MasterVersion = item.Version,
                                        };

                                        slideInHelpService.Insert(slideInHelp);
                                    }
                                }
                            }

                            slideInHelpService.RefreshDictionary();
                        }
                    }
                }
                catch (Exception exception)
                {
                    logger.Error(string.Format("[{0}]", EzCMSContants.SlideInHelpUpdatingTaskName), exception);
                }
                logger.Info(string.Format("[{0}] End slide in help updating task", EzCMSContants.SlideInHelpUpdatingTaskName));
                Interlocked.Exchange(ref _hasActiveTask, 0);
            }

        }
    }
}