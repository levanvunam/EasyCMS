using Ez.Framework.Core.BackgroundTasks.Base;
using Ez.Framework.Core.Context;
using Ez.Framework.Core.Entity.Entities;
using Ez.Framework.Core.Entity.Enums;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Models.BackgroundTasks;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace EzCMS.Core.Services.BackgroundTasks
{
    public class BackgroundTaskService : ServiceHelper, IBackgroundTaskService
    {
        private readonly IRepository<BackgroundTask> _backgroundTaskRepository;

        public BackgroundTaskService(IRepository<BackgroundTask> backgroundTaskRepository)
        {
            _backgroundTaskRepository = backgroundTaskRepository;
        }

        #region Base

        public IQueryable<BackgroundTask> GetAll()
        {
            return _backgroundTaskRepository.GetAll();
        }

        public IQueryable<BackgroundTask> Fetch(Expression<Func<BackgroundTask, bool>> expression)
        {
            return _backgroundTaskRepository.Fetch(expression);
        }

        public BackgroundTask FetchFirst(Expression<Func<BackgroundTask, bool>> expression)
        {
            return _backgroundTaskRepository.FetchFirst(expression);
        }

        public BackgroundTask GetById(object id)
        {
            return _backgroundTaskRepository.GetById(id);
        }

        internal ResponseModel Insert(BackgroundTask backgroundTask)
        {
            return _backgroundTaskRepository.Insert(backgroundTask);
        }

        internal ResponseModel Delete(BackgroundTask backgroundTask)
        {
            return _backgroundTaskRepository.Delete(backgroundTask);
        }

        internal ResponseModel Delete(object id)
        {
            return _backgroundTaskRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _backgroundTaskRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the background tasks
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchBackgroundTasks(JqSearchIn si)
        {
            var data = GetAll();

            var backgroundTasks = Maps(data);

            return si.Search(backgroundTasks);
        }

        /// <summary>
        /// Export background tasks
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var backgroundTasks = Maps(data);

            var exportData = si.Export(backgroundTasks, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="backgroundTasks"></param>
        /// <returns></returns>
        private IQueryable<BackgroundTaskModel> Maps(IQueryable<BackgroundTask> backgroundTasks)
        {
            return backgroundTasks.Select(m => new BackgroundTaskModel
            {
                Id = m.Id,
                Name = m.Name,
                LastRunningTime = m.LastRunningTime,
                Description = m.Description,
                ScheduleType = m.ScheduleType,
                Interval = m.Interval,
                StartTime = m.StartTime,
                Status = m.Status,
                RecordOrder = m.RecordOrder,
                Created = m.Created,
                CreatedBy = m.CreatedBy,
                LastUpdate = m.LastUpdate,
                LastUpdateBy = m.LastUpdateBy
            });
        }

        #endregion

        #region Manage

        /// <summary>
        /// Get BackgroundTask manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BackgroundTaskManageModel GetBackgroundTaskManageModel(int id)
        {
            var backgroundTask = GetById(id);
            if (backgroundTask != null)
            {
                return new BackgroundTaskManageModel(backgroundTask);
            }
            return null;
        }

        /// <summary>
        /// Save BackgroundTask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveBackgroundTask(BackgroundTaskManageModel model)
        {
            var backgroundTask = GetById(model.Id);
            if (backgroundTask != null)
            {
                backgroundTask.Description = model.Description;
                if (backgroundTask.Interval != model.Interval || backgroundTask.Status != model.Status ||
                    backgroundTask.ScheduleType != model.ScheduleType || backgroundTask.StartTime != model.StartTime)
                {
                    backgroundTask.ScheduleType = model.ScheduleType;
                    switch (model.ScheduleType)
                    {
                        case BackgroundTaskEnums.ScheduleType.Daily:
                            backgroundTask.StartTime = model.StartTime;
                            backgroundTask.Interval = null;
                            break;
                        case BackgroundTaskEnums.ScheduleType.Interval:
                            backgroundTask.StartTime = null;
                            backgroundTask.Interval = model.Interval;
                            break;
                    }
                    backgroundTask.Status = model.Status;

                    // Need to restart to apply changes
                    EzWorkContext.IsSystemChanged = true;
                }

                var response = Update(backgroundTask);

                if (!response.Success)
                {
                    EzWorkContext.IsSystemChanged = false;
                }

                return response.SetMessage(response.Success
                    ? T("BackgroundTask_Message_UpdateSuccessfully")
                    : T("BackgroundTask_Message_UpdateFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("BackgroundTask_Message_ObjectNotFound")
            };
        }

        #endregion

        #region Logs

        /// <summary>
        /// Get background task logs
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentDateLog"></param>
        /// <returns></returns>
        public BackgroundTaskLogsModel GetLogs(int id, string currentDateLog)
        {
            var backgroundTask = GetById(id);
            if (backgroundTask != null)
            {
                var logDirectory = HttpContext.Current.Server.MapPath("~/Logs");
                var logsDaysPerView = 1;

                var logs = new List<BackgroundTaskLogsDateModel>();
                var oldestLog =
                    new DirectoryInfo(logDirectory).GetFiles("*.log")
                        .OrderBy(f => f.Name)
                        .FirstOrDefault();
                var loadComplete = false;
                if (oldestLog == null)
                {
                    return null;
                }
                DateTime oldestDate;
                DateTime.TryParse(Path.GetFileNameWithoutExtension(oldestLog.Name).Replace("_", "/"), out oldestDate);
                DateTime dateLog;
                DateTime.TryParse(currentDateLog.Replace("_", "/"), out dateLog);
                if (logsDaysPerView > 0)
                {
                    while (logsDaysPerView > 0)
                    {
                        if (dateLog < oldestDate)
                        {
                            loadComplete = true;
                            break;
                        }
                        var total = 0;
                        var file = Path.Combine(logDirectory, string.Format("{0}.log", dateLog.ToString("yyyy.MM.dd")));
                        if (File.Exists(file))
                        {
                            var contents = File.ReadAllText(file);
                            total =
                                contents.Split(new[] { string.Format("[{0}]", backgroundTask.Name) },
                                    StringSplitOptions.None).Length - 1;
                        }
                        var logDate = new BackgroundTaskLogsDateModel
                        {
                            DateLog = dateLog.ToString("yyyy_MM_dd"),
                            Total = total
                        };
                        logs.Add(logDate);
                        dateLog = dateLog.AddDays(-1);
                        logsDaysPerView--;
                    }
                    if (dateLog < oldestDate)
                    {
                        loadComplete = true;
                    }
                }

                else
                {
                    loadComplete = true;
                    while (dateLog >= oldestDate)
                    {
                        var total = 0;
                        var file = Path.Combine(logDirectory, string.Format("{0}.log", dateLog.ToString("yyyy_MM_dd")));
                        if (File.Exists(file))
                        {
                            total = File.ReadLines(file).Count();
                        }
                        var logDate = new BackgroundTaskLogsDateModel
                        {
                            DateLog = dateLog.ToString("yyyy_MM_dd"),
                            Total = total
                        };
                        logs.Add(logDate);
                        dateLog = dateLog.AddDays(-1);
                    }
                }

                var model = new BackgroundTaskLogsModel
                {
                    Id = backgroundTask.Id,
                    BackgroundTaskName = backgroundTask.Name,
                    LogsDates = logs,
                    LoadComplete = loadComplete,
                    NextDateLog = dateLog.ToString("yyyy_MM_dd")
                };
                return model;
            }
            return null;
        }

        /// <summary>
        /// Get details of background task logs on a specific date
        /// </summary>
        /// <param name="backgroundTaskName"></param>
        /// <param name="dateLogs"></param>
        /// <returns></returns>
        public List<string> GetLogsDetails(string backgroundTaskName, string dateLogs)
        {
            var logDirectory = HttpContext.Current.Server.MapPath("~/Logs");
            var logsInfo = new List<string>();
            var backgroundTaskKey = string.Format("[{0}]", backgroundTaskName);
            if (Directory.Exists(logDirectory))
            {
                foreach (var file in Directory.GetFiles(logDirectory))
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                    if (fileNameWithoutExtension != null)
                    {
                        if (dateLogs.Equals(fileNameWithoutExtension.Replace(".", "_")))
                        {
                            using (var sr = new StreamReader(file))
                            {
                                var previousLine = "";
                                while (sr.Peek() >= 0)
                                {
                                    var line = sr.ReadLine();
                                    DateTime date;
                                    var isCombined = false;
                                    if (logsInfo.Any() && line != null && line.Length > 10 &&
                                        !DateTime.TryParse(line.Substring(0, 10), out date) && previousLine != null &&
                                        previousLine.Contains(backgroundTaskKey))
                                    {
                                        logsInfo[logsInfo.Count() - 1] = string.Format("{0}<br/>{1}",
                                            logsInfo[logsInfo.Count() - 1], line);
                                        isCombined = true;
                                    }
                                    else
                                    {
                                        if (line != null && line.Contains(backgroundTaskKey))
                                        {
                                            logsInfo.Add(line.Replace(backgroundTaskKey, ""));
                                        }
                                    }
                                    if (!isCombined)
                                        previousLine = line;
                                }
                            }
                        }
                    }
                }
            }
            return logsInfo;
        }

        #endregion

        /// <summary>
        /// Update last running time of task
        /// </summary>
        /// <param name="backgroundTaskType"></param>
        public void UpdateLastRunningTimeTask(Type backgroundTaskType)
        {
            var taskSetups = HostContainer.GetInstance<IEnumerable<IBackgroundTaskSetup>>();
            foreach (var backgroundTaskSetup in taskSetups)
            {
                if (backgroundTaskSetup.GetJob().GetType() == backgroundTaskType)
                {
                    var task = GetByName(backgroundTaskSetup.GetName());
                    task.LastRunningTime = DateTime.UtcNow;
                    Update(task);
                    return;
                }
            }
        }

        public BackgroundTask GetByType(Type backgroundTaskType)
        {
            var taskSetups = HostContainer.GetInstance<IEnumerable<IBackgroundTaskSetup>>();
            foreach (var backgroundTaskSetup in taskSetups)
            {
                if (backgroundTaskSetup.GetJob().GetType() == backgroundTaskType)
                {
                    return GetByName(backgroundTaskSetup.GetName());
                }
            }

            return null;
        }

        /// <summary>
        /// Get the task by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BackgroundTask GetByName(string name)
        {
            return _backgroundTaskRepository.FetchFirst(t => t.Name.Equals(name));
        }

        /// <summary>
        /// Update background task
        /// </summary>
        /// <param name="backgroundTask"></param>
        /// <returns></returns>
        public ResponseModel Update(BackgroundTask backgroundTask)
        {
            return _backgroundTaskRepository.Update(backgroundTask);
        }
    }
}