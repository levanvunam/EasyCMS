using Ez.Framework.Utilities.Time;
using EzCMS.Core.Framework.Context;
using System;
using System.Threading;
using System.Web.Mvc;

namespace EzCMS.Core.Framework.Mvc.ModelBinders
{
    public class DateTimeModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var baseValue = (DateTime?)value.ConvertTo(typeof(DateTime?), Thread.CurrentThread.CurrentCulture);
            return baseValue.ToUTCTime(WorkContext.CurrentTimezone);
        }
    }
}