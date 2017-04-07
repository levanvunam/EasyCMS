using AutoMapper;
using Ez.Framework.Configurations;
using Ez.Framework.Core.Entity.Entities;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Reflection;
using Ez.Framework.Utilities.Web;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.StarterKits.Interfaces;
using EzCMS.Core.Models.SiteSetup;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Core.SiteInitialize;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.Users;
using System;
using System.Linq;
using System.Transactions;
using System.Web;

namespace EzCMS.Core.Services.SiteSetup
{
    public class SiteSetupService : ISiteSetupService
    {
        private readonly IRepository<EmailAccount> _emailAccountRepository;
        private readonly IRepository<EmailLog> _emailLogRepository;
        private readonly IRepository<SiteSetting> _siteSettingRepository;
        private readonly IUserRepository _userRepository;

        public SiteSetupService(IRepository<EmailAccount> emailAccountRepository, IUserRepository userRepository,
            IRepository<EmailLog> emailLogRepository, IRepository<SiteSetting> siteSettingRepository)
        {
            _emailAccountRepository = emailAccountRepository;
            _userRepository = userRepository;
            _emailLogRepository = emailLogRepository;
            _siteSettingRepository = siteSettingRepository;
        }

        /// <summary>
        /// Finish setup and restart AppDomain
        /// </summary>
        /// <returns></returns>
        public ResponseModel FinishSetup()
        {
            ResponseModel response;

            //Finish setup
            var siteConfig = SiteInitializer.GetConfiguration();
            siteConfig.IsSetupFinish = true;
            SiteInitializer.SaveConfiguration(siteConfig);

            //Notify user of new setup site.
            var setupInfo = WorkContext.SetupInformation;
            var url = HttpContext.Current.Request.Url;
            if (setupInfo.InitialUser != null && setupInfo.SiteConfiguration != null)
            {
                var emailAccount = _emailAccountRepository.FetchFirst(e => e.IsDefault);
                if (emailAccount != null)
                {
                    _emailLogRepository.Insert(new EmailLog
                    {
                        From = emailAccount.Email,
                        FromName = emailAccount.DisplayName,
                        EmailAccount = emailAccount,
                        ToName =
                            string.Format("{0} {1}", setupInfo.InitialUser.FirstName, setupInfo.InitialUser.LastName),
                        To = setupInfo.InitialUser.Email,
                        Subject = "Website setup",
                        Body =
                            string.Format("Hello {0},<br/> Your new site has been setup at <a href='{2}'>{1}</a>",
                                setupInfo.InitialUser.FirstName, url.Host,
                                url.IsDefaultPort
                                    ? string.Format("{0}://{1}/", url.Scheme, url.Host)
                                    : string.Format("{0}://{1}:{2}/", url.Scheme, url.Host, url.Port))
                    });
                }
            }

            //Restart app domain
            try
            {
                WebUtilities.RestartAppDomain();
                response = new ResponseModel
                {
                    Success = true
                };
            }
            catch (Exception exception)
            {
                response = new ResponseModel
                {
                    Success = false,
                    Message = exception.Message
                };
            }

            return response;
        }

        #region Setup database

        /// <summary>
        /// Get database setup model
        /// </summary>
        /// <returns></returns>
        public DatabaseSetupModel GetDatabaseSetupModel()
        {
            return new DatabaseSetupModel();
        }

        /// <summary>
        /// Save database setup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveDatabaseSetupModel(DatabaseSetupModel model)
        {
            ResponseModel response;
            try
            {
                var initialize = new SiteInitializer();
                response = initialize.TestConnectionString(model.Server, model.DatabaseName,
                    model.UserName, model.Password, model.IntegratedSecurity);
                if (response.Success)
                {
                    var config = new SiteConfiguration
                    {
                        ConnectionString =
                            string.Format(
                                "Data Source={0};Initial Catalog={1};uid={2};pwd={3};Integrated Security={4};MultipleActiveResultSets=True",
                                model.Server, model.DatabaseName, model.UserName, model.Password,
                                model.IntegratedSecurity ? "True" : "False"),
                        IsSetupFinish = false
                    };
                    SiteInitializer.SaveConfiguration(config);
                    initialize.InitDatabase();
                    WorkContext.SetupInformation.SiteConfiguration = config;
                }
                else
                {
                    if (response.Message.StartsWith("Login failed for user"))
                    {
                        response.Message = "Invalid username or password";
                    }
                }
            }
            catch (Exception exception)
            {
                response = new ResponseModel
                {
                    Success = false,
                    Message =
                        exception.Message.StartsWith("Login failed for user")
                            ? "Invalid username or password"
                            : exception.Message
                };
            }
            return response;
        }

