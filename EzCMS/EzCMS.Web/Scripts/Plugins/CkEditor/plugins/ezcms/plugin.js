/**
 * Copyright (c) 2014, CKSource - Frederico Knabben. All rights reserved.
 * Licensed under the terms of the MIT License (see LICENSE.md).
 *
 * Basic sample plugin inserting EzCMS widget element into the CKEditor editing area.
 *
 */

// Register the plugin within the editor.
CKEDITOR.plugins.add('EzCMSwidget', {

    // This plugin requires the Widgets System defined in the 'widget' plugin.
    requires: 'widget',

    // Register the icons.
    icons: 'EzCMSwidget',

    // The plugin initialization logic goes inside this method.
    init: function (editor) {

        // Register the editing dialog.
        CKEDITOR.dialog.add('EzCMSwidgetdialog', this.path + 'dialogs/EzCMSwidget.js');

        // Register the EzCMSwidget widget.
        editor.widgets.add('EzCMSwidget', {
            // Allow all HTML elements, classes, and styles that this widget requires.
            // Read more about the Advanced Content Filter here:
            // * http://docs.ckeditor.com/#!/guide/dev_advanced_content_filter
            // * http://docs.ckeditor.com/#!/guide/plugin_sdk_integration_with_acf
            allowedContent: 'widget',

            // Minimum HTML which is required by this widget to work.
            requiredContent: 'widget',

            // Define two nested editable areas.
            editables: {
                title: {
                    // Define CSS selector used for finding the element inside widget element.
                    selector: 'widget',
                    // Define content allowed in this nested editable. Its content will be
                    // filtered accordingly and the toolbar will be adjusted when this editable
                    // is focused.
                    allowedContent: ''
                }
            },

            // Define the template of a new Simple Box widget.
            // The template will be used when creating new instances of the Simple Box widget.
            template: '<widget></widget>',

            // Define the label for a widget toolbar button which will be automatically
            // created by the Widgets System. This button will insert a new widget instance
            // created from the template defined above, or will edit selected widget
            // (see second part of this tutorial to learn about editing widgets).
            //
            // Note: In order to be able to translate your widget you should use the
            // editor.lang.EzCMSwidget.* property. A string was used directly here to simplify this tutorial.
            button: 'Widget',

            // Set the widget dialog window name. This enables the automatic widget-dialog binding.
            // This dialog window will be opened when creating a new widget or editing an existing one.
            dialog: 'EzCMSwidgetdialog',

            // Check the elements that need to be converted to widgets.
            //
            // Note: The "element" argument is an instance of http://docs.ckeditor.com/#!/api/CKEDITOR.htmlParser.element
            // so it is not a real DOM element yet. This is caused by the fact that upcasting is performed
            // during data processing which is done on DOM represented by JavaScript objects.
            upcast: function (element) {
                // Return "true" (that element needs to converted to a Simple Box widget)
                // for all <div> elements with a "EzCMSwidget" class.
                return element.name == 'widget';
            },

            // When a widget is being initialized, we need to read the data ("align" and "width")
            // from DOM and set it by using the widget.setData() method.
            // More code which needs to be executed when DOM is available may go here.
            init: function () {
                var widget = this.element.getText();
                this.setData('widget', widget);
            },

            // Listen on the widget#data event which is fired every time the widget data changes
            // and updates the widget's view.
            // Data may be changed by using the widget.setData() method, which we use in the
            // Simple Box dialog window.
            data: function () {
                // Check whether "width" widget data is set and remove or set "width" CSS style.
                // The style is set on widget main element (div.EzCMSwidget).
                if (this.data.widget == '')
                    this.element.setText('');
                else
                    this.element.setText(this.data.widget);
            }
        });

        // Create a toolbar button that executes the above command.
        editor.ui.addButton('EzCMSwidget', {
            label: 'Widget',
            toolbar: 'insert',
            command: 'EzCMSwidget',
            icon: this.path + 'icons/EzCMSwidget.png'
        });
    }
});