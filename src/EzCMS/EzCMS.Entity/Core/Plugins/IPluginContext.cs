using Ez.Framework.Core.IoC.Attributes;
using System.Data.Entity;

namespace EzCMS.Entity.Core.Plugins
{
    [Register(Lifetime.SingleTon)]
    public interface IPluginContext
    {
        /// <summary>
        /// Setup context
        /// </summary>
        /// <param name="modelBuilder"></param>
        void Setup(DbModelBuilder modelBuilder);

        /// <summary>
        /// Setup seed
        /// </summary>
        /// <param name="context"></param>
        void Seed(DbContext context);
    }
}
