﻿using Ez.Framework.Core.Entity.RepositoryBase;
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
using System.Reflection;

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
            _anonymousContactRepositoryMock.Setup(f => f.GetAll()).Returns(_contacts.AsQueryable());
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

        [Test]
        public void SearchAnonymousContacts_Search_ReturnSearchResult()
        {
            IAnonymousContactService service = new AnonymousContactService(_anonymousContactRepositoryMock.Object);

            Assert.AreEqual(service.GetAll().Count(), 2);
        }
    }
}