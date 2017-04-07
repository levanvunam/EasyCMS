using Ez.Framework.Core.Entity.Intialize;
using Ez.Framework.Core.IoC;
using EzCMS.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace EzCMS.Entity.Migrations
{
    using System.Data.Entity.Migrations;

    public sealed class Configuration : DbMigrationsConfiguration<Entities.EzCMSEntities>
    {
        public const string MigrationNameSpace = "EzCMS.Entity.MigrationScripts";

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(EzCMSEntities context)
        {
            IEnumerable<IDataInitializer> initializers = Enumerable.Empty<IDataInitializer>();
            try
            {
                initializers = HostContainer.GetInstances<IDataInitializer>().OrderBy(i => i.Priority());
            }
            catch (Exception)
            {
                //Likely called from Package Manager Console
            }
            using (var scope = new TransactionScope())
            {
                foreach (var initializer in initializers)
                {
                    initializer.Initialize(context);
                    context.SaveChanges();
                }
                scope.Complete();
            }
        }
    }
}
