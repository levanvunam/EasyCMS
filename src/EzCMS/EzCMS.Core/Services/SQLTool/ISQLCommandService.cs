using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.SQLTool;
using EzCMS.Entity.Entities.Models;
using System.Collections.Generic;
using System.Data.Common;

namespace EzCMS.Core.Services.SQLTool
{
    [Register(Lifetime.PerInstance)]
    public interface ISQLCommandService : IBaseService<SQLCommandHistory>
    {
        #region Grid

        JqGridSearchOut SearchCommands(JqSearchIn si);

        #endregion

        ResponseModel SaveCommand(SQLRequest request);

        IEnumerable<SqlCommandHistoryModel> GetHistories(int index, int pageSize);

        DbConnection GetConnection();

        string GetConnectionString();
    }
}