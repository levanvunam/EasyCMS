using System.Configuration;
using EzCMS.Entity.Entities;

namespace EzCMS.Entity.Tests
{
    public static class TestUtilities
    {
        public const string ConnectionStringConfigurationName = "TestConnectionString";
        private static readonly string TestConnectionString;
        static TestUtilities()
        {
            TestConnectionString = ConfigurationManager.AppSettings[ConnectionStringConfigurationName];
        }

        /// <summary>
        /// Init test db context
        /// </summary>
        /// <returns></returns>
        public static EzCMSEntities GetTestDbContext()
        {
            return new EzCMSEntities(TestConnectionString);
        }
    }
}
