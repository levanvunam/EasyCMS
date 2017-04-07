/**
 * Copyright (c) 2014, CKSource - Frederico Knabben. All rights reserved.
 * Licensed under the terms of the MIT License (see LICENSE.md).
 *
 * Basic sample plugin inserting EzCMSwidgeteviation elements into the CKEditor editing area.
 *
 * Created out of the CKEditor Plugin SDK:
 * http://docs.ckeditor.com/#!/guide/plugin_sdk_sample_1
 */

// Register the plugin within the editor.
CKEDITOR.plugins.add( 'EzCMSwidget', {

	// Register the icons.
	icons: 'EzCMSwidget',

	// The plugin initialization logic goes inside this method.
	init: function( editor ) {

		// Define an editor command that opens our dialog window.
		editor.addCommand( 'EzCMSwidget', new CKEDITOR.dialogCommand( 'EzCMSwidgetDialog' ) );

		// Create a toolbar button that executes the above command.
		editor.ui.addButton( 'EzCMSwidget', {

			// The text part of the button (if available) and the tooltip.
		    label: 'Widget',

			// The command to execute on click.
			command: 'EzCMSwidget',

			// The button placement in the toolbar (toolbar group name).
			toolbar: 'insert'
		});
	    
		editor.on('doubleclick', function (evt) {
		    var element = evt.data.element;

		    if (element.is('widget')) {
		        editor.execCommand('EzCMSwidget');
		        //evt.data.dialog = 'EzCMSwidgetDialog';
		    }
		});


		if ( editor.contextNavigation ) {
			
			// Add a context menu group with the Edit EzCMSwidgeteviation item.
			editor.addNavigationGroup( 'EzCMSwidgetGroup' );
			//editor.addNavigationItem( 'EzCMSwidgetItem', {
			//	label: 'Edit widget',
			//	icon: this.path + 'icons/EzCMSwidget.png',
			//	command: 'EzCMSwidget',
			//	group: 'EzCMSwidgetGroup'
			//});

			//editor.contextNavigation.addListener( function( element ) {
			//	if ( element.getAscendant( 'widget', true ) ) {
			//		return { EzCMSwidgetItem: CKEDITOR.TRISTATE_OFF };
			//	}
			//});
		}

		// Register our dialog file -- this.path is the plugin folder path.
		CKEDITOR.dialog.add( 'EzCMSwidgetDialog', this.path + 'dialogs/EzCMSwidget.js' );
	}
});
