var updateUrl = updateUrl || "";
var dateFormat = dateFormat || "";
var timeFormat = timeFormat || "";
var dateTimeFormat = dateTimeFormat || "";
var pk = pk || 0;
var emptyText = emptyText || "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

function editableValidate(element, value) {
    var validates = $(element).data("validation") != undefined ? $(element).data("validation").split(" ") : [];
    var message = $(element).data("message") != undefined ? $(element).data("message") : "";
    var returnMessage = "";
    $.each(validates, function (i, validate) {
        switch (validate) {
            case 'required':
                if ($.trim(value) == '') {
                    returnMessage = (message == "" ? "Field is required" : message);
                    return;
                }
                break;
            case 'email':
                if (!siteHelper.isEmail(value)) {
                    returnMessage = (message == "" ? "Email is not valid" : message);
                    return;
                }
                break;
            case 'url':
                if (!siteHelper.isUrl(value)) {
                    returnMessage = (message == "" ? "Url is not valid" : message);
                    return;
                }
                break;
            default:
        }
    });
    return returnMessage;
}

$(function () {
    $.fn.editable.defaults.mode = 'popup';
    $.fn.editableform.loading = "<div class='editableform-loading'><i class='light-blue fa fa-2x fa-spinner fa-spin'></i></div>";
    $.fn.editableform.buttons = '<button type="submit" class="btn btn-info editable-submit"><i class="fa fa-check fa-white"></i></button>' +
        '<button type="button" class="btn editable-cancel"><i class="fa fa-times s"></i></button>';

    $('[language-edit=true]').editable({
        type: 'text',
        pk: '0',
        url: "/Admin/LocalizedResources/UpdateLocalizedResource",
        validate: function (v) {
            var result = editableValidate(this, v);
            if (result != '') {
                return result;
            }
        },
        ajaxOptions: {
            type: 'post',
            dataType: 'json'
        },
        success: function (response) {
            siteHelper.showMessage(response);
            if (!response.Success)
                return response.Message;
        }
    }).on('shown', function () {
        var $innerForm = $(this).data('editable').input.$input.closest('form');
        var $outerForm = $innerForm.parents('form').eq(0);
        $innerForm.data('validator', $outerForm.data('validator'));
    });

    //editables
    $(".editable-property").editable({
        emptytext: emptyText,
        pk: pk,
        url: updateUrl,
        ajaxOptions: {
            type: 'post',
            dataType: 'json'
        },
        success: function (response) {
            siteHelper.showMessage(response);
            if (!response.Success)
                return response.Message;
        }
    }).on('shown', function () {
        var $innerForm = $(this).data('editable').input.$input.closest('form');
        var $outerForm = $innerForm.parents('form').eq(0);
        $innerForm.data('validator', $outerForm.data('validator'));
    });

    $('.editable-text').editable({
        emptytext: emptyText,
        type: 'text',
        pk: pk,
        url: updateUrl,
        validate: function (v) {
            var result = editableValidate(this, v);
            if (result != '') {
                return result;
            }
        },
        ajaxOptions: {
            type: 'post',
            dataType: 'json'
        },
        success: function (response) {
            siteHelper.showMessage(response);
            if (!response.Success)
                return response.Message;
        }
    });

    $('.editable-textarea').editable({
        emptytext: emptyText,
        type: 'textarea',
        pk: pk,
        url: updateUrl,
        validate: function (v) {
            var result = editableValidate(this, v);
            if (result != '') {
                return result;
            }
        },
        ajaxOptions: {
            type: 'post',
            dataType: 'json'
        },
        success: function (response) {
            siteHelper.showMessage(response);
            if (!response.Success)
                return response.Message;
        }
    });

    $('.editable-date').editable({
        emptytext: emptyText,
        type: 'combodate',
        format: siteHelper.convertMomentFormat(dateFormat),
        viewformat: siteHelper.convertMomentFormat(dateFormat),
        template: siteHelper.convertMomentFormat(dateFormat),
        datepicker: {
            weekStart: 1
        },
        pk: pk,
        url: updateUrl,
        validate: function (v) {
            var result = editableValidate(this, v);
            if (result != '') {
                return result;
            }
        },
        ajaxOptions: {
            type: 'post',
            dataType: 'json'
        },
        success: function (response) {
            siteHelper.showMessage(response);
            if (!response.Success)
                return response.Message;
        }
    });

    $('.editable-datetime').editable({
        emptytext: emptyText,
        type: 'combodate',
        format: siteHelper.convertMomentFormat(dateTimeFormat),
        viewformat: siteHelper.convertMomentFormat(dateTimeFormat),
        template: siteHelper.convertMomentFormat(dateTimeFormat),
        datepicker: {
            weekStart: 1
        },
        pk: pk,
        url: updateUrl,
        validate: function (v) {
            var result = editableValidate(this, v);
            if (result != '') {
                return result;
            }
        },
        ajaxOptions: {
            type: 'post',
            dataType: 'json'
        },
        success: function (response) {
            siteHelper.showMessage(response);
            if (!response.Success)
                return response.Message;
        }
    });

    $('.editable-boolean').editable({
        emptytext: emptyText,
        type: 'select2',
        pk: pk,
        url: updateUrl,
        validate: function (v) {
            var result = editableValidate(this, v);
            if (result != '') {
                return result;
            }
        },
        source: [
              { id: 'True', text: 'Yes' },
              { id: 'False', text: 'No' }
        ],
        select2: {
            multiple: false,
            minimumResultsForSearch: Infinity
        }
    });

    $.each($('.editable-select'), function (index, value) {
        var editableSelector = this;

        //Get parameters from attributes
        var source = $(editableSelector).data("editable-source");
        var isAjaxSource = $(editableSelector).data("editable-ajax");

        if (isAjaxSource) {
            $(editableSelector).editable({
                source: source,
                emptytext: emptyText,
                type: 'select2',
                pk: pk,
                url: updateUrl,
                validate: function (v) {
                    var result = editableValidate(this, v);
                    if (result != '') {
                        return result;
                    }
                },
                select2: {
                    allowClear: true,
                    minimumInputLength: 3,
                    id: function (item) {
                        return item.id;
                    },
                    ajax: {
                        type: 'POST',
                        quietMillis: 50,
                        dataType: 'json',
                        data: function (term, page) {
                            return { keyword: term };
                        },
                        results: function (response, page) {
                            return {
                                results: $.map(response, function (item) {
                                    return {
                                        id: item.Value,
                                        text: item.Text
                                    };
                                })
                            };
                        }
                    },
                    initSelection: function (element, callback) {
                        return siteHelper.httpPost({
                            showLoading: false,
                            url: source,
                            data: { keyword: element.val() },
                            success: function (response) {
                                callback({ id: response[0].Value, text: response[0].Text });
                            }
                        });
                    }
                }
            });
        } else {
            siteHelper.httpPost({
                showLoading: false,
                url: source,
                data: {},
                success: function (response) {
                    if (response.Success) {
                        var dataSource = [];
                        $.each(response.Data, function (i, v) {
                            dataSource.push({
                                id: v.Value, text: v.Text
                            });
                        });

                        $(editableSelector).editable({
                            emptytext: emptyText,
                            type: 'select2',
                            pk: pk,
                            url: updateUrl,
                            source: dataSource,
                            validate: function (v) {
                                var result = editableValidate(this, v);
                                if (result != '') {
                                    return result;
                                }
                            },
                            select2: {
                                width: 200,
                            }
                        });
                    }
                }
            });
        }
    });
});