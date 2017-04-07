using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.IoC.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.Users
{
    [Register(Lifetime.PerInstance)]
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        User GetByEmail(string email);

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="isRemote"></param>
        /// <returns></returns>
        User GetByUsername(string username, bool? isRemote = null);

        /// <summary>
        /// Get active user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        User GetActiveUser(string username);

        /// <summary>
        /// Get active users
        /// </summary>
        /// <returns></returns>
        IQueryable<User> GetActiveUsers();

        /// <summary>
        /// Save user and also insert contact if needed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="contactId"></param>
        /// <returns></returns>
        ResponseModel InsertUser(User user, int? contactId = null);
    }
}