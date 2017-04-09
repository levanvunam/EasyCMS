using System.Configuration;
using System.Data.Entity;
using NUnit.Framework;

namespace Ez.Framework.Tests.Core.Entity.RepositoryBase
{
    [TestFixture]
    public class RepositoryTest
    {
        #region Setup

        private DbContext _dbContext;

        /// <summary>
        /// Setup
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _dbContext = TestUtilities.GetTestDbContext();
        }

        /// <summary>
        /// Teardown
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        #endregion

        #region GetDbContext

        [Test]
        public void GetDbContext_Success()
        {
            #region Arrange

            #endregion

            #region Act

            #endregion

            #region Assert

            Assert.IsTrue(_dbContext != null);

            #endregion
        }

        #endregion

        #region Connection

        [Test]
        public void Connection_Success()
        {
            #region Arrange

            #endregion

            #region Act

            #endregion

            #region Assert

            Assert.AreEqual(_dbContext.Database.Connection.ConnectionString, 
                ConfigurationManager.AppSettings[TestUtilities.ConnectionStringConfigurationName]);

            #endregion
        }

        #endregion
    }
}
