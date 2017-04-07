using EzCMS.Entity.Core.Plugins;
using System.Data.Entity;

namespace EzCMS.Help.Plugins.Entity
{
    public class PluginContext : IPluginContext
    {
        public void Setup(DbModelBuilder modelBuilder)
        {

        }

        public void Seed(DbContext context)
        {
        }
    }
}