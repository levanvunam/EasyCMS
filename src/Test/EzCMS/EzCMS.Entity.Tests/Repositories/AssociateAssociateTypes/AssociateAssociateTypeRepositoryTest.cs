using EzCMS.Entity.Entities;
using EzCMS.Entity.Repositories.AssociateAssociateTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Tests.Repositories.AssociateAssociateTypes
{
    [TestFixture]
    public class AssociateAssociateTypeRepositoryTest
    {
        private EzCMSEntities _dbContext;
        private IAssociateAssociateTypeRepository _repository;

        #region Setup

        [SetUp]
        public void Setup()
        {
            _dbContext = TestUtilities.GetTestDbContext();
            _repository = new AssociateAssociateTypeRepository(_dbContext);

        }

        [TearDown]
        public void TearDown()
        {
            //Clean up database
            _dbContext.Database.ExecuteSqlCommand("Delete AssociateAssociateTypes");
            _dbContext.Database.ExecuteSqlCommand("Delete AssociateTypes");
            _dbContext.Database.ExecuteSqlCommand("Delete Associates");
            _dbContext.SaveChanges();
            _dbContext.Dispose();
        }

        private AssociateAssociateType CreateNewAssociateAssociateType()
        {
            var associateAssociateType = new AssociateAssociateType
            {
                Associate = new Associate
                {
                    Created = DateTime.Now
                },
                AssociateType = new AssociateType
                {
                    Created = DateTime.Now
                },
                Created = DateTime.Now
            };
            _dbContext.AssociateAssociateTypes.Add(associateAssociateType);
            _dbContext.SaveChanges();

            return associateAssociateType;
        }

        #endregion

        #region DeleteByAssociateId

        [Test]
        public void DeleteByAssociateId_Valid_Success()
        {
            #region Arrange

            var associateAssociateType = CreateNewAssociateAssociateType();

            #endregion

            #region Act

            _repository.DeleteByAssociateTypeId(associateAssociateType.AssociateType.Id, new List<int>
            {
                associateAssociateType.Associate.Id
            });

            #endregion

            #region Assert

            Assert.AreEqual(_dbContext.AssociateAssociateTypes.Count(), 0);

            #endregion
        }

        [Test]
        public void DeleteByAssociateId_NotExistedAssociateTypeId_Success()
        {
            #region Arrange

            var associateAssociateType = CreateNewAssociateAssociateType();

            #endregion

            #region Act

            _repository.DeleteByAssociateId(-1, new List<int>
            {
                associateAssociateType.AssociateType.Id
            });

            #endregion

            #region Assert

            Assert.AreEqual(_dbContext.AssociateAssociateTypes.Count(), 1);

            #endregion
        }


        [Test]
        public void DeleteByAssociateId_NotExistedAssociateId_Success()
        {
            #region Arrange

            var associateAssociateType = CreateNewAssociateAssociateType();

            #endregion

            #region Act

            _repository.DeleteByAssociateId(associateAssociateType.AssociateId, new List<int>
            {
                -1
            });

            #endregion

            #region Assert

            Assert.AreEqual(_dbContext.AssociateAssociateTypes.Count(), 1);

            #endregion
        }

        [Test]
        public void DeleteByAssociateId_EmptyAssociateId_Success()
        {
            #region Arrange

            var associateAssociateType = CreateNewAssociateAssociateType();

            #endregion

            #region Act

            _repository.DeleteByAssociateId(associateAssociateType.AssociateId, new List<int>());

            #endregion

            #region Assert

            Assert.AreEqual(_dbContext.AssociateAssociateTypes.Count(), 1);

            #endregion
        }

        [Test]
        public void DeleteByAssociateId_InvalidAssociateIdAndAssociateTypeId_Success()
        {
            #region Arrange

            var associateAssociateType = CreateNewAssociateAssociateType();

            #endregion

            #region Act

            _repository.DeleteByAssociateId(-1, new List<int>());

            #endregion

            #region Assert

            Assert.AreEqual(_dbContext.AssociateAssociateTypes.Count(), 1);

            #endregion
        }

        #endregion

        #region InsertByAssociateId

        [Test]
        public void InsertByAssociateId_ValidInput_Success()
        {
            #region Arrange

            var associate = new Associate
            {
                Created = DateTime.Now
            };
            _dbContext.Associates.Add(associate);
            var associateType = new AssociateType
            {
                Created = DateTime.Now
            };
            _dbContext.AssociateTypes.Add(associateType);
            _dbContext.SaveChanges();

            #endregion

            #region Act

            _repository.InsertByAssociateId(associate.Id, new List<int>
            {
                associateType.Id
            });

            #endregion

            #region Assert

            Assert.AreEqual(_dbContext.AssociateAssociateTypes.Count(), 1);

            #endregion
        }

        [Test]
        public void InsertByAssociateId_InvalidInput_Failure()
        {
            #region Arrange

            var associate = new Associate
            {
                Created = DateTime.Now
            };
            var associateType = new AssociateType
            {
                Created = DateTime.Now
            };
            _dbContext.Associates.Add(associate);
            _dbContext.AssociateTypes.Add(associateType);
            _dbContext.SaveChanges();

            #endregion

            #region Act

            var response = _repository.InsertByAssociateId(-1, new List<int>
            {
                associateType.Id
            });

            #endregion

            #region Assert

            Assert.AreEqual(response.Success, false);
            Assert.AreEqual(_dbContext.AssociateAssociateTypes.Count(), 0);

            #endregion
        }

        #endregion

        #region DeleteByAssociateTypeId

        [Test]
        public void DeleteByAssociateTypeId_Valid_Success()
        {
            #region Arrange

            var associateAssociateType = CreateNewAssociateAssociateType();

            #endregion

            #region Act

            _repository.DeleteByAssociateTypeId(associateAssociateType.AssociateType.Id, new List<int>
            {
                associateAssociateType.Associate.Id
            });

            #endregion

            #region Assert

            Assert.AreEqual(_dbContext.AssociateAssociateTypes.Count(), 0);

            #endregion
        }

        [Test]
        public void DeleteByAssociateTypeId_NotExistedAssociateTypeId_Success()
        {
            #region Arrange

            var associateAssociateType = CreateNewAssociateAssociateType();

            #endregion

            #region Act

            _repository.DeleteByAssociateTypeId(-1, new List<int>
            {
                associateAssociateType.Associate.Id
            });

            #endregion

            #region Assert

            Assert.AreEqual(_dbContext.AssociateAssociateTypes.Count(), 1);

            #endregion
        }


        [Test]
        public void DeleteByAssociateTypeId_NotExistedAssociateId_Success()
        {
            #region Arrange

            var associateAssociateType = CreateNewAssociateAssociateType();

            #endregion

            #region Act

            _repository.DeleteByAssociateTypeId(associateAssociateType.AssociateTypeId, new List<int>
            {
                -1
            });

            #endregion

            #region Assert

            Assert.AreEqual(_dbContext.AssociateAssociateTypes.Count(), 1);

            #endregion
        }

        [Test]
        public void DeleteByAssociateTypeId_EmptyAssociateId_Success()
        {
            #region Arrange

            var associateAssociateType = CreateNewAssociateAssociateType();

            #endregion

            #region Act

            _repository.DeleteByAssociateTypeId(associateAssociateType.AssociateTypeId, new List<int>());

            #endregion

            #region Assert

            Assert.AreEqual(_dbContext.AssociateAssociateTypes.Count(), 1);

            #endregion
        }

        [Test]
        public void DeleteByAssociateTypeId_InvalidAssociateIdAndAssociateTypeId_Success()
        {
            #region Arrange

            var associateAssociateType = CreateNewAssociateAssociateType();

            #endregion

            #region Act

            _repository.DeleteByAssociateTypeId(-1, new List<int>());

            #endregion

            #region Assert

            Assert.AreEqual(_dbContext.AssociateAssociateTypes.Count(), 1);

            #endregion
        }

        #endregion

        #region InsertByAssociateTypeId

        [Test]
        public void InsertByAssociateTypeId_ValidInput_Success()
        {
            #region Arrange

            var associate = new Associate
            {
                FirstName = "First name",
                LastName = "Last name",
                Created = DateTime.Now,
                LastUpdate = DateTime.Now
            };
            _dbContext.Associates.Add(associate);
            var associateType = new AssociateType
            {
                Name = "Associate Type",
                Created = DateTime.Now,
                LastUpdate = DateTime.Now
            };
            _dbContext.AssociateTypes.Add(associateType);

            #endregion

            #region Act

            _repository.InsertByAssociateTypeId(associateType.Id, new List<int>
            {
                associate.Id
            });

            #endregion

            #region Assert

            Assert.AreEqual(_dbContext.AssociateAssociateTypes.Count(), 1);

            #endregion
        }

        [Test]
        public void InsertByAssociateTypeId_InvalidInput_Failure()
        {
            #region Arrange

            var associate = new Associate
            {
                Created = DateTime.Now
            };
            var associateType = new AssociateType
            {
                Created = DateTime.Now
            };
            _dbContext.Associates.Add(associate);
            _dbContext.AssociateTypes.Add(associateType);
            _dbContext.SaveChanges();

            #endregion

            #region Act

            var response = _repository.InsertByAssociateTypeId(-1, new List<int>
            {
                associate.Id
            });

            #endregion

            #region Assert

            Assert.AreEqual(response.Success, false);
            Assert.AreEqual(_dbContext.AssociateAssociateTypes.Count(), 0);

            #endregion
        }

        #endregion
    }
}
