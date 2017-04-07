using Ez.Framework.Core.BackgroundTasks;
using Quartz;
using System;
using System.Net.Http;

namespace EzCMS.Web.Areas.BackgroundTasks.Models
{
    public class ProxyTask : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            var proxyContext = dataMap.Get(BackgroundTaskManager.ProxyContextKey) as ProxyTaskExecuteContext;

            var client = new HttpClient();

            if (proxyContext != null)
            {
                var baseUri = proxyContext.Url.IsDefaultPort
                    ? string.Format("{0}://{1}/", proxyContext.Url.Scheme,
                        proxyContext.Url.Host)
                    : string.Format("{0}://{1}:{2}/", proxyContext.Url.Scheme,
                        proxyContext.Url.Host, proxyContext.Url.Port);

                client.BaseAddress = new Uri(baseUri);

                var entryUrl = "api/BackgroundEntryPoint/ExcuteTask?taskKey=" + proxyContext.TaskKey;

                var result = client.PostAsJsonAsync(entryUrl, new { }).Result;
                var message = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}