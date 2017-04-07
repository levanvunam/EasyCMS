namespace EzCMS.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnonymousContacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContactId = c.Int(),
                        CookieKey = c.String(),
                        IpAddress = c.String(),
                        Email = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Phone = c.String(),
                        Address = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.ContactId)
                .Index(t => t.ContactId);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        Email = c.String(maxLength: 255),
                        Title = c.String(maxLength: 50),
                        Company = c.String(),
                        FirstName = c.String(maxLength: 100),
                        LastName = c.String(maxLength: 100),
                        Department = c.String(maxLength: 60),
                        IsCompanyAdministrator = c.Boolean(nullable: false),
                        AddressLine1 = c.String(maxLength: 255),
                        AddressLine2 = c.String(maxLength: 255),
                        Suburb = c.String(maxLength: 100),
                        State = c.String(maxLength: 50),
                        Postcode = c.String(maxLength: 50),
                        Country = c.String(maxLength: 100),
                        PostalAddressLine1 = c.String(maxLength: 255),
                        PostalAddressLine2 = c.String(maxLength: 255),
                        PostalSuburb = c.String(maxLength: 100),
                        PostalState = c.String(maxLength: 50),
                        PostalPostcode = c.String(maxLength: 50),
                        PostalCountry = c.String(maxLength: 100),
                        PreferredPhoneNumber = c.String(maxLength: 50),
                        PhoneWork = c.String(maxLength: 50),
                        PhoneHome = c.String(maxLength: 50),
                        MobilePhone = c.String(maxLength: 50),
                        Fax = c.String(maxLength: 50),
                        Occupation = c.String(maxLength: 255),
                        Website = c.String(maxLength: 100),
                        Sex = c.String(maxLength: 20),
                        DateOfBirth = c.DateTime(),
                        DontSendMarketing = c.Boolean(nullable: false),
                        Unsubscribed = c.Boolean(nullable: false),
                        UnsubscribeDateTime = c.DateTime(),
                        SubscriptionType = c.Int(nullable: false),
                        Confirmed = c.Boolean(),
                        ConfirmDateTime = c.String(maxLength: 50),
                        FromIPAddress = c.String(maxLength: 50),
                        ValidatedOk = c.Long(),
                        ValidateLevel = c.Int(),
                        CRMID = c.String(maxLength: 255),
                        SalesPerson = c.String(maxLength: 255),
                        UnsubscribedIssueId = c.Int(),
                        Facebook = c.String(maxLength: 100),
                        Twitter = c.String(maxLength: 100),
                        LinkedIn = c.String(maxLength: 100),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ContactCommunications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContactId = c.Int(nullable: false),
                        ReferredBy = c.String(maxLength: 255),
                        CampaignCode = c.String(maxLength: 255),
                        Comments = c.String(),
                        CommentDate = c.DateTime(),
                        ProductOfInterest = c.String(maxLength: 255),
                        CurrentlyOwn = c.String(maxLength: 255),
                        PurchaseDate = c.String(maxLength: 255),
                        InterestedInOwning = c.Boolean(nullable: false),
                        TimeFrameToOwn = c.String(maxLength: 255),
                        Certification = c.String(maxLength: 255),
                        SubscriberType = c.String(maxLength: 50),
                        UnsubscribeComment = c.String(maxLength: 255),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.ContactId)
                .Index(t => t.ContactId);
            
            CreateTable(
                "dbo.ContactGroupContacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContactId = c.Int(nullable: false),
                        ContactGroupId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.ContactId)
                .ForeignKey("dbo.ContactGroups", t => t.ContactGroupId)
                .Index(t => t.ContactId)
                .Index(t => t.ContactGroupId);
            
            CreateTable(
                "dbo.ContactGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 512),
                        Queries = c.String(),
                        Active = c.Boolean(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        Module = c.Int(nullable: false),
                        Parameters = c.String(),
                        Active = c.Boolean(nullable: false),
                        DeactivatedDate = c.DateTime(),
                        ContactId = c.Int(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.ContactId)
                .Index(t => t.ContactId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(maxLength: 255),
                        Email = c.String(maxLength: 255),
                        Password = c.String(maxLength: 512),
                        PasswordSalt = c.String(maxLength: 512),
                        FirstName = c.String(nullable: false, maxLength: 512),
                        LastName = c.String(nullable: false, maxLength: 512),
                        Phone = c.String(maxLength: 50),
                        IsSystemAdministrator = c.Boolean(nullable: false),
                        DateOfBirth = c.DateTime(),
                        Gender = c.Int(nullable: false),
                        About = c.String(),
                        AvatarFileName = c.String(maxLength: 512),
                        Address = c.String(maxLength: 1024),
                        Status = c.Int(nullable: false),
                        LastLogin = c.DateTime(),
                        Facebook = c.String(maxLength: 512),
                        Twitter = c.String(maxLength: 512),
                        LinkedIn = c.String(maxLength: 512),
                        IsRemoteAccount = c.Boolean(nullable: false),
                        Settings = c.String(),
                        ReleaseLockDate = c.DateTime(),
                        LastPasswordChange = c.DateTime(nullable: false),
                        PasswordFailsCount = c.Int(nullable: false),
                        LastFailedLogin = c.DateTime(),
                        ResetPasswordCode = c.String(),
                        ResetPasswordExpiryDate = c.DateTime(),
                        ChangePasswordAfterLogin = c.Boolean(nullable: false),
                        AccountExpiresDate = c.DateTime(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserLoginHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        IpAddress = c.String(),
                        Platform = c.String(),
                        OsVersion = c.String(),
                        BrowserType = c.String(),
                        BrowserName = c.String(),
                        BrowserVersion = c.String(),
                        MajorVersion = c.Int(nullable: false),
                        MinorVersion = c.Double(nullable: false),
                        IsBeta = c.Boolean(nullable: false),
                        IsCrawler = c.Boolean(nullable: false),
                        IsAOL = c.Boolean(nullable: false),
                        IsWin16 = c.Boolean(nullable: false),
                        IsWin32 = c.Boolean(nullable: false),
                        SupportsFrames = c.Boolean(nullable: false),
                        SupportsTables = c.Boolean(nullable: false),
                        SupportsCookies = c.Boolean(nullable: false),
                        SupportsVBScript = c.Boolean(nullable: false),
                        SupportsJavaScript = c.String(),
                        SupportsJavaApplets = c.Boolean(nullable: false),
                        SupportsActiveXControls = c.Boolean(nullable: false),
                        JavaScriptVersion = c.String(),
                        RawAgentString = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserUserGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        UserGroupId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.UserGroups", t => t.UserGroupId)
                .Index(t => t.UserId)
                .Index(t => t.UserGroupId);
            
            CreateTable(
                "dbo.UserGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 256),
                        Description = c.String(maxLength: 512),
                        RedirectUrl = c.String(maxLength: 256),
                        ToolbarId = c.Int(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Toolbars", t => t.ToolbarId)
                .Index(t => t.ToolbarId);
            
            CreateTable(
                "dbo.GroupPermissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserGroupId = c.Int(nullable: false),
                        PermissionId = c.Int(nullable: false),
                        HasPermission = c.Boolean(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserGroups", t => t.UserGroupId)
                .Index(t => t.UserGroupId);
            
            CreateTable(
                "dbo.PageSecurities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PageId = c.Int(nullable: false),
                        GroupId = c.Int(nullable: false),
                        CanView = c.Boolean(nullable: false),
                        CanEdit = c.Boolean(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pages", t => t.PageId)
                .ForeignKey("dbo.UserGroups", t => t.GroupId)
                .Index(t => t.PageId)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.Pages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 255),
                        Abstract = c.String(),
                        SeoTitle = c.String(maxLength: 255),
                        Description = c.String(),
                        AbstractWorking = c.String(),
                        Content = c.String(),
                        ContentWorking = c.String(),
                        FriendlyUrl = c.String(maxLength: 255),
                        Keywords = c.String(maxLength: 255),
                        PageTemplateId = c.Int(),
                        FileTemplateId = c.Int(),
                        BodyTemplateId = c.Int(),
                        Status = c.Int(nullable: false),
                        IsHomePage = c.Boolean(nullable: false),
                        IncludeInSiteNavigation = c.Boolean(nullable: false),
                        DisableNavigationCascade = c.Boolean(nullable: false),
                        StartPublishingDate = c.DateTime(),
                        EndPublishingDate = c.DateTime(),
                        SSL = c.Boolean(nullable: false),
                        ParentId = c.Int(),
                        Hierarchy = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BodyTemplates", t => t.BodyTemplateId)
                .ForeignKey("dbo.Pages", t => t.ParentId)
                .ForeignKey("dbo.FileTemplates", t => t.FileTemplateId)
                .ForeignKey("dbo.PageTemplates", t => t.PageTemplateId)
                .Index(t => t.PageTemplateId)
                .Index(t => t.FileTemplateId)
                .Index(t => t.BodyTemplateId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.BodyTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 512),
                        Description = c.String(maxLength: 512),
                        ImageUrl = c.String(maxLength: 256),
                        Content = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClientNavigations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 512),
                        PageId = c.Int(),
                        Url = c.String(maxLength: 512),
                        UrlTarget = c.String(),
                        IncludeInSiteNavigation = c.Boolean(nullable: false),
                        DisableNavigationCascade = c.Boolean(nullable: false),
                        StartPublishingDate = c.DateTime(),
                        EndPublishingDate = c.DateTime(),
                        ParentId = c.Int(),
                        Hierarchy = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientNavigations", t => t.ParentId)
                .ForeignKey("dbo.Pages", t => t.PageId)
                .Index(t => t.PageId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.FileTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 512),
                        Controller = c.String(maxLength: 512),
                        Action = c.String(maxLength: 512),
                        Area = c.String(maxLength: 512),
                        Parameters = c.String(maxLength: 512),
                        PageTemplateId = c.Int(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PageTemplates", t => t.PageTemplateId)
                .Index(t => t.PageTemplateId);
            
            CreateTable(
                "dbo.PageTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Content = c.String(),
                        IsDefaultTemplate = c.Boolean(nullable: false),
                        CacheName = c.String(),
                        IsValid = c.Boolean(nullable: false),
                        CompileMessage = c.String(),
                        ParentId = c.Int(),
                        Hierarchy = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PageTemplates", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.PageTemplateLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SessionId = c.String(maxLength: 512),
                        Name = c.String(maxLength: 512),
                        PageTemplateId = c.Int(nullable: false),
                        ParentId = c.Int(),
                        Content = c.String(),
                        ChangeLog = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PageTemplates", t => t.PageTemplateId)
                .Index(t => t.PageTemplateId);
            
            CreateTable(
                "dbo.LinkTrackers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsAllowMultipleClick = c.Boolean(nullable: false),
                        RedirectUrl = c.String(),
                        PageId = c.Int(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pages", t => t.PageId)
                .Index(t => t.PageId);
            
            CreateTable(
                "dbo.LinkTrackerClicks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LinkTrackerId = c.Int(nullable: false),
                        IpAddress = c.String(),
                        Platform = c.String(),
                        OsVersion = c.String(),
                        BrowserType = c.String(),
                        BrowserName = c.String(),
                        BrowserVersion = c.String(),
                        MajorVersion = c.Int(nullable: false),
                        MinorVersion = c.Double(nullable: false),
                        IsBeta = c.Boolean(nullable: false),
                        IsCrawler = c.Boolean(nullable: false),
                        IsAOL = c.Boolean(nullable: false),
                        IsWin16 = c.Boolean(nullable: false),
                        IsWin32 = c.Boolean(nullable: false),
                        SupportsFrames = c.Boolean(nullable: false),
                        SupportsTables = c.Boolean(nullable: false),
                        SupportsCookies = c.Boolean(nullable: false),
                        SupportsVBScript = c.Boolean(nullable: false),
                        SupportsJavaScript = c.String(),
                        SupportsJavaApplets = c.Boolean(nullable: false),
                        SupportsActiveXControls = c.Boolean(nullable: false),
                        JavaScriptVersion = c.String(),
                        RawAgentString = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LinkTrackers", t => t.LinkTrackerId)
                .Index(t => t.LinkTrackerId);
            
            CreateTable(
                "dbo.PageLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SessionId = c.String(maxLength: 512),
                        PageId = c.Int(nullable: false),
                        ParentId = c.Int(),
                        Title = c.String(maxLength: 512),
                        SeoTitle = c.String(),
                        Description = c.String(),
                        Abstract = c.String(),
                        AbstractWorking = c.String(),
                        Content = c.String(),
                        ContentWorking = c.String(),
                        FriendlyUrl = c.String(maxLength: 512),
                        PageTemplateId = c.Int(),
                        FileTemplateId = c.Int(),
                        BodyTemplateId = c.Int(),
                        Status = c.Int(nullable: false),
                        IncludeInSiteNavigation = c.Boolean(nullable: false),
                        DisableNavigationCascade = c.Boolean(nullable: false),
                        StartPublishingDate = c.DateTime(),
                        EndPublishingDate = c.DateTime(),
                        SSL = c.Boolean(nullable: false),
                        Keywords = c.String(maxLength: 512),
                        ChangeLog = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pages", t => t.PageId)
                .Index(t => t.PageId);
            
            CreateTable(
                "dbo.PageReads",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PageId = c.Int(nullable: false),
                        AnonymousContactId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnonymousContacts", t => t.AnonymousContactId)
                .ForeignKey("dbo.Pages", t => t.PageId)
                .Index(t => t.PageId)
                .Index(t => t.AnonymousContactId);
            
            CreateTable(
                "dbo.PageTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PageId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pages", t => t.PageId)
                .ForeignKey("dbo.Tags", t => t.TagId)
                .Index(t => t.PageId)
                .Index(t => t.TagId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 512),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProtectedDocumentGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProtectedDocumentId = c.Int(nullable: false),
                        UserGroupId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProtectedDocuments", t => t.ProtectedDocumentId)
                .ForeignKey("dbo.UserGroups", t => t.UserGroupId)
                .Index(t => t.ProtectedDocumentId)
                .Index(t => t.UserGroupId);
            
            CreateTable(
                "dbo.ProtectedDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Path = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProtectedDocumentCompanies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProtectedDocumentId = c.Int(nullable: false),
                        CompanyId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.ProtectedDocuments", t => t.ProtectedDocumentId)
                .Index(t => t.ProtectedDocumentId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 512),
                        CompanyTypeId = c.Int(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CompanyTypes", t => t.CompanyTypeId)
                .Index(t => t.CompanyTypeId);
            
            CreateTable(
                "dbo.CompanyTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 512),
                        Description = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AssociateCompanyTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssociateId = c.Int(nullable: false),
                        CompanyTypeId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Associates", t => t.AssociateId)
                .ForeignKey("dbo.CompanyTypes", t => t.CompanyTypeId)
                .Index(t => t.AssociateId)
                .Index(t => t.CompanyTypeId);
            
            CreateTable(
                "dbo.Associates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 50),
                        JobTitle = c.String(maxLength: 255),
                        FirstName = c.String(maxLength: 100),
                        LastName = c.String(maxLength: 100),
                        Email = c.String(maxLength: 255),
                        Company = c.String(maxLength: 512),
                        Gender = c.String(maxLength: 20),
                        Photo = c.String(maxLength: 255),
                        University = c.String(maxLength: 255),
                        Qualification = c.String(),
                        OtherQualification = c.String(),
                        Achievements = c.String(),
                        Memberships = c.String(),
                        Appointments = c.String(),
                        PersonalInterests = c.String(),
                        ProfessionalInterests = c.String(),
                        Positions = c.String(),
                        IsNew = c.Boolean(nullable: false),
                        DateStart = c.DateTime(),
                        DateEnd = c.DateTime(),
                        AddressLine1 = c.String(maxLength: 255),
                        AddressLine2 = c.String(maxLength: 255),
                        Suburb = c.String(maxLength: 100),
                        State = c.String(maxLength: 50),
                        Postcode = c.String(maxLength: 50),
                        Country = c.String(maxLength: 100),
                        PhoneWork = c.String(maxLength: 50),
                        PhoneHome = c.String(maxLength: 50),
                        MobilePhone = c.String(maxLength: 50),
                        Fax = c.String(maxLength: 50),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AssociateAssociateTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssociateId = c.Int(nullable: false),
                        AssociateTypeId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Associates", t => t.AssociateId)
                .ForeignKey("dbo.AssociateTypes", t => t.AssociateTypeId)
                .Index(t => t.AssociateId)
                .Index(t => t.AssociateTypeId);
            
            CreateTable(
                "dbo.AssociateTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AssociateLocations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssociateId = c.Int(nullable: false),
                        LocationId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Associates", t => t.AssociateId)
                .ForeignKey("dbo.Locations", t => t.LocationId)
                .Index(t => t.AssociateId)
                .Index(t => t.LocationId);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        TrainingAffiliation = c.String(maxLength: 255),
                        PinImage = c.String(maxLength: 255),
                        ContactName = c.String(maxLength: 100),
                        ContactTitle = c.String(maxLength: 100),
                        OpeningHoursWeekdays = c.String(maxLength: 100),
                        OpeningHoursSaturday = c.String(maxLength: 100),
                        OpeningHoursSunday = c.String(maxLength: 100),
                        TimeZone = c.String(),
                        AddressLine1 = c.String(maxLength: 255),
                        AddressLine2 = c.String(maxLength: 255),
                        Suburb = c.String(maxLength: 100),
                        State = c.String(maxLength: 50),
                        Postcode = c.String(maxLength: 50),
                        Country = c.String(maxLength: 100),
                        Latitude = c.Double(),
                        Longitude = c.Double(),
                        Phone = c.String(maxLength: 50),
                        Fax = c.String(maxLength: 50),
                        Email = c.String(maxLength: 255),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LocationLocationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LocationId = c.Int(nullable: false),
                        LocationTypeId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Locations", t => t.LocationId)
                .ForeignKey("dbo.LocationTypes", t => t.LocationTypeId)
                .Index(t => t.LocationId)
                .Index(t => t.LocationTypeId);
            
            CreateTable(
                "dbo.LocationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        PinImage = c.String(maxLength: 255),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProtectedDocumentCompanyTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProtectedDocumentId = c.Int(nullable: false),
                        CompanyTypeId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CompanyTypes", t => t.CompanyTypeId)
                .ForeignKey("dbo.ProtectedDocuments", t => t.ProtectedDocumentId)
                .Index(t => t.ProtectedDocumentId)
                .Index(t => t.CompanyTypeId);
            
            CreateTable(
                "dbo.Toolbars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 512),
                        BasicToolbar = c.String(),
                        PageToolbar = c.String(),
                        IsDefault = c.Boolean(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NewsReads",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NewsId = c.Int(nullable: false),
                        AnonymousContactId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnonymousContacts", t => t.AnonymousContactId)
                .ForeignKey("dbo.News", t => t.NewsId)
                .Index(t => t.NewsId)
                .Index(t => t.AnonymousContactId);
            
            CreateTable(
                "dbo.News",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Abstract = c.String(),
                        Content = c.String(),
                        DateStart = c.DateTime(),
                        DateEnd = c.DateTime(),
                        ImageUrl = c.String(maxLength: 512),
                        Status = c.Int(nullable: false),
                        IsHotNews = c.Boolean(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NewsNewsCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NewsId = c.Int(nullable: false),
                        NewsCategoryId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.News", t => t.NewsId)
                .ForeignKey("dbo.NewsCategories", t => t.NewsCategoryId)
                .Index(t => t.NewsId)
                .Index(t => t.NewsCategoryId);
            
            CreateTable(
                "dbo.NewsCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 512),
                        Abstract = c.String(),
                        ParentId = c.Int(),
                        Hierarchy = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NewsCategories", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.RemoteAuthentications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ServiceUrl = c.String(),
                        AuthorizeCode = c.String(),
                        Active = c.Boolean(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BackgroundTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        ScheduleType = c.Int(nullable: false),
                        Interval = c.Int(),
                        StartTime = c.Time(precision: 7),
                        Status = c.Int(nullable: false),
                        LastRunningTime = c.DateTime(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Banners",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImageUrl = c.String(maxLength: 1024),
                        Text = c.String(),
                        Url = c.String(maxLength: 512),
                        GroupName = c.String(maxLength: 512),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CampaignCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 200),
                        Description = c.String(),
                        TargetCount = c.Int(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 512),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmailAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(maxLength: 512),
                        DisplayName = c.String(maxLength: 512),
                        Host = c.String(maxLength: 512),
                        Port = c.Int(nullable: false),
                        Username = c.String(maxLength: 512),
                        Password = c.String(maxLength: 512),
                        EnableSsl = c.Boolean(nullable: false),
                        UseDefaultCredentials = c.Boolean(nullable: false),
                        TimeOut = c.Int(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmailLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Priority = c.Int(nullable: false),
                        From = c.String(maxLength: 500),
                        FromName = c.String(maxLength: 500),
                        To = c.String(maxLength: 500),
                        ToName = c.String(maxLength: 500),
                        CC = c.String(maxLength: 500),
                        Bcc = c.String(maxLength: 500),
                        Subject = c.String(maxLength: 1000),
                        Body = c.String(),
                        SendLater = c.DateTime(),
                        SentTries = c.Int(nullable: false),
                        SentOn = c.DateTime(),
                        Message = c.String(),
                        Attachment = c.String(),
                        EmailAccountId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailAccounts", t => t.EmailAccountId)
                .Index(t => t.EmailAccountId);
            
            CreateTable(
                "dbo.EmailTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Subject = c.String(),
                        From = c.String(),
                        FromName = c.String(),
                        CC = c.String(),
                        BCC = c.String(),
                        Body = c.String(),
                        DataType = c.String(),
                        Type = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EventCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 512),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EventEventCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventId = c.Int(nullable: false),
                        EventCategoryId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.EventId)
                .ForeignKey("dbo.EventCategories", t => t.EventCategoryId)
                .Index(t => t.EventId)
                .Index(t => t.EventCategoryId);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        EventSummary = c.String(),
                        EventDescription = c.String(),
                        MaxAttendees = c.Int(),
                        RegistrationFullText = c.String(),
                        RegistrationWaiver = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EventSchedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Location = c.String(),
                        TimeStart = c.DateTime(nullable: false),
                        TimeEnd = c.DateTime(),
                        MaxAttendees = c.Int(),
                        EventId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.EventId)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.FavouriteNavigations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NavigationId = c.String(),
                        UserId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FormComponentFields",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FormComponentId = c.Int(nullable: false),
                        Name = c.String(maxLength: 60),
                        Attributes = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FormComponents", t => t.FormComponentId)
                .Index(t => t.FormComponentId);
            
            CreateTable(
                "dbo.FormComponents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 60),
                        FormTabId = c.Int(nullable: false),
                        FormComponentTemplateId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FormComponentTemplates", t => t.FormComponentTemplateId)
                .ForeignKey("dbo.FormTabs", t => t.FormTabId)
                .Index(t => t.FormTabId)
                .Index(t => t.FormComponentTemplateId);
            
            CreateTable(
                "dbo.FormComponentTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 60),
                        Content = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FormDefaultComponents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 60),
                        FormComponentTemplateId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FormComponentTemplates", t => t.FormComponentTemplateId)
                .Index(t => t.FormComponentTemplateId);
            
            CreateTable(
                "dbo.FormDefaultComponentFields",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FormDefaultComponentId = c.Int(nullable: false),
                        Name = c.String(maxLength: 60),
                        Attributes = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FormDefaultComponents", t => t.FormDefaultComponentId)
                .Index(t => t.FormDefaultComponentId);
            
            CreateTable(
                "dbo.FormTabs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 60),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Forms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Content = c.String(),
                        JsonContent = c.String(),
                        Active = c.Boolean(nullable: false),
                        StyleId = c.Int(),
                        FromName = c.String(),
                        FromEmail = c.String(),
                        ThankyouMessage = c.String(),
                        AllowAjaxSubmit = c.Boolean(nullable: false),
                        SendSubmitFormEmail = c.Boolean(nullable: false),
                        EmailTo = c.String(),
                        SendNotificationEmail = c.Boolean(nullable: false),
                        NotificationSubject = c.String(),
                        NotificationBody = c.String(),
                        NotificationEmailTo = c.String(),
                        SendAutoResponse = c.Boolean(nullable: false),
                        AutoResponseSubject = c.String(),
                        AutoResponseBody = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Styles", t => t.StyleId)
                .Index(t => t.StyleId);
            
            CreateTable(
                "dbo.Styles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 512),
                        CdnUrl = c.String(),
                        Content = c.String(),
                        IncludeIntoEditor = c.Boolean(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StyleLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SessionId = c.String(maxLength: 512),
                        StyleId = c.Int(nullable: false),
                        Name = c.String(maxLength: 512),
                        Content = c.String(),
                        CdnUrl = c.String(),
                        IncludeIntoEditor = c.Boolean(nullable: false),
                        ChangeLog = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Styles", t => t.StyleId)
                .Index(t => t.StyleId);
            
            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 512),
                        Culture = c.String(maxLength: 512),
                        Key = c.String(maxLength: 10),
                        IsDefault = c.Boolean(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LocalizedResources",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LanguageId = c.Int(nullable: false),
                        TextKey = c.String(),
                        DefaultValue = c.String(),
                        TranslatedValue = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Languages", t => t.LanguageId)
                .Index(t => t.LanguageId);
            
            CreateTable(
                "dbo.SlideInHelps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LanguageId = c.Int(nullable: false),
                        TextKey = c.String(),
                        HelpTitle = c.String(),
                        LocalHelpContent = c.String(),
                        MasterHelpContent = c.String(),
                        MasterVersion = c.Int(nullable: false),
                        LocalVersion = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Languages", t => t.LanguageId)
                .Index(t => t.LanguageId);
            
            CreateTable(
                "dbo.Links",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 512),
                        Url = c.String(),
                        UrlTarget = c.String(),
                        Description = c.String(),
                        LinkTypeId = c.Int(nullable: false),
                        DateStart = c.DateTime(),
                        DateEnd = c.DateTime(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LinkTypes", t => t.LinkTypeId)
                .Index(t => t.LinkTypeId);
            
            CreateTable(
                "dbo.LinkTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 512),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Notices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(maxLength: 512),
                        IsUrgent = c.Boolean(nullable: false),
                        NoticeTypeId = c.Int(nullable: false),
                        DateStart = c.DateTime(),
                        DateEnd = c.DateTime(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NoticeTypes", t => t.NoticeTypeId)
                .Index(t => t.NoticeTypeId);
            
            CreateTable(
                "dbo.NoticeTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Parameters = c.String(),
                        ContactQueries = c.String(),
                        NotifiedContacts = c.String(),
                        Module = c.Int(nullable: false),
                        NotificationSubject = c.String(),
                        NotificationBody = c.String(),
                        SendTime = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NotificationTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Subject = c.String(),
                        Body = c.String(),
                        IsDefaultTemplate = c.Boolean(nullable: false),
                        Module = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PollAnswers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AnswerText = c.String(),
                        Total = c.Int(nullable: false),
                        PollId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Polls", t => t.PollId)
                .Index(t => t.PollId);
            
            CreateTable(
                "dbo.Polls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PollQuestion = c.String(),
                        PollSummary = c.String(),
                        IsMultiple = c.Boolean(nullable: false),
                        ThankyouText = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductOfInterests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 200),
                        TargetCount = c.Int(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProtectedDocumentLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Path = c.String(),
                        UserId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.RotatingImageGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Settings = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RotatingImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 512),
                        ImageUrl = c.String(maxLength: 512),
                        Text = c.String(),
                        Url = c.String(maxLength: 512),
                        UrlTarget = c.String(),
                        GroupId = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RotatingImageGroups", t => t.GroupId)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.RssFeeds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 512),
                        Url = c.String(maxLength: 512),
                        RssType = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ScriptLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SessionId = c.String(maxLength: 512),
                        ScriptId = c.Int(nullable: false),
                        Name = c.String(maxLength: 512),
                        Content = c.String(),
                        DataType = c.String(maxLength: 512),
                        ChangeLog = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Scripts", t => t.ScriptId)
                .Index(t => t.ScriptId);
            
            CreateTable(
                "dbo.Scripts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 512),
                        Content = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Services",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Content = c.String(),
                        ImageUrl = c.String(maxLength: 512),
                        Status = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SiteSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 512),
                        Description = c.String(),
                        FieldName = c.String(),
                        Value = c.String(),
                        EditorType = c.Int(nullable: false),
                        SettingType = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SocialMedia",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        MaxCharacter = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SocialMediaTokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SocialMediaId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        AppId = c.String(maxLength: 255),
                        AppSecret = c.String(maxLength: 255),
                        IsDefault = c.Boolean(nullable: false),
                        FullName = c.String(maxLength: 255),
                        Email = c.String(maxLength: 255),
                        AccessToken = c.String(),
                        AccessTokenSecret = c.String(),
                        Verifier = c.String(),
                        ExpiredDate = c.DateTime(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SocialMedia", t => t.SocialMediaId)
                .Index(t => t.SocialMediaId);
            
            CreateTable(
                "dbo.SocialMediaLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SocialMediaId = c.Int(nullable: false),
                        SocialMediaTokenId = c.Int(nullable: false),
                        PageId = c.Int(nullable: false),
                        PostedContent = c.String(),
                        PostedResponse = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pages", t => t.PageId)
                .ForeignKey("dbo.SocialMediaTokens", t => t.SocialMediaTokenId)
                .Index(t => t.SocialMediaTokenId)
                .Index(t => t.PageId);
            
            CreateTable(
                "dbo.SubscriptionLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Parameters = c.String(),
                        Module = c.Int(nullable: false),
                        ChangeLog = c.String(),
                        IsNightlySent = c.Boolean(nullable: false),
                        IsDirectlySent = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SubscriptionTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Body = c.String(),
                        Module = c.Int(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Testimonials",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Author = c.String(maxLength: 512),
                        AuthorDescription = c.String(maxLength: 512),
                        AuthorImageUrl = c.String(maxLength: 512),
                        Content = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WidgetTemplateLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SessionId = c.String(maxLength: 512),
                        TemplateId = c.Int(nullable: false),
                        Name = c.String(maxLength: 512),
                        Widgets = c.String(),
                        Content = c.String(),
                        Script = c.String(),
                        Style = c.String(),
                        FullContent = c.String(),
                        DataType = c.String(maxLength: 512),
                        ChangeLog = c.String(),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WidgetTemplates", t => t.TemplateId)
                .Index(t => t.TemplateId);
            
            CreateTable(
                "dbo.WidgetTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 512),
                        Widgets = c.String(),
                        Content = c.String(),
                        Script = c.String(),
                        Style = c.String(),
                        FullContent = c.String(),
                        DataType = c.String(maxLength: 512),
                        Widget = c.String(maxLength: 512),
                        IsDefaultTemplate = c.Boolean(nullable: false),
                        RecordOrder = c.Int(nullable: false),
                        RecordDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 100),
                        LastUpdate = c.DateTime(),
                        LastUpdateBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WidgetTemplateLogs", "TemplateId", "dbo.WidgetTemplates");
            DropForeignKey("dbo.SocialMediaLogs", "SocialMediaTokenId", "dbo.SocialMediaTokens");
            DropForeignKey("dbo.SocialMediaLogs", "PageId", "dbo.Pages");
            DropForeignKey("dbo.SocialMediaTokens", "SocialMediaId", "dbo.SocialMedia");
            DropForeignKey("dbo.ScriptLogs", "ScriptId", "dbo.Scripts");
            DropForeignKey("dbo.RotatingImages", "GroupId", "dbo.RotatingImageGroups");
            DropForeignKey("dbo.ProtectedDocumentLogs", "UserId", "dbo.Users");
            DropForeignKey("dbo.PollAnswers", "PollId", "dbo.Polls");
            DropForeignKey("dbo.Notices", "NoticeTypeId", "dbo.NoticeTypes");
            DropForeignKey("dbo.Links", "LinkTypeId", "dbo.LinkTypes");
            DropForeignKey("dbo.SlideInHelps", "LanguageId", "dbo.Languages");
            DropForeignKey("dbo.LocalizedResources", "LanguageId", "dbo.Languages");
            DropForeignKey("dbo.StyleLogs", "StyleId", "dbo.Styles");
            DropForeignKey("dbo.Forms", "StyleId", "dbo.Styles");
            DropForeignKey("dbo.FormComponents", "FormTabId", "dbo.FormTabs");
            DropForeignKey("dbo.FormDefaultComponentFields", "FormDefaultComponentId", "dbo.FormDefaultComponents");
            DropForeignKey("dbo.FormDefaultComponents", "FormComponentTemplateId", "dbo.FormComponentTemplates");
            DropForeignKey("dbo.FormComponents", "FormComponentTemplateId", "dbo.FormComponentTemplates");
            DropForeignKey("dbo.FormComponentFields", "FormComponentId", "dbo.FormComponents");
            DropForeignKey("dbo.EventEventCategories", "EventCategoryId", "dbo.EventCategories");
            DropForeignKey("dbo.EventSchedules", "EventId", "dbo.Events");
            DropForeignKey("dbo.EventEventCategories", "EventId", "dbo.Events");
            DropForeignKey("dbo.EmailLogs", "EmailAccountId", "dbo.EmailAccounts");
            DropForeignKey("dbo.NewsReads", "NewsId", "dbo.News");
            DropForeignKey("dbo.NewsNewsCategories", "NewsCategoryId", "dbo.NewsCategories");
            DropForeignKey("dbo.NewsCategories", "ParentId", "dbo.NewsCategories");
            DropForeignKey("dbo.NewsNewsCategories", "NewsId", "dbo.News");
            DropForeignKey("dbo.NewsReads", "AnonymousContactId", "dbo.AnonymousContacts");
            DropForeignKey("dbo.UserUserGroups", "UserGroupId", "dbo.UserGroups");
            DropForeignKey("dbo.UserGroups", "ToolbarId", "dbo.Toolbars");
            DropForeignKey("dbo.ProtectedDocumentGroups", "UserGroupId", "dbo.UserGroups");
            DropForeignKey("dbo.ProtectedDocumentGroups", "ProtectedDocumentId", "dbo.ProtectedDocuments");
            DropForeignKey("dbo.ProtectedDocumentCompanies", "ProtectedDocumentId", "dbo.ProtectedDocuments");
            DropForeignKey("dbo.ProtectedDocumentCompanies", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.ProtectedDocumentCompanyTypes", "ProtectedDocumentId", "dbo.ProtectedDocuments");
            DropForeignKey("dbo.ProtectedDocumentCompanyTypes", "CompanyTypeId", "dbo.CompanyTypes");
            DropForeignKey("dbo.Companies", "CompanyTypeId", "dbo.CompanyTypes");
            DropForeignKey("dbo.AssociateCompanyTypes", "CompanyTypeId", "dbo.CompanyTypes");
            DropForeignKey("dbo.LocationLocationTypes", "LocationTypeId", "dbo.LocationTypes");
            DropForeignKey("dbo.LocationLocationTypes", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.AssociateLocations", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.AssociateLocations", "AssociateId", "dbo.Associates");
            DropForeignKey("dbo.AssociateCompanyTypes", "AssociateId", "dbo.Associates");
            DropForeignKey("dbo.AssociateAssociateTypes", "AssociateTypeId", "dbo.AssociateTypes");
            DropForeignKey("dbo.AssociateAssociateTypes", "AssociateId", "dbo.Associates");
            DropForeignKey("dbo.PageSecurities", "GroupId", "dbo.UserGroups");
            DropForeignKey("dbo.PageTags", "TagId", "dbo.Tags");
            DropForeignKey("dbo.PageTags", "PageId", "dbo.Pages");
            DropForeignKey("dbo.PageSecurities", "PageId", "dbo.Pages");
            DropForeignKey("dbo.PageReads", "PageId", "dbo.Pages");
            DropForeignKey("dbo.PageReads", "AnonymousContactId", "dbo.AnonymousContacts");
            DropForeignKey("dbo.PageLogs", "PageId", "dbo.Pages");
            DropForeignKey("dbo.LinkTrackers", "PageId", "dbo.Pages");
            DropForeignKey("dbo.LinkTrackerClicks", "LinkTrackerId", "dbo.LinkTrackers");
            DropForeignKey("dbo.PageTemplateLogs", "PageTemplateId", "dbo.PageTemplates");
            DropForeignKey("dbo.Pages", "PageTemplateId", "dbo.PageTemplates");
            DropForeignKey("dbo.FileTemplates", "PageTemplateId", "dbo.PageTemplates");
            DropForeignKey("dbo.PageTemplates", "ParentId", "dbo.PageTemplates");
            DropForeignKey("dbo.Pages", "FileTemplateId", "dbo.FileTemplates");
            DropForeignKey("dbo.ClientNavigations", "PageId", "dbo.Pages");
            DropForeignKey("dbo.ClientNavigations", "ParentId", "dbo.ClientNavigations");
            DropForeignKey("dbo.Pages", "ParentId", "dbo.Pages");
            DropForeignKey("dbo.Pages", "BodyTemplateId", "dbo.BodyTemplates");
            DropForeignKey("dbo.GroupPermissions", "UserGroupId", "dbo.UserGroups");
            DropForeignKey("dbo.UserUserGroups", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserLoginHistories", "UserId", "dbo.Users");
            DropForeignKey("dbo.Contacts", "UserId", "dbo.Users");
            DropForeignKey("dbo.Subscriptions", "ContactId", "dbo.Contacts");
            DropForeignKey("dbo.ContactGroupContacts", "ContactGroupId", "dbo.ContactGroups");
            DropForeignKey("dbo.ContactGroupContacts", "ContactId", "dbo.Contacts");
            DropForeignKey("dbo.ContactCommunications", "ContactId", "dbo.Contacts");
            DropForeignKey("dbo.AnonymousContacts", "ContactId", "dbo.Contacts");
            DropIndex("dbo.WidgetTemplateLogs", new[] { "TemplateId" });
            DropIndex("dbo.SocialMediaLogs", new[] { "PageId" });
            DropIndex("dbo.SocialMediaLogs", new[] { "SocialMediaTokenId" });
            DropIndex("dbo.SocialMediaTokens", new[] { "SocialMediaId" });
            DropIndex("dbo.ScriptLogs", new[] { "ScriptId" });
            DropIndex("dbo.RotatingImages", new[] { "GroupId" });
            DropIndex("dbo.ProtectedDocumentLogs", new[] { "UserId" });
            DropIndex("dbo.PollAnswers", new[] { "PollId" });
            DropIndex("dbo.Notices", new[] { "NoticeTypeId" });
            DropIndex("dbo.Links", new[] { "LinkTypeId" });
            DropIndex("dbo.SlideInHelps", new[] { "LanguageId" });
            DropIndex("dbo.LocalizedResources", new[] { "LanguageId" });
            DropIndex("dbo.StyleLogs", new[] { "StyleId" });
            DropIndex("dbo.Forms", new[] { "StyleId" });
            DropIndex("dbo.FormDefaultComponentFields", new[] { "FormDefaultComponentId" });
            DropIndex("dbo.FormDefaultComponents", new[] { "FormComponentTemplateId" });
            DropIndex("dbo.FormComponents", new[] { "FormComponentTemplateId" });
            DropIndex("dbo.FormComponents", new[] { "FormTabId" });
            DropIndex("dbo.FormComponentFields", new[] { "FormComponentId" });
            DropIndex("dbo.EventSchedules", new[] { "EventId" });
            DropIndex("dbo.EventEventCategories", new[] { "EventCategoryId" });
            DropIndex("dbo.EventEventCategories", new[] { "EventId" });
            DropIndex("dbo.EmailLogs", new[] { "EmailAccountId" });
            DropIndex("dbo.NewsCategories", new[] { "ParentId" });
            DropIndex("dbo.NewsNewsCategories", new[] { "NewsCategoryId" });
            DropIndex("dbo.NewsNewsCategories", new[] { "NewsId" });
            DropIndex("dbo.NewsReads", new[] { "AnonymousContactId" });
            DropIndex("dbo.NewsReads", new[] { "NewsId" });
            DropIndex("dbo.ProtectedDocumentCompanyTypes", new[] { "CompanyTypeId" });
            DropIndex("dbo.ProtectedDocumentCompanyTypes", new[] { "ProtectedDocumentId" });
            DropIndex("dbo.LocationLocationTypes", new[] { "LocationTypeId" });
            DropIndex("dbo.LocationLocationTypes", new[] { "LocationId" });
            DropIndex("dbo.AssociateLocations", new[] { "LocationId" });
            DropIndex("dbo.AssociateLocations", new[] { "AssociateId" });
            DropIndex("dbo.AssociateAssociateTypes", new[] { "AssociateTypeId" });
            DropIndex("dbo.AssociateAssociateTypes", new[] { "AssociateId" });
            DropIndex("dbo.AssociateCompanyTypes", new[] { "CompanyTypeId" });
            DropIndex("dbo.AssociateCompanyTypes", new[] { "AssociateId" });
            DropIndex("dbo.Companies", new[] { "CompanyTypeId" });
            DropIndex("dbo.ProtectedDocumentCompanies", new[] { "CompanyId" });
            DropIndex("dbo.ProtectedDocumentCompanies", new[] { "ProtectedDocumentId" });
            DropIndex("dbo.ProtectedDocumentGroups", new[] { "UserGroupId" });
            DropIndex("dbo.ProtectedDocumentGroups", new[] { "ProtectedDocumentId" });
            DropIndex("dbo.PageTags", new[] { "TagId" });
            DropIndex("dbo.PageTags", new[] { "PageId" });
            DropIndex("dbo.PageReads", new[] { "AnonymousContactId" });
            DropIndex("dbo.PageReads", new[] { "PageId" });
            DropIndex("dbo.PageLogs", new[] { "PageId" });
            DropIndex("dbo.LinkTrackerClicks", new[] { "LinkTrackerId" });
            DropIndex("dbo.LinkTrackers", new[] { "PageId" });
            DropIndex("dbo.PageTemplateLogs", new[] { "PageTemplateId" });
            DropIndex("dbo.PageTemplates", new[] { "ParentId" });
            DropIndex("dbo.FileTemplates", new[] { "PageTemplateId" });
            DropIndex("dbo.ClientNavigations", new[] { "ParentId" });
            DropIndex("dbo.ClientNavigations", new[] { "PageId" });
            DropIndex("dbo.Pages", new[] { "ParentId" });
            DropIndex("dbo.Pages", new[] { "BodyTemplateId" });
            DropIndex("dbo.Pages", new[] { "FileTemplateId" });
            DropIndex("dbo.Pages", new[] { "PageTemplateId" });
            DropIndex("dbo.PageSecurities", new[] { "GroupId" });
            DropIndex("dbo.PageSecurities", new[] { "PageId" });
            DropIndex("dbo.GroupPermissions", new[] { "UserGroupId" });
            DropIndex("dbo.UserGroups", new[] { "ToolbarId" });
            DropIndex("dbo.UserUserGroups", new[] { "UserGroupId" });
            DropIndex("dbo.UserUserGroups", new[] { "UserId" });
            DropIndex("dbo.UserLoginHistories", new[] { "UserId" });
            DropIndex("dbo.Subscriptions", new[] { "ContactId" });
            DropIndex("dbo.ContactGroupContacts", new[] { "ContactGroupId" });
            DropIndex("dbo.ContactGroupContacts", new[] { "ContactId" });
            DropIndex("dbo.ContactCommunications", new[] { "ContactId" });
            DropIndex("dbo.Contacts", new[] { "UserId" });
            DropIndex("dbo.AnonymousContacts", new[] { "ContactId" });
            DropTable("dbo.WidgetTemplates");
            DropTable("dbo.WidgetTemplateLogs");
            DropTable("dbo.Testimonials");
            DropTable("dbo.SubscriptionTemplates");
            DropTable("dbo.SubscriptionLogs");
            DropTable("dbo.SocialMediaLogs");
            DropTable("dbo.SocialMediaTokens");
            DropTable("dbo.SocialMedia");
            DropTable("dbo.SiteSettings");
            DropTable("dbo.Services");
            DropTable("dbo.Scripts");
            DropTable("dbo.ScriptLogs");
            DropTable("dbo.RssFeeds");
            DropTable("dbo.RotatingImages");
            DropTable("dbo.RotatingImageGroups");
            DropTable("dbo.ProtectedDocumentLogs");
            DropTable("dbo.ProductOfInterests");
            DropTable("dbo.Polls");
            DropTable("dbo.PollAnswers");
            DropTable("dbo.NotificationTemplates");
            DropTable("dbo.Notifications");
            DropTable("dbo.NoticeTypes");
            DropTable("dbo.Notices");
            DropTable("dbo.LinkTypes");
            DropTable("dbo.Links");
            DropTable("dbo.SlideInHelps");
            DropTable("dbo.LocalizedResources");
            DropTable("dbo.Languages");
            DropTable("dbo.StyleLogs");
            DropTable("dbo.Styles");
            DropTable("dbo.Forms");
            DropTable("dbo.FormTabs");
            DropTable("dbo.FormDefaultComponentFields");
            DropTable("dbo.FormDefaultComponents");
            DropTable("dbo.FormComponentTemplates");
            DropTable("dbo.FormComponents");
            DropTable("dbo.FormComponentFields");
            DropTable("dbo.FavouriteNavigations");
            DropTable("dbo.EventSchedules");
            DropTable("dbo.Events");
            DropTable("dbo.EventEventCategories");
            DropTable("dbo.EventCategories");
            DropTable("dbo.EmailTemplates");
            DropTable("dbo.EmailLogs");
            DropTable("dbo.EmailAccounts");
            DropTable("dbo.Countries");
            DropTable("dbo.CampaignCodes");
            DropTable("dbo.Banners");
            DropTable("dbo.BackgroundTasks");
            DropTable("dbo.RemoteAuthentications");
            DropTable("dbo.NewsCategories");
            DropTable("dbo.NewsNewsCategories");
            DropTable("dbo.News");
            DropTable("dbo.NewsReads");
            DropTable("dbo.Toolbars");
            DropTable("dbo.ProtectedDocumentCompanyTypes");
            DropTable("dbo.LocationTypes");
            DropTable("dbo.LocationLocationTypes");
            DropTable("dbo.Locations");
            DropTable("dbo.AssociateLocations");
            DropTable("dbo.AssociateTypes");
            DropTable("dbo.AssociateAssociateTypes");
            DropTable("dbo.Associates");
            DropTable("dbo.AssociateCompanyTypes");
            DropTable("dbo.CompanyTypes");
            DropTable("dbo.Companies");
            DropTable("dbo.ProtectedDocumentCompanies");
            DropTable("dbo.ProtectedDocuments");
            DropTable("dbo.ProtectedDocumentGroups");
            DropTable("dbo.Tags");
            DropTable("dbo.PageTags");
            DropTable("dbo.PageReads");
            DropTable("dbo.PageLogs");
            DropTable("dbo.LinkTrackerClicks");
            DropTable("dbo.LinkTrackers");
            DropTable("dbo.PageTemplateLogs");
            DropTable("dbo.PageTemplates");
            DropTable("dbo.FileTemplates");
            DropTable("dbo.ClientNavigations");
            DropTable("dbo.BodyTemplates");
            DropTable("dbo.Pages");
            DropTable("dbo.PageSecurities");
            DropTable("dbo.GroupPermissions");
            DropTable("dbo.UserGroups");
            DropTable("dbo.UserUserGroups");
            DropTable("dbo.UserLoginHistories");
            DropTable("dbo.Users");
            DropTable("dbo.Subscriptions");
            DropTable("dbo.ContactGroups");
            DropTable("dbo.ContactGroupContacts");
            DropTable("dbo.ContactCommunications");
            DropTable("dbo.Contacts");
            DropTable("dbo.AnonymousContacts");
        }
    }
}
