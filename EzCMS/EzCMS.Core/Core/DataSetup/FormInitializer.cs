using Ez.Framework.Configurations;
using System.Data.Entity;
using Ez.Framework.Core.Entity.Extensions;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Forms.FormConfigurations;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ez.Framework.Core.Entity.Intialize;

namespace EzCMS.Core.Core.DataSetup
{
    public class FormInitializer : IDataInitializer
    {
        /// <summary>
        /// Priority of initializer
        /// </summary>
        /// <returns></returns>
        public DataInitializerPriority Priority()
        {
            return DataInitializerPriority.Low;
        }

        #region Initialize

        /// <summary>
        /// Initialize default settings
        /// </summary>
        public void Initialize(DbContext ezContext)
        {
            var context = ezContext as EzCMSEntities;
            if (context != null)
            {
                if (!context.FormTabs.Any())
                {
                    var now = DateTime.UtcNow;

                    #region Save Templates

                    var embeddedResources = DataInitializeHelper.GetAllResourcesInFolder("FormBuilder.Components",
                        DataSetupResourceType.Others);

                    foreach (var embeddedResource in embeddedResources)
                    {
                        var formComponentTemplate = new FormComponentTemplate
                        {
                            Name = Path.GetFileNameWithoutExtension(embeddedResource.Name),
                            Content = embeddedResource.Content
                        };
                        context.FormComponentTemplates.AddEntities(formComponentTemplate);
                    }
                    context.SaveChanges();

                    #endregion

                    #region Save Tabs

                    var setup = DataInitializeHelper.GetResourceContent("FormBuilder.Tabs.json", DataSetupResourceType.Others);
                    var tabs = SerializeUtilities.Deserialize<List<FormTabModel>>(setup);

                    var tabOrder = 0;
                    foreach (var item in tabs)
                    {
                        tabOrder += 10;
                        var tab = new FormTab
                        {
                            Name = item.title,
                            RecordOrder = tabOrder,
                            Created = now,
                            CreatedBy = FrameworkConstants.DefaultMigrationAccount
                        };

                        var componentOrder = 0;
                        foreach (var data in item.data)
                        {
                            // Get templates for component
                            var formComponentTemplate = context.FormComponentTemplates.First(t => t.Name.Equals(data.template));

                            componentOrder += 10;
                            var component = new FormComponent
                            {
                                FormTab = tab,
                                FormComponentTemplate = formComponentTemplate,
                                Name = data.title,
                                RecordOrder = componentOrder,
                                Created = now,
                                CreatedBy = FrameworkConstants.DefaultMigrationAccount
                            };

                            if (data.fields != null && data.fields.Keys.Any())
                            {
                                var fieldOrder = 0;
                                foreach (var field in data.fields)
                                {
                                    fieldOrder += 10;
                                    var formField = new FormComponentField
                                    {
                                        FormComponent = component,
                                        Name = field.Key,
                                        Attributes = SerializeUtilities.Serialize(field.Value),
                                        RecordOrder = fieldOrder,
                                        Created = now,
                                        CreatedBy = FrameworkConstants.DefaultMigrationAccount
                                    };

                                    context.FormComponentFields.AddEntities(formField);
                                }
                            }
                            else
                            {
                                context.FormComponents.AddEntities(component);
                            }
                        }
                    }

                    #endregion

                    #region Save Default Form components

                    var componentsSetup = DataInitializeHelper.GetResourceContent("FormBuilder.DefaultFormComponents.json", DataSetupResourceType.Others);
                    var defaultFormComponents = SerializeUtilities.Deserialize<List<FormData>>(componentsSetup);

                    var defaultComponentOrder = 0;
                    foreach (var data in defaultFormComponents)
                    {
                        defaultComponentOrder += 10;

                        // Get template for component
                        var formComponentTemplate = context.FormComponentTemplates.First(t => t.Name.Equals(data.template));

                        var component = new FormDefaultComponent
                        {
                            FormComponentTemplate = formComponentTemplate,
                            Name = data.title,
                            RecordOrder = defaultComponentOrder,
                            Created = now,
                            CreatedBy = FrameworkConstants.DefaultMigrationAccount
                        };

                        if (data.fields != null && data.fields.Keys.Any())
                        {
                            var fieldOrder = 0;
                            foreach (var field in data.fields)
                            {
                                fieldOrder += 10;
                                var formField = new FormDefaultComponentField
                                {
                                    FormDefaultComponent = component,
                                    Name = field.Key,
                                    Attributes = SerializeUtilities.Serialize(field.Value),
                                    RecordOrder = fieldOrder,
                                    Created = now,
                                    CreatedBy = FrameworkConstants.DefaultMigrationAccount
                                };

                                context.FormDefaultComponentFields.AddEntities(formField);
                            }
                        }
                        else
                        {
                            context.FormDefaultComponents.AddEntities(component);
                        }
                    }
                    #endregion

                    context.SaveChanges();
                }
            }
        }

        #endregion
    }
}