        #endregion

        #region Setup admin user

        /// <summary>
        /// Get user setup model
        /// </summary>
        /// <returns></returns>
        public UserSetupModel GetUserSetupModel()
        {
            return new UserSetupModel();
        }

        /// <summary>
        /// Save admin user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveUserSetupModel(UserSetupModel model)
        {
            ResponseModel response;
            try
            {
                Mapper.CreateMap<UserSetupModel, User>();
                var siteAdmin = Mapper.Map<UserSetupModel, User>(model);

                string saltKey, hashPass;
                model.Password.GeneratePassword(out saltKey, out hashPass);

                siteAdmin.PasswordSalt = saltKey;
                siteAdmin.Password = hashPass;

                siteAdmin.IsSystemAdministrator = true;
                siteAdmin.Status = UserEnums.UserStatus.Active;

                response = _userRepository.InsertUser(siteAdmin);

                WorkContext.SetupInformation.InitialUser = model;
            }
            catch (Exception exception)
            {
                response = new ResponseModel
                {
                    Success = false,
                    Message = exception.Message
                };
            }

            return response;
        }

        #endregion

        #region Setup site

        /// <summary>
        /// Get site setup model
        /// </summary>
        /// <returns></returns>
        public CompanySetupModel GetCompanySetupModel()
        {
            return new CompanySetupModel();
        }

        /// <summary>
        /// Save site setup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveCompanySetupModel(CompanySetupModel model)
        {
            ResponseModel response;
            var setting = _siteSettingRepository.FetchFirst(s => s.Name.Equals(SettingNames.CompanySetupSetting));
            if (setting == null)
            {
                setting = new SiteSetting
                {
                    Name = SettingNames.CompanySetupSetting,
                    Description = SettingNames.CompanySetupSetting,
                    Value = SerializeUtilities.Serialize(model),
                    SettingType = "system"
                };

                response = _siteSettingRepository.Insert(setting);
            }
            else
            {
                setting.Value = SerializeUtilities.Serialize(model);
                response = _siteSettingRepository.Update(setting);
            }

            return response;
        }

        #endregion

        #region Setup email account

        /// <summary>
        /// Get email setup model
        /// </summary>
        /// <returns></returns>
        public EmailSetupModel GetEmailSetupModel()
        {
            return new EmailSetupModel();
        }

        /// <summary>
        /// Save email setup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveEmailSetupModel(EmailSetupModel model)
        {
            ResponseModel response;
            try
            {
                Mapper.CreateMap<EmailSetupModel, EmailAccount>();
                var emailAccount = Mapper.Map<EmailSetupModel, EmailAccount>(model);
                emailAccount.IsDefault = true;

                response = _emailAccountRepository.Insert(emailAccount);
            }
            catch (Exception exception)
            {
                response = new ResponseModel
                {
                    Success = false,
                    Message = exception.Message
                };
            }

            return response;
        }

        #endregion

        #region Setup starter kit

        /// <summary>
        /// Get avail starter kits model
        /// </summary>
        /// <returns></returns>
        public StarterKitsModel GetStarterKitsModel()
        {
            return new StarterKitsModel();
        }

        /// <summary>
        /// Save starter kits
        /// </summary>
        /// <param name="chosenKit"></param>
        /// <returns></returns>
        public ResponseModel SaveStarterKitsModel(string chosenKit)
        {
            ResponseModel response;
            using (var scope = new TransactionScope())
            {
                try
                {
                    var type = typeof(IStarterKit).GetAllImplementTypesOf(FrameworkConstants.EzSolution)
                        .FirstOrDefault(s => s.FullName.Equals(chosenKit, StringComparison.CurrentCultureIgnoreCase));

                    if (type != null)
                    {
                        var starterKit = (IStarterKit)Activator.CreateInstance(type);
                        starterKit.Setup();

                        WorkContext.SetupInformation.StarterKit = chosenKit;

                        response = new ResponseModel
                        {
                            Success = true
                        };
                    }
                    else
                    {
                        response = new ResponseModel
                        {
                            Success = false,
                            Message = "Wrong Starter Kit"
                        };
                    }
                }
                catch (Exception exception)
                {
                    response = new ResponseModel
                    {
                        Success = false,
                        Message = exception.Message
                    };
                }

                scope.Complete();
            }

            return response;
        }

        #endregion
    }
}