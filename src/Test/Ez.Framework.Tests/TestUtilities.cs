using System.Configuration;
using System.Data.Entity;

namespace Ez.Framework.Tests
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
        public static DbContext GetTestDbContext()
        {
            return new DbContext(TestConnectionString);
        }
    }
}
