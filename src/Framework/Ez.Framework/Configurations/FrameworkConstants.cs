namespace Ez.Framework.Configurations
{
    public class FrameworkConstants
    {
        #region Context

        public const string EzSettings = ":::EzSettings:::";
        public const string EzNavigations = ":::EzNavigations:::";
        public const string CurrentUserId = ":::CurrentUserId:::";
        public const string IsSystemChanged = ":::IsSystemChanged:::";
        public const string CurrentControllerContext = ":::CurrentControllerContext:::";
        public const string CurrentControllerAction = ":::CurrentControllerAction:::";

        #endregion

        #region Media

        public const string MediaPath = "/Media";
        public const string ResizedFolder = "resized";

        #endregion

        #region Localize Resource

        public const string LocalizeResourceSeperator = ":::";

        #endregion

        #region Context Key

        #region Controller

        public const string EzCurrentController = ":::EzCurrentController:::";

        public const string EzMessageResponse = ":::EzMessageResponse:::";

        public const string EzErrorMessage = ":::EzErrorMessage:::";

        public const string EzSuccessMessage = ":::EzSuccessMessage:::";

        public const string EzWarningMessage = ":::EzWarningMessage:::";

        #endregion

        #endregion

        #region Entity

        public const string IdPropertyName = "Id";

        #region Hierarchy

        public const char IdSeparator = '.';
        public const string HierarchyLevelPrefix = "---";
        public const string ParentIdPropertyName = "ParentId";
        public const string HierarchyPropertyName = "Hierarchy";
        public const string HierarchyNodeFormat = "D6";

        #endregion

        public const string DefaultSystemAccount = "system";

        public const string DefaultMigrationAccount = "migrator";

        #endregion

        #region Reflection

        public const string EzSolution = "Ez";

        #endregion

        #region Common

        public const string UniqueLinkSeparator = "::::::";

        public const double Zero = 0.000000000000001;

        public const string SemicolonSeparator = "; ";
        public const string ColonSeparator = ", ";
        public const string Semicolon = ";";
        public const string Colon = ",";
        public const string Space = " ";
        public const string BreakLine = "<br />";


        public const string UniqueDateTimeFormat = "-yyyyMMddhhmmss";

        #endregion

        #region Mime

        public const string JavascriptMIME = ".js";
        public const string CssMIME = ".css";

        #endregion

        public const int DefaultEmailSendTimeout = 30000;

        public const string GoogleApikey = "AIzaSyCC96kbXkz0cQ4zjH1iFAxWdTEnSyXuSvE";
    }
}
