/**
 * @license Copyright (c) 2003-2015, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';

    config.filebrowserBrowseUrl = '/Admin/Media/MediaBrowser?mode=File';
    config.filebrowserImageBrowseUrl = '/Admin/Media/MediaBrowser?mode=Image';
    config.filebrowserWindowWidth = 1200;
    config.filebrowserWindowHeight = 640;
    
    config.enterMode = CKEDITOR.ENTER_BR;
    config.allowedContent = true;
};