using Ez.Framework.Core.Entity.Entities;
using Ez.Framework.Core.IoC;
using EzCMS.Entity.Core.Configurations;
using EzCMS.Entity.Core.Plugins;
using EzCMS.Entity.Core.SiteInitialize;
using EzCMS.Entity.Entities.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace EzCMS.Entity.Entities
{
    /// <summary>
    /// EzCMS db context
    /// </summary>
    public class EzCMSEntities : DbContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EzCMSEntities()
            : base(SiteInitializer.GetHttpSiteConfiguration() == null ? "Name=" + EzCMSEntityConstants.EzCMSEntities : SiteInitializer.GetHttpSiteConfiguration().ConnectionString)
        {
        }

        /// <summary>
        /// Constructor with connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public EzCMSEntities(string connectionString): 
            base(connectionString)
        {
            
        }

        #region CRM

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<AnonymousContact> AnonymousContacts { get; set; }
        public DbSet<ContactCommunication> ContactCommunications { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ProductOfInterest> ProductOfInterests { get; set; }
        public DbSet<CampaignCode> CampaignCodes { get; set; }

        #endregion

        #region Features

        #region Associates and Locations

        public DbSet<Associate> Associates { get; set; }
        public DbSet<AssociateAssociateType> AssociateAssociateTypes { get; set; }
        public DbSet<AssociateCompanyType> AssociateCompanyTypes { get; set; }
        public DbSet<AssociateLocation> AssociateLocations { get; set; }
        public DbSet<AssociateType> AssociateTypes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<LocationType> LocationTypes { get; set; }
        public DbSet<LocationLocationType> LocationLocationTypes { get; set; }

        #endregion

        #region Pages

        public DbSet<Page> Pages { get; set; }
        public DbSet<PageLog> PageLogs { get; set; }
        public DbSet<PageRead> PageReads { get; set; }
        public DbSet<PageSecurity> PageSecurities { get; set; }
        public DbSet<ClientNavigation> ClientNavigations { get; set; }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<PageTag> PageTags { get; set; }

        public DbSet<BodyTemplate> BodyTemplates { get; set; }

        public DbSet<FileTemplate> FileTemplates { get; set; }

        public DbSet<PageTemplate> PageTemplates { get; set; }
        public DbSet<PageTemplateLog> PageTemplateLogs { get; set; }

        public DbSet<WidgetTemplate> WidgetTemplates { get; set; }
        public DbSet<WidgetTemplateLog> WidgetTemplateLogs { get; set; }
        #endregion

        #region News

        public DbSet<News> News { get; set; }
        public DbSet<NewsCategory> NewsCategories { get; set; }
        public DbSet<NewsNewsCategory> NewsNewsCategories { get; set; }
        public DbSet<NewsRead> NewsReads { get; set; }

        #endregion

        #region Styles & Scripts

        public DbSet<Script> Scripts { get; set; }
        public DbSet<ScriptLog> ScriptLogs { get; set; }

        public DbSet<Style> Style { get; set; }
        public DbSet<StyleLog> StyleLogs { get; set; }

        #endregion

        #region Subscriptions

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionTemplate> SubscriptionTemplates { get; set; }
        public DbSet<SubscriptionLog> SubscriptionLogs { get; set; }

        #endregion

        #region Protected Documents

        public DbSet<ProtectedDocument> ProtectedDocuments { get; set; }
        public DbSet<ProtectedDocumentLog> ProtectedDocumentLogs { get; set; }
        public DbSet<ProtectedDocumentGroup> ProtectedDocumentGroups { get; set; }
        public DbSet<ProtectedDocumentCompany> ProtectedDocumentGroupCompanies { get; set; }
        public DbSet<ProtectedDocumentCompanyType> ProtectedDocumentCompanyTypes { get; set; }

        #endregion

        #region Notifications

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<ContactGroup> ContactGroups { get; set; }
        public DbSet<ContactGroupContact> ContactGroupContacts { get; set; }

        #endregion

        #region Others

        public DbSet<Banner> Banners { get; set; }

        public DbSet<RotatingImageGroup> RotatingImageGroups { get; set; }
        public DbSet<RotatingImage> RotatingImages { get; set; }

        #region Social Media

        public DbSet<SocialMedia> SocialMedia { get; set; }
        public DbSet<SocialMediaToken> SocialMediaTokens { get; set; }
        public DbSet<SocialMediaLog> SocialMediaLogs { get; set; }

        #endregion

        #region Others

        public DbSet<Service> Services { get; set; }

        public DbSet<Testimonial> Testimonials { get; set; }

        #endregion

        #endregion

        #region Form Builder

        public DbSet<Form> Forms { get; set; }
        public DbSet<FormTab> FormTabs { get; set; }
        public DbSet<FormComponentTemplate> FormComponentTemplates { get; set; }
        public DbSet<FormComponent> FormComponents { get; set; }
        public DbSet<FormComponentField> FormComponentFields { get; set; }
        public DbSet<FormDefaultComponent> FormDefaultComponents { get; set; }
        public DbSet<FormDefaultComponentField> FormDefaultComponentFields { get; set; }

        #endregion

        #region Notices

        public DbSet<Notice> Notices { get; set; }
        public DbSet<NoticeType> NoticeTypes { get; set; }

        #endregion

        #region Links

        public DbSet<Link> Links { get; set; }
        public DbSet<LinkType> LinkTypes { get; set; }

        #endregion

        #region Events

        public DbSet<Event> Events { get; set; }
        public DbSet<EventSchedule> EventSchedules { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<EventEventCategory> EventsEventCategories { get; set; }

        #endregion

        #region Polls

        public DbSet<Poll> Polls { get; set; }
        public DbSet<PollAnswer> PollAnswers { get; set; }

        #endregion

        #region Link Trackers

        public DbSet<LinkTracker> LinkTrackers { get; set; }
        public DbSet<LinkTrackerClick> LinkTrackerClicks { get; set; }

        #endregion

        #region RssFeed

        public DbSet<RssFeed> RssFeeds { get; set; }

        #endregion

        #endregion

        #region System

        #region Users

        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserUserGroup> UserUserGroups { get; set; }
        public DbSet<Toolbar> Toolbars { get; set; }
        public DbSet<GroupPermission> GroupPermissions { get; set; }

        #endregion

        #region Emails

        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<EmailAccount> EmailAccounts { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }

        #endregion

        public DbSet<Country> Countries { get; set; }

        public DbSet<Language> Languages { get; set; }

        public DbSet<LocalizedResource> LocalizedResources { get; set; }

        public DbSet<FavouriteNavigation> FavouriteNavigations { get; set; }

        public DbSet<SlideInHelp> SlideInHelps { get; set; }

        public DbSet<RemoteAuthentication> AuthenticateConfigurations { get; set; }

        public DbSet<BackgroundTask> BackgroundTasks { get; set; }

        public DbSet<SiteSetting> SiteSettings { get; set; }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            #region Build Plugin Entities

            try
            {
                var plugins = HostContainer.GetInstances<IPluginContext>();

                foreach (var plugin in plugins)
                {
                    plugin.Setup(modelBuilder);
                }
            }
            catch (Exception)
            {
                //This may come from NUGET
            }

            #endregion
        }
    }
}