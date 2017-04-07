/**
 * Copyright (c) 2014, CKSource - Frederico Knabben. All rights reserved.
 * Licensed under the terms of the MIT License (see LICENSE.md).
 *
 * Basic sample plugin inserting ezcms_widgeteviation elements into the CKEditor editing area.
 *
 * Created out of the CKEditor Plugin SDK:
 * http://docs.ckeditor.com/#!/guide/plugin_sdk_sample_1
 */

// Register the plugin within the editor.
CKEDITOR.plugins.add( 'ezcms_widget', {

	// Register the icons.
	icons: 'ezcms_widget',

	// The plugin initialization logic goes inside this method.
	init: function( editor ) {

		// Define an editor command that opens our dialog window.
		editor.addCommand( 'ezcms_widget', new CKEDITOR.dialogCommand( 'ezcms_widget_dialog' ) );

		// Create a toolbar button that executes the above command.
		editor.ui.addButton( 'ezcms_widget', {

			// The text part of the button (if available) and the tooltip.
		    label: 'Widget',

			// The command to execute on click.
			command: 'ezcms_widget',

			// The button placement in the toolbar (toolbar group name).
			toolbar: 'insert'
		});
	    
		editor.on('doubleclick', function (evt) {
		    var element = evt.data.element;

		    if (element.is('widget')) {
		        editor.execCommand('ezcms_widget');
		        //evt.data.dialog = 'ezcms_widget_dialog';
		    }
		});


		if ( editor.contextMenu ) {
			
			// Add a context menu group with the Edit ezcms_widgeteviation item.
			editor.addMenuGroup( 'ezcms_widgetGroup' );
			//editor.addMenuItem( 'ezcms_widgetItem', {
			//	label: 'Edit widget',
			//	icon: this.path + 'icons/ezcms_widget.png',
			//	command: 'ezcms_widget',
			//	group: 'ezcms_widgetGroup'
			//});

			//editor.contextMenu.addListener( function( element ) {
			//	if ( element.getAscendant( 'widget', true ) ) {
			//		return { ezcms_widgetItem: CKEDITOR.TRISTATE_OFF };
			//	}
			//});
		}

		// Register our dialog file -- this.path is the plugin folder path.
		CKEDITOR.dialog.add( 'ezcms_widget_dialog', this.path + 'dialogs/ezcms_widget.js' );
	}
});
