using Ez.Framework.Core.IoC.Attributes;
using System.Data.Entity;

namespace Ez.Framework.Core.Entity.Intialize
{
    [Register(Lifetime.PerInstance)]
    public interface IDataInitializer
    {
        /// <summary>
        /// Get priority of data initializer
        /// </summary>
        /// <returns></returns>
        DataInitializerPriority Priority();

        /// <summary>
        /// Initialize data
        /// </summary>
        /// <param name="ezDbContext"></param>
        void Initialize(DbContext ezDbContext);
    }

    public enum DataInitializerPriority
    {
        High = 1,
        Medium = 2,
        Low = 3
    }
}
