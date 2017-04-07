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
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            container.Register(() => new EzCMSEntities(), Lifestyle.Scoped);
            container.Register<DbContext>(() => new EzCMSEntities(), Lifestyle.Scoped);

            container.Register(typeof(IRepository<>), typeof(Repository<>), Lifestyle.Scoped);
            container.Register(typeof(IHierarchyRepository<>), typeof(HierarchyRepository<>), Lifestyle.Scoped);
            IoCRegister.RegisterDepencies(container);

            // Logger using site settings
            container.Register(typeof(ILogger), () => new Logger(MethodBase.GetCurrentMethod().DeclaringType), Lifestyle.Scoped);
            HostContainer.SetContainer(container);
            container.Verify();

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