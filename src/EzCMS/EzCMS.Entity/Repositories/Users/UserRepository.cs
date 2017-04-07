using System;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Utilities;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Entity.Repositories.Users
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly IRepository<Contact> _contactRepository;

        public UserRepository(EzCMSEntities entities, IRepository<Contact> contactRepository)
            : base(entities)
        {
            _contactRepository = contactRepository;
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetByEmail(string email)
        {
            return FetchFirst(user => user.Email.Equals(email));
        }

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="isRemote"></param>
        /// <returns></returns>
        public User GetByUsername(string username, bool? isRemote = null)
        {
            var query = GetAll();
            if (isRemote.HasValue)
            {
                query = query.Where(u => u.IsRemoteAccount == isRemote);
            }

            // Get user by username or email
            var user =
                query.FirstOrDefault(
                    u => string.IsNullOrEmpty(u.Username) ? u.Email.Equals(username) : u.Username.Equals(username));

            // Get user with unique email
            if (user == null && query.Count(u => u.Email.Equals(username)) == 1)
            {
                user = query.FirstOrDefault(u => u.Email.Equals(username));
            }
            return user;
        }

        /// <summary>
        /// Get active user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public User GetActiveUser(string username)
        {
            var query = GetActiveUsers();

            // Get user by username or email
            var user =
                query.FirstOrDefault(
                    u => string.IsNullOrEmpty(u.Username) ? u.Email.Equals(username) : u.Username.Equals(username));

            // Get user with unique email
            if (user == null && query.Count(u => u.Email.Equals(username)) == 1)
            {
                user = query.FirstOrDefault(u => u.Email.Equals(username));
            }
            return user;
        }

        /// <summary>
        /// Get active users
        /// </summary>
        /// <returns></returns>
        public IQueryable<User> GetActiveUsers()
        {
            return Fetch(user => user.Status == UserEnums.UserStatus.Active);
        }

        /// <summary>
        /// Insert user and add contact if needed
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public ResponseModel InsertUser(User entity, int? contactId = null)
        {
            entity.LastPasswordChange = DateTime.UtcNow;
            var response = Insert(entity);

            if (response.Success)
            {
                var contact = _contactRepository.GetById(contactId);
                if (contact != null)
                {
                    contact.UserId = entity.Id;
                    _contactRepository.Update(contact);
                }
                else
                {
                    contact = new Contact
                    {
                        UserId = entity.Id,
                        Email = entity.Email,
                        AddressLine1 = entity.Address,
                        FirstName = entity.FirstName,
                        LastName = entity.LastName,
                        PreferredPhoneNumber = entity.Phone,
                        Sex = entity.Gender.GetEnumName(),
                        DateOfBirth = entity.DateOfBirth,
                        Facebook = entity.Facebook,
                        Twitter = entity.Twitter,
                        LinkedIn = entity.LinkedIn
                    };
                    _contactRepository.Insert(contact);
                }
            }

            return response;
        }
    }
}