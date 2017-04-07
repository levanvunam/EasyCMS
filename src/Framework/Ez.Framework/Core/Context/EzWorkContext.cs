using Ez.Framework.Configurations;
using Ez.Framework.Core.Navigations;
using Ez.Framework.Utilities;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Ez.Framework.Core.Context
{
    /// <summary>
    /// Ez work context
    /// </summary>
    public class EzWorkContext
    {
        #region System

        /// <summary>
        /// Flag for checking if system changed and need to restart
        /// </summary>
        public static bool IsSystemChanged
        {
            get
            {
                return StateManager.GetApplication(FrameworkConstants.IsSystemChanged, false);
            }
            set
            {
                StateManager.SetApplication(FrameworkConstants.IsSystemChanged, value);
            }
        }

        /// <summary>
        /// Site url
        /// </summary>
        public static string SiteUrl
        {
            get
            {
                return "/".ToHttpAbsolute();
            }
        }

        #endregion

        #region MVC

        /// <summary>
        /// Current controller context
        /// </summary>
        public static ControllerContext CurrentControllerContext
        {
            get
            {
                return StateManager.GetContextItem<ControllerContext>(FrameworkConstants.CurrentControllerContext);
            }
            set
            {
                StateManager.SetContextItem(FrameworkConstants.CurrentControllerContext, value);
            }
        }

        /// <summary>
        /// Current controller action
        /// </summary>
        public static string CurrentControllerAction
        {
            get
            {
                return StateManager.GetContextItem<string>(FrameworkConstants.CurrentControllerAction);
            }
            set
            {
                StateManager.SetContextItem(FrameworkConstants.CurrentControllerAction, value);
            }
        }

        #endregion

        #region User information

        /// <summary>
        /// Check if current user browser enable cookie or not
        /// </summary>
        public static bool EnableCookie
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Navigation

        /// <summary>
        /// Ez dynamic navigations
        /// </summary>
        public static List<Navigation> EzCMSNavigations
        {
            get
            {
                return StateManager.GetApplication<List<Navigation>>(FrameworkConstants.EzNavigations);
            }
            set
            {
                StateManager.SetApplication(FrameworkConstants.EzNavigations, value);
            }
        }

        #endregion

        public static int? CurrentUserId
        {
            get
            {
                return StateManager.GetApplication<int?>(FrameworkConstants.CurrentUserId);
            }
            set
            {
                StateManager.SetApplication(FrameworkConstants.CurrentUserId, value);
            }
        }
    }
}