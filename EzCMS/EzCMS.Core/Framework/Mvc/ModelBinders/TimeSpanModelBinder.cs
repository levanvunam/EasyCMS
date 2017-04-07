using Ez.Framework.Utilities.Time;
using EzCMS.Core.Framework.Context;
using System;
using System.Web.Mvc;

namespace EzCMS.Core.Framework.Mvc.ModelBinders
{
    public class TimeSpanModelBinder : DefaultModelBinder
    {
        #region Explicit Interface Methods

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            try
            {
                if (value != null)
                {
                    var datetime = DateTime.ParseExact(value.AttemptedValue, DateTimeUtilities.TimeFormat(), null);
                    datetime = datetime.ToUTCTime(WorkContext.CurrentTimezone);
                    return new TimeSpan(datetime.Hour, datetime.Minute, datetime.Second);
                }
            }
            catch (Exception)
            {
                //Invalid
                return null;
            }
            return base.BindModel(controllerContext, bindingContext);
        }

        #endregion
    }
}