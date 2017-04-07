/**
 * Copyright (c) 2014, CKSource - Frederico Knabben. All rights reserved.
 * Licensed under the terms of the MIT License (see LICENSE.md).
 *
 * The EzCMSwidget plugin dialog window definition.
 *
 * Created out of the CKEditor Plugin SDK:
 * http://docs.ckeditor.com/#!/guide/plugin_sdk_sample_1
 */

// Our dialog definition.
CKEDITOR.dialog.add('EzCMSwidgetdialog', function (editor) {
    return {
        title: 'Widget',
        minWidth: 400,
        minHeight: 100,

        // Dialog window content definition.
        contents: [
			{
			    // Definition of the Basic Settings dialog tab (page).
			    id: 'tab-setup',
			    label: 'Settings',

			    // The tab content.
			    elements: [
					{
					    // Text input field for the widgeteviation text.
					    type: 'text',
					    id: 'widget',
					    label: 'Widget',

					    // Validation checking whether the field is not empty.
					    validate: CKEDITOR.dialog.validate.notEmpty("Widget field cannot be empty."),

					    // When setting up this field, set its value to the "align" value from widget data.
					    // Note: Align values used in the widget need to be the same as those defined in the "items" array above.
					    setup: function (widget) {
					        this.setValue(widget.data.widget);
					    },
					    commit: function (widget) {
					        widget.setData('widget', this.getValue());
					    }
					},
				    {
				        type: 'button',
				        id: 'buttonId',
				        label: 'Configure',
				        title: 'Setup widget',
				        style: 'float:right',
				        onLoad: function () {

				        },
				        onClick: function () {
				            var widget = this.getDialog().getContentElement('tab-setup', 'widget').getValue();
				            var href;
				            if (widget == '') {
				                href = "/Admin/Widgets/SelectWidgets?isCkEditor=true";
				            } else {
				                href = "/Admin/Widgets/GenerateWidget?id=" + widget + "&isCkEditor=true";
				            }
				            siteHelper.showPopup({
				                href: href,
				                type: "iframe",
				                width: "1000px",
				                height: "650px"
				            });
				        }
				    }
			    ]
			}
        ]
    };
});
