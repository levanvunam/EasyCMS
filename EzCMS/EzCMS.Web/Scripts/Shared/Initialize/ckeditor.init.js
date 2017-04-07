CKEDITOR.dtd.$removeEmpty['span'] = false;
CKEDITOR.dtd.$removeEmpty['i'] = false;

function buildCkEditor(selectorId, options) {
    var defaultOptions = {
        extraPlugins: 'widget,ezcms_stylesheetparser,ezcms_widget,fontawesome,lineutils,html5validation,youtube',
    };
    $.extend(options, defaultOptions);

    var editor = CKEDITOR.replace(selectorId, options);

    $('[type=submit]').bind('click', function () {
        editor.updateElement();
    });

    return editor;
};