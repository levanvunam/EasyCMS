using System.ComponentModel;

namespace Ez.Framework.Utilities.Social.Enums
{
    public class SocialMediaEnums
    {
        public enum SocialNetwork
        {
            Facebook = 1,
            Twitter = 2,
            LinkedIn = 3
        }

        public enum TokenStatus
        {
            Pending,
            Active,
            Expired
        }

        public enum FacebookPermission
        {
            [Description("public_profile")]
            PublicProfile,
            [Description("user_friends")]
            UserFriends,
            [Description("email")]
            Email,
            [Description("user_about_me")]
            UserAboutMe,
            [Description("user_actions.books")]
            UserActionBooks,
            [Description("user_actions.fitness")]
            UserActionsFitness,
            [Description("user_actions.music")]
            UserActionsMusic,
            [Description("user_actions.news")]
            UserActionsNews,
            [Description("user_actions.video")]
            UserActionsVideo,
            //user_actions:{app_namespace}
            [Description("user_birthday")]
            UserBirthday,
            [Description("user_education_history")]
            UserEducationHistory,
            [Description("user_events")]
            UserEvents,
            [Description("user_games_activity")]
            UserGamesActivity,
            [Description("user_hometown")]
            UserHometown,
            [Description("user_likes")]
            UserLikes,
            [Description("user_location")]
            UserLocation,
            [Description("user_managed_groups")]
            UserManagedGroups,
            [Description("user_photos")]
            UserPhotos,
            [Description("user_posts")]
            UserPosts,
            [Description("user_relationships")]
            UserRelationships,
            [Description("user_relationship_details")]
            UserRelationshipDetails,
            [Description("user_religion_politics")]
            UserReligionPolitics,
            [Description("user_tagged_places")]
            UserTaggedPlaces,
            [Description("user_videos")]
            UserVideos,
            [Description("user_website")]
            UserWebsite,
            [Description("user_work_history")]
            UserWorkHistory,
            [Description("read_customWriendlists")]
            ReadCustomFriendlists,
            [Description("read_insights")]
            ReadInsights,
            [Description("read_audience_network_insights")]
            ReadAudienceNetworkInsights,
            [Description("read_page_mailboxes")]
            ReadPageMailboxes,
            [Description("manage_pages")]
            ManagePages,
            [Description("publish_pages")]
            PublishPages,
            [Description("publish_actions")]
            PublishActions,
            [Description("rsvp_event")]
            RsvpEvent,
            [Description("pages_show_list")]
            PagesShowList,
            [Description("pages_manage_cta")]
            PagesManageCta,
            [Description("pages_manage_leads")]
            PagesManageLeads,
            [Description("ads_read")]
            AdsRead,
            [Description("ads_management")]
            AdsManagement
        }
    }
}
