/**
 <b>Treeview</b>. A wrapper for FuelUX treeview element.
 It's just a wrapper so you still need to include FuelUX treeview script first.
*/
(function ($, undefined) {

    $.fn.EzCMSTree = $.fn.EzCMS_tree = function (options) {
        var $options = {
            'open-icon': 'ace-icon fa fa-folder-open',
            'close-icon': 'ace-icon fa fa-folder',
            'selectable': true,
            'selected-icon': 'ace-icon fa fa-check',
            'unselected-icon': 'ace-icon fa fa-times',
            'loadingHTML': 'Loading...'
        };

        $options = $.extend({}, $options, options);
        this.each(function () {
            var $this = $(this);
            $this.addClass('tree').attr('role', 'tree');
            $this.html(
                '<li class="tree-branch hide" data-template="treebranch" role="treeitem" aria-expanded="false">\
				<div class="tree-branch-header">\
					<span class="tree-branch-name">\
						<i class="icon-folder ' + $options['close-icon'] + '"></i>\
						<span class="tree-label"></span>\
					</span>\
				</div>\
				<ul class="tree-branch-children" role="group"></ul>\
				<div class="tree-loader" role="alert">' + $options['loadingHTML'] + '</div>\
			</div>\
			<li class="tree-item hide" data-template="treeitem" role="treeitem">\
				<span class="tree-item-name">\
				  <i class="icon-item"></i>\
				  <span class="tree-label"></span>\
				</span>\
			</li>');

            $this.addClass($options['selectable'] == true ? 'tree-selectable' : 'tree-unselectable');

            $this.tree($options);

            $this.on('loaded.fu.tree', function (e) {
                $('[data-rel=tooltip]').tooltip({ container: 'body' });
            });
        });

        return this;
    };

})(window.jQuery);


function replaceDocumentData(template, data) {
    $.each(data, function (name, value) {
        template = replaceAll(template, "{" + name + "}", value);
    });

    if (data.IsRead) {
        template = template.replace("{IsReadClass}", "visible");
        template = template.replace("{IsNotReadClass}", "hidden");
    } else {
        template = template.replace("{IsReadClass}", "hidden");
        template = template.replace("{IsNotReadClass}", "visible");
    }

    return template;
}

function replaceAll(input, stringToReplace, replaceString) {
    var regex = new RegExp(stringToReplace, "igm");
    return input.replace(regex, replaceString);
};