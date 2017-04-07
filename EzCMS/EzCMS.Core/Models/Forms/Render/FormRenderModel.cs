using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Html;
using Ez.Framework.Utilities.Reflection;
using Ez.Framework.Utilities.Reflection.Enums;
using Ez.Framework.IoC;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Services.Forms;
using EzCMS.Core.Services.Styles;
using EzCMS.Entity.Entities.Models;
using HtmlAgilityPack;
using System;
using System.Globalization;
using System.Linq;
using Ez.Framework.Core.IoC;

namespace EzCMS.Core.Models.Forms.Render
{
    public class FormRenderModel
    {
        public FormRenderModel()
        {

        }

        public FormRenderModel(Form form)
            : this()
        {
            var styleService = HostContainer.GetInstance<IStyleService>();
            var formService = HostContainer.GetInstance<IFormService>();

            Style = styleService.GetStyleUrl(form.StyleId);

            Id = form.Id;
            EncryptId = PasswordUtilities.ComplexEncrypt(Id.ToString(CultureInfo.InvariantCulture));
            Content = formService.ParseForm(form.Name, form.Content);

            #region Adding current contact information

            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(Content);

                if (document.DocumentNode != null)
                {
                    var modelProperties = typeof(ContactCookieModel).GetPropertyListFromType(1, string.Empty, PropertyKind.Value).Select(p => p.Name).ToList();
                    HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//input");

                    for (int i = 0; i < nodes.Count; i++)
                    {
                        HtmlNode node = nodes[i];
                        foreach (var modelProperty in modelProperties)
                        {
                            if (node.Attributes["name"] != null && node.Attributes["name"].Value.Equals(modelProperty, StringComparison.CurrentCultureIgnoreCase))
                            {
                                var propertyValue = WorkContext.CurrentContact.GetPropertyValue(modelProperty) != null
                                    ? WorkContext.CurrentContact.GetPropertyValue(modelProperty).ToString()
                                    : String.Empty;
                                if (node.Attributes["value"] == null)
                                {
                                    node.Attributes.Add("value", propertyValue);
                                }
                                else
                                {
                                    node.Attributes["value"].Value = propertyValue;
                                }
                            }
                        }
                    }

                    Content = document.DocumentNode.OuterHtml;
                }

            }
            catch (Exception)
            {
                //May have some problems when parsing the html
            }

            #endregion

            ThankyouMessage = form.ThankyouMessage.RemoveNewLine();
        }

        #region Public Properties

        public int Id { get; set; }

        public string EncryptId { get; set; }

        public string Style { get; set; }

        public string Content { get; set; }

        public string ThankyouMessage { get; set; }

        #endregion
    }
}