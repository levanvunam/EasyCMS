using System;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Logging;
using EzCMS.Core.Framework.Logging;
using EzCMS.Core.Services.AnonymousContacts;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;
using Moq;
using NUnit.Framework;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ez.Framework.Core.JqGrid;
using EzCMS.Core.Models.AnonymousContacts;

namespace EzCMS.Core.Tests.Services.AnonymousContacts
{
    [TestFixture]
    public class AnonymousContactServiceTest
    {
        #region Setup

        private Mock<Repository<AnonymousContact>> _anonymousContactRepositoryMock;
        private DbContext _dbContext;
        private readonly List<AnonymousContact> _contacts = new List<AnonymousContact>
        {
            new AnonymousContact
            {
                Id = 1
            },
            new AnonymousContact
            {
                Id = 2
            }
        };

        [SetUp]
        public void Setup()
        {
            //Register IOC
            TestUtilties.RegisterIOC();

            _dbContext = new EzCMSEntities();
            _anonymousContactRepositoryMock = new Mock<Repository<AnonymousContact>>(_dbContext);
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

        #region GetAll
         
        [Test]
        public void GetAll_Valid_ReturnValidResult()
        {
            #region Arrange 

            _anonymousContactRepositoryMock.Setup(f => f.GetAll()).Returns(_contacts.AsQueryable());
            IAnonymousContactService service = new AnonymousContactService(_anonymousContactRepositoryMock.Object);

            #endregion

            #region Act
            #endregion

            #region Assert

            Assert.AreEqual(service.GetAll().Count(), 2);

            #endregion
        }

        #endregion

        #region SearchAnonymousContacts

        #region GetAll

        [Test]
        public void SearchAnonymousContacts_Valid_ReturnValidResult()
        {
            #region Arrange 

            _anonymousContactRepositoryMock.Setup(f => f.Fetch(It.IsAny<Expression<Func<AnonymousContact, bool>>>())).Returns(_contacts.AsQueryable());
            IAnonymousContactService service = new AnonymousContactService(_anonymousContactRepositoryMock.Object);
            Mock<JqSearchIn> jqSearchInMock = new Mock<JqSearchIn>();
            Mock<AnonymousContactSearchModel> searchModelMock = new Mock<AnonymousContactSearchModel>();

            #endregion

            #region Act

            var result = service.SearchAnonymousContacts(jqSearchInMock.Object, searchModelMock.Object);
            #endregion

            #region Assert

            Assert.AreEqual(result.records, 2);

            #endregion
        }

        #endregion

        #endregion
    }
}