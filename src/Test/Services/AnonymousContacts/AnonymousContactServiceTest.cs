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
using System.Reflection;

namespace EzCMS.Core.Tests.Services.AnonymousContacts
{
    [TestFixture]
    public class AnonymousContactServiceTest
    {
        private Mock<Repository<AnonymousContact>> _anonymousContactRepositoryMock;
        private DbContext _dataContext;
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

            _dataContext = new EzCMSEntities();
            _anonymousContactRepositoryMock = new Mock<Repository<AnonymousContact>>(_dataContext);
            _anonymousContactRepositoryMock.Setup(f => f.GetAll()).Returns(_contacts.AsQueryable());
        }

        [Test]
        public void SearchAnonymousContacts_Search_ReturnSearchResult()
        {
            IAnonymousContactService service = new AnonymousContactService(_anonymousContactRepositoryMock.Object);

            Assert.Equals(service.GetAll().Count(), 2);
        }
    }
}