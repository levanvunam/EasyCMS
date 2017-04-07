using Ez.Framework.Configurations;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using System.Web.Mvc;

namespace Ez.Framework.Core.Mvc.Controllers
{
    /// <summary>
    /// Administrator base controller
    /// </summary>
    public class EzAdministratorController : EzBaseController
    {
        #region Protected Methods

        /// <summary>
        /// Set view bag for popup action
        /// </summary>
        protected void SetupPopupAction()
        {
            ViewBag.IsPopup = true;
        }

        #endregion

        #region Message

        [NonAction]
        public void SetResponseMessage(ResponseModel response)
        {
            TempData[FrameworkConstants.EzMessageResponse] = response;
        }

        [NonAction]
        public void SetErrorMessage(string message)
        {
            TempData[FrameworkConstants.EzErrorMessage] = message;
        }

        [NonAction]
        public void SetSuccessMessage(string message)
        {
            TempData[FrameworkConstants.EzSuccessMessage] = message;
        }

        [NonAction]
        public void SetWarningMessage(string message)
        {
            TempData[FrameworkConstants.EzWarningMessage] = message;
        }

        #endregion
    }
}
