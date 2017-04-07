var siteHelper = {};
var siteHelper = {};
var timezoneOffSet = timezoneOffSet || 0;
var currentTimezoneCookie = currentTimezoneCookie || "";
var language = language || "";
var dateFormat = dateFormat || "";

(function(context) {

    /*
     * Common helpers
     */

    //Add leaving page asking script
    context.addLeavingPrompt = function() {
        var s = document.createElement("script");
        s.type = "text/javascript";
        s.src = "/Scripts/Shared/askWhenLeaving.js";
        $("head").append(s);
    };

    //Remove asking leaving script
    context.removeLeavingPrompt = function() {
        $("head").find('script').each(function() {
            if (this.src.indexOf("/Scripts/Shared/askWhenLeaving.js") >= 0) {
                $(this).remove();
                removeUnload = true;
            }
        });
    };

    // Check if 2 arrays meet each other at least  item
    context.isArrayMeetOther = function(array1, array2) {
        return $(array1).filter(array2).length > 0;
    };

    //Create guid
    context.guid = function() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    };

    //Get url param value using param name
    context.getUrlParam = function(paramName) {
        var reParam = new RegExp('(?:[\?&]|&amp;)' + paramName + '=([^&]+)', 'i');
        var match = window.location.search.match(reParam);

        return (match && match.length > 1) ? match[1] : '';
    };

    //Add or update url params
    context.addUrlParam = function(url, paramName, paramValue) {
        if (url.indexOf(paramName + "=") >= 0) {
            var prefix = url.substring(0, url.indexOf(paramName));
            var suffix = url.substring(url.indexOf(paramName));
            suffix = suffix.substring(suffix.indexOf("=") + 1);
            suffix = (suffix.indexOf("&") >= 0) ? suffix.substring(suffix.indexOf("&")) : "";
            url = prefix + paramName + "=" + paramValue + suffix;
        } else {
            if (url.indexOf("?") < 0)
                url += "?" + paramName + "=" + paramValue;
            else
                url += "&" + paramName + "=" + paramValue;
        }
        return url;
    };

    // Excute function
    context.executeFunction = function(functionName, functionContext) {
        if (functionContext == null) functionContext = window;
        var args = [].slice.call(arguments).splice(2);
        var namespaces = functionName.split(".");
        var func = namespaces.pop();
        for (var i = 0; i < namespaces.length; i++) {
            functionContext = functionContext[namespaces[i]];
        }
        return functionContext[func].apply(this, args);
    };

    /*
     * String helpers
     */

    // Replace all match string
    context.replaceAll = function(input, stringToReplace, replaceString) {
        var regex = new RegExp(stringToReplace, "gi");
        return input.replace(regex, replaceString);
    };

    //Remove duplicate 
    context.removeDuplicate = function(input) {
        var regex = /(.)\1{1,}/g;
        return input.replace(regex, "$1");
    };

    //Covert string to money format
    context.toMoney = function(money) {
        return '$' + parseFloat(money, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
    };

    //Convert string to id
    context.toIdString = function(path) {
        path = path.replace(new RegExp("/", "gmi"), "");
        path = path.replace(new RegExp(" ", "gmi"), "");
        path = path.replace(new RegExp("-", "gmi"), "");
        path = path.replace(/^.+\.\//, '');

        return context.slug(path);
    };

    //Create hash from string
    context.getHash = function(path) {
        var hash = 0, i, chr, len;
        if (path.length == 0) return hash;
        for (i = 0, len = path.length; i < len; i++) {
            chr = path.charCodeAt(i);
            hash = ((hash << 5) - hash) + chr;
            hash |= 0; // Convert to 32bit integer
        }
        return hash > 0 ? hash : hash * -1;
    };

    //Convert to friendly url
    context.slug = function(input) {
        input = input.replace(/^\s+|\s+$/g, ''); // trim

        // remove accents, swap ñ for n, etc
        var from = "ÃÀÁÄÂẼÈÉËÊÌÍÏÎÕÒÓÖÔÙÚÜÛÑÇ·/,:;ÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴãàáäâẽèéëêìíïîõòóöôùúüûñç·/,:;áàảãạăắằẳẵặâấầẩẫậđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵ";
        var to = "AAAAAEEEEEIIIIOOOOOUUUUNC-----AAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYYaaaaaeeeeeiiiiooooouuuunc-----aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyy";
        for (var i = 0, l = from.length; i < l; i++) {
            input = input.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i));
        }

        input = input.replace(/[^A-Za-z0-9 -]/g, '') // remove invalid chars
            .replace(/\s+/g, '-') // collapse whitespace and replace by -
            .replace(/-+/g, '-'); // collapse dashes
        return input;
    };

    //Convert string to camel friendly
    context.toCamelFriendly = function(input) {
        return input.replace(/([A-Z])/g, ' $1')
            // uppercase the first character
            .replace(/^./, function(str) { return str.toUpperCase(); });
    };

    /*
     * Web helpers
     */

    context.delay = (function() {
        var timer = 0;
        return function(callback, ms) {
            clearTimeout(timer);
            timer = setTimeout(callback, ms);
        };
    })();

    //Confirm
    context.confirm = function(message, callback) {
        bootbox.confirm(message, function(ok) {
            if (callback && $.isFunction(callback)) callback(ok);
        });
    };

    //Prompt
    context.prompt = function(message, callback) {
        bootbox.prompt(message, function(result) {
            if (callback && $.isFunction(callback)) callback(result);
        });
    };

    //Alert
    context.alert = function(message, callback) {
        if (callback != null && $.isFunction(callback)) {
            bootbox.alert(message, callback);
        } else {
            bootbox.alert(message);
        }
    };

    //Submit contact form
    context.submitContact = function(formSelector, callback) {
        var url = "/SiteApi/SaveContact";
        siteHelper.httpPost({
            url: url,
            data: $(formSelector).serialize(),
            success: function(response) {
                if (callback != null) {
                    callback(response);
                } else {
                    context.alert(response.Message);
                }
            }
        });
    };

    //Show fancybox popup
    context.showPopup = function(options, callback) {
        var $options = {
            type: 'iframe',
            autoSize: false,
            width: '100%',
            height: 'auto',
            helpers: {
                overlay: { closeClick: false } // prevents closing when clicking OUTSIDE fancybox 
            },
            keys: {
                close: [null] // disable escape on fancybox
            },
            afterClose: function() {
                if (callback != null) callback();
            }
        };

        $options = $.extend({}, $options, options);

        $.fancybox($options);
    };

    //View the document in fancybox
    context.viewDocument = function(url, mime, width, height) {
        if (width == null) {
            width = "100%";
        }
        if (height == null) {
            height = "100%";
        }

        var embedContent = '<object data="' + url + '" type="' + mime + '" height="99%" width="100%" ><a class="center" onclick="$.fancybox.close()" href="' + url + '" target="_blank" >Click here to download the document.</a></object>';
        console.log(embedContent);
        $.fancybox({
            type: 'html',
            width: width,
            height: height,
            autoSize: false,
            content: embedContent,
            beforeClose: function() {
                $(".fancybox-inner").unwrap();
            },
            helpers: {
                overlay: {
                    opacity: 0.3
                } // overlay
            }
        });
    };

    // Get radio button value
    context.getRadioButtonValue = function(name) {
        $('input:radio[name="' + name + '"]:checked').val();
    };

    // Open url
    context.openUrl = function(url, target) {
        var page = window.open(url, target);
        if (page) {
            //Browser has allowed it to be opened
            page.focus();
        } else {
            //Broswer has blocked it
            alert('Please allow popups for site');
        }
    };

    //Open window popup
    context.openWindowPopup = function(url, width, height) {
        var popup = window.open(url, 'child', 'width=' + width + ', height=' + height + ', left=0, top=0, scrollbars, resizable');
        if (popup) {
            //Browser has allowed it to be opened
            popup.focus();
        } else {
            //Broswer has blocked it
            alert('Please allow popups for site');
        }
    };

    //Get image thumbnail path
    context.imageThumbnail = function(filePath, width, height) {
        if (height == null) {
            return "/Thumbnail?path=" + filePath + "&w=" + width;
        }
        if (width == null) {
            return "/Thumbnail?path=" + filePath + "&h=" + height;
        }
        return "/Thumbnail?path=" + filePath + "&w=" + width + "&h=" + height;
    };

    //Get image thumbnail path
    context.buildImageThumbnail = function(filePath, width, height) {
        var path = context.imageThumbnail(filePath, width, height);

        if (filePath != null && filePath != "") {
            return "<img src='" + path + "' style='width: " + width + "px; height: " + height + "px'/>";
        }

        return "<img src='" + path + "'/>";
    };

    //Show login page
    context.showLogin = function(reloadOnClose) {
        var url = "/Admin/Account/Login";
        context.showPopup({
                href: url,
                type: 'iframe',
                autoSize: false,
                width: 500,
                height: 600
            }, function() {
                if (reloadOnClose != null && reloadOnClose) {
                    window.location.reload();
                }
            });
    };

    //Logout
    context.logout = function(returnUrl) {
        var url = "/Admin/Account/Logout";
        if (returnUrl != null && returnUrl != '')
            url += "?returnUrl=" + returnUrl;
        window.location.href = url;
    };

    //Show edit page
    context.setupPageOrder = function(id) {
        var url = "/Admin/Pages/SetupPageOrder/" + id;
        context.showPopup({
            href: url,
            type: 'iframe',
            autoSize: false,
            width: '60%',
            height: '100%',
            afterClose: function() {
                location.reload();
            }
        });
    };

    //Show create page
    context.selectBodyTemplate = function(id) {
        //mode = 1: Create new page
        //mode = 2: Choose body template when create/edit page
        var url = "/Admin/Pages/SelectBodyTemplate?mode=1&parentId=" + id;
        context.showPopup({
            href: url,
            type: 'iframe',
            autoSize: false,
            width: "1000px",
        });
    };

    //Show create page
    context.createPage = function (parentId, bodyTemplateId) {
        var url = "/Admin/Pages/PopupCreate?parentId=" + parentId + "&bodyTemplateId=" + bodyTemplateId;
        if (bodyTemplateId == null) {
            url = "/Admin/Pages/PopupCreate?parentId=" + parentId;
        }

        context.showPopup({
            href: url,
            type: 'iframe',
            autoSize: false,
            width: '95%',
            height: '100%'
        });
    };

    //Show edit page
    context.editPage = function(id) {
        var url = "/Admin/Pages/PopupEdit/" + id;
        context.showPopup({
            href: url,
            type: 'iframe',
            autoSize: false,
            width: '95%',
            height: '100%'
        });
    };

    //Show delete page
    context.deletePage = function(id, callback) {
        context.showPopup({
            href: "/Admin/Pages/DeleteConfirm?id=" + id + "&callback=" + callback,
            type: 'iframe',
            autoSize: false,
            width: '700px',
            height: '443px'
        });
    };

    /*
     *
     * VALIDATION HELPER
     *
     */

    //Check if email format is valid
    context.isEmail = function(email) {
        var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
        return pattern.test(email);
    };

    // Check if value is date
    context.isDate = function isDate(val) {
        var d = new Date(val);
        return !isNaN(d.valueOf());
    };

    // Check if value is url
    context.isUrl = function(url) {
        if (url.length == 0) {
            return true;
        }

        // if user has not entered http:// https:// or ftp:// assume they mean http://
        if (!/^(https?|ftp):\/\//i.test(url)) {
            url = 'http://' + url; // set both the value
            $(elem).val(url); // also update the form element
        }
        var pattern = new RegExp(/^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i);
        return pattern.test(url);
    };


    /* 
     *
     * 
     * Admin Helpers
     *
     *
     */


    /*
     * Datetime helpers
     */

    // Convert from UTC to local time
    context.getLocalAndUTCTime = function(input) {
        if (input == null || input == '')
            return null;

        //Convert ISO string to UTC
        var isoString = input;
        if (input.indexOf("Z") < 0) {
            isoString = input + "Z";
        }

        var utc = moment(isoString);

        //if cannot parse current datetime - this may be time
        if (utc._d == "Invalid Date") {
            isoString = "1970-01-01T" + input + ".000Z";

            utc = moment(isoString);
        }

        var utcDate = utc.toDate();

        var miliseconds = utcDate.getTime();

        //Get current browser timezone offset
        var browserTimezoneOffset = utcDate.getTimezoneOffset() * 60 * 1000;

        //Convert to local date time
        var localDateTime = new Date(miliseconds + browserTimezoneOffset + timezoneOffSet);
        var utcDateTime = new Date(miliseconds + browserTimezoneOffset);

        var returnObject = {
            local: localDateTime,
            utc: utcDateTime
        };

        return returnObject;
    };

    //Convert float to string base on culture
    context.formatFloat = function (input, format) {
        if (Globalize.length > 0) {
            //Set default format as 5 digits
            if (format == undefined) format = "N5";
            return Globalize.format(input, format);
        }

        return input;
    };

    // Convert date time format from C# format to javascript format
    context.convertMomentFormat = function (format) {
        //Replace date
        format = format.replace(/d/g, 'D');

        //Replace year
        format = format.replace(/y/g, 'Y');

        //Replace AM/PM
        format = format.replace('tt', 'A');

        //Replace hour
        //format = format.replace('HH', 'hh');

        return format;
    };

    /*
     * Web helpers
     */

    //Ajax post
    context.httpPost = function (options) {

        //Default options for post
        var defaultOptions = {
            type: 'POST'
        };

        var ajaxOptions = $.extend(true, defaultOptions, options);

        context.http(ajaxOptions);
    };

    //Ajax get
    context.httpGet = function (options) {

        //Default options for post
        var defaultOptions = {
            type: 'GET'
        };

        var ajaxOptions = $.extend(defaultOptions, options);

        context.http(ajaxOptions);
    };

    //Ajax
    context.http = function (options) {

        //Default options for http ajax
        var defaultOptions = {
            showLoading: true,
            hideLoadingWhenFinish: true,
            showError: true
        };

        var ajaxOptions = $.extend(true, defaultOptions, options);

        //Show ajax loading
        if (ajaxOptions.showLoading) {
            context.showLoading();
        }

        $.ajax(ajaxOptions).error(function (jqXHR, exception) {
            if (ajaxOptions.showError) {
                var message;

                if (jqXHR.status === 0) {
                    message = 'Not connect.\n Please verify network.';
                } else if (jqXHR.status == 404) {
                    message = 'Requested page not found. [404]';
                } else if (jqXHR.status == 500) {
                    message = 'Internal Server Error [500].';
                } else if (exception === 'parsererror') {
                    message = 'Requested JSON parse failed.';
                } else if (exception === 'timeout') {
                    message = 'Time out error.';
                } else if (exception === 'abort') {
                    message = 'Ajax request aborted.';
                } else {
                    message = 'Uncaught Error.\n' + jqXHR.responseText;
                }

                try {
                    context.alert(message);
                } catch (e) {
                    //In case no bootbox defined
                    alert(message);
                }
            }
        }).always(function () {
            //Remove loading when finish
            if (ajaxOptions.showLoading && ajaxOptions.hideLoadingWhenFinish) {
                context.hideLoading();
            }
        });
    };

    //Restart the application
    context.restartApplication = function () {
        context.confirm("Are you sure want to restart the application?", function (ok) {
            if (ok) {
                context.httpPost({
                    hideLoadingWhenFinish: false,
                    url: '/Admin/Home/Restart',
                    data: {},
                    success: function (response) {
                        if (response.Success) {
                            location.reload();
                        } else {
                            siteHelper.showMessage(response);
                        }
                    }
                });
            }
        });
    };

    // Convert select list data to select 2 data
    context.convertSelectListToSelect2 = function (data) {
        var select2Data = [];
        $.each(data, function (index, value) {
            select2Data.push(new {
                text: value.Text,
                id: value.Id,
                slug: value.Text
            });
        });

        return select2Data;
    };

    // Build select 2 dropdown can add new value
    context.buildSelectCanAdd = function (selector, initialData, needConvert) {
        if (needConvert != null && needConvert) {
            initialData = context.convertSelectListToSelect2(initialData);
        }
        $(selector).select2({
            selectOnBlur: true,
            createSearchChoice: function (term, data) {
                if ($(data).filter(function () {
                    return this.text.localeCompare(term) === 0;
                }).length === 0) {
                    return {
                        id: term,
                        text: term
                    };
                }
            },
            data: initialData,
            initSelection: function (element, callback) {
                var currentValue = element.val();
                callback(currentValue);
            }
        });
    };

    // Get hash from url and update the current tag
    context.updateSelectedTab = function () {
        var url = document.location.toString();
        if (url.match('#')) {
            var tabSelector = '.nav-tabs a[href=#' + url.split('#')[1] + ']';
            if ($(tabSelector).length > 0) {
                $(tabSelector).tab('show');
            }
        }
    };

    //Build dropdownlist
    context.buildSelect2 = function (selector, data, defaultValue) {
        if ($(selector).data("select2")) {
            $(selector).select2("destroy");
        }

        $(selector).empty();

        if (defaultValue != null) {
            $(selector).append('<option value="">' + defaultValue + '</option>');
        }
        if (data != null) {
            $.each(data, function (index, item) {
                $(selector).append('<option value="' + item.Value + '"' + (item.Selected ? "selected" : "") + '>' + item.Text + '</option>');
            });
        }

        $(selector).select2();
    };

    // Build select 2 multiple dropdown can add new value
    context.buildMultiSelectCanAdd = function (selector, initialData, needConvert) {
        if (needConvert != null && needConvert) {
            initialData = context.convertSelectListToSelect2(initialData);
        }
        var preselect = function (preselectedIds) {
            var preSelections = [];
            for (index in initialData)
                for (var idIndex in preselectedIds)
                    if (initialData[index].id == preselectedIds[idIndex].id)
                        preSelections.push(initialData[index]);
            return preSelections;
        };

        var extractPreselectedIds = function (element) {
            var preselectedIds = [];
            if (element.val())
                $(element.val().split(",")).each(function () {
                    preselectedIds.push({ id: this });
                });
            //console.log(preselectedIds);
            return preselectedIds;
        };

        $(selector).select2({
            selectOnBlur: true,
            multiple: true,
            createSearchChoice: function (term, data) {
                if ($(data).filter(function () {
                    return this.text.localeCompare(term) === 0;
                }).length === 0) {
                    return {
                        id: term,
                        text: term
                    };
                }
            },
            data: initialData,
            initSelection: function (element, callback) {
                var preselectedIds = extractPreselectedIds(element);
                var preselections = preselect(preselectedIds);
                callback(preselections);
            }
        });
    };

    //Build the html editor
    context.buildHtmlEditor = function (selectorId, mode, height) {
        if (mode == null || mode == "") {
            mode = "text/html";
        }

        var editor = CodeMirror.fromTextArea(document.getElementById(selectorId), {
            mode: mode,
            theme: 'pastel-on-dark',
            lineNumbers: true,
            lineWrapping: true,
            foldGutter: true,
            gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
            matchTags: { bothTags: true },
            extraKeys: {
                "Ctrl-Q": function (cm) { cm.foldCode(cm.getCursor()); },
                "Ctrl-J": "toMatchingTag",
                "Ctrl-Space": "autocomplete",
                "F11": function (cm) {
                    if ($(".CodeMirror-scroll").css("top") == "45px") {
                        $(".CodeMirror-scroll").css("top", "0");
                    }
                    else {
                        $(".CodeMirror-scroll").css("top", "45px");
                    }
                    cm.setOption("fullScreen", !cm.getOption("fullScreen"));
                },
                "Esc": function (cm) {
                    $(".CodeMirror-scroll").css("top", "0");
                    if (cm.getOption("fullScreen")) cm.setOption("fullScreen", false);
                }
            },
            value: document.documentElement.innerHTML
        });
        if (height == null) {
            height = 520;
        }
        editor.setSize("null", height);
        return editor;
    };

    //Build the html display
    context.buildHtmlDisplay = function (selectorId, height) {
        var editor = CodeMirror.fromTextArea(document.getElementById(selectorId), {
            mode: "text/html",
            lineNumbers: true,
            lineWrapping: true,
            readOnly: true,
            value: document.documentElement.innerHTML
        });
        if (height == null) {
            height = 520;
        }
        editor.setSize("null", height);
        return editor;
    };

    //Build date range
    context.buildDateRange = function (dateFromSelector, dateToSelector) {
        $(dateFromSelector).datetimepicker({
            format: siteHelper.convertMomentFormat(dateFormat),
            locale: language,
            showClear: true,
            useCurrent: false,
            keepInvalid: true,
            icons: {
                time: 'fa fa-clock-o',
                date: 'fa fa-calendar',
                up: 'fa fa-chevron-up',
                down: 'fa fa-chevron-down'
            },
            useStrict: false,
            sideBySide: false
        }).on("dp.change", function (e) {
            $(dateToSelector).data("DateTimePicker").minDate(e.date);
        });
        
        $(dateToSelector).datetimepicker({
            format: siteHelper.convertMomentFormat(dateFormat),
            locale: language,
            showClear: true,
            useCurrent: false,
            keepInvalid: true,
            icons: {
                time: 'fa fa-clock-o',
                date: 'fa fa-calendar',
                up: 'fa fa-chevron-up',
                down: 'fa fa-chevron-down'
            },
            useStrict: false,
            sideBySide: false
        }).on("dp.change", function (e) {
            $(dateFromSelector).data("DateTimePicker").maxDate(e.date);
        });


        var dateFrom = $(dateFromSelector).data("DateTimePicker").date();
        var dateTo = $(dateToSelector).data("DateTimePicker").date();
        
        if (dateFrom != null) {
            $(dateToSelector).data("DateTimePicker").minDate(dateFrom);
        }
        
        if (dateTo != null) {
            $(dateFromSelector).data("DateTimePicker").maxDate(dateTo);
        }
        

        //$(dateFromSelector).val($(dateFromSelector).val());
        //$(dateToSelector).val($(dateToSelector).val());
    };

    //Show loading wrapper
    context.showLoading = function () {
        $("#loading-wrapper").show();
    };

    //Hide loading wrapper
    context.hideLoading = function () {
        $("#loading-wrapper").hide();
    };

    //Show message by response data
    context.showMessage = function (response, center) {
        if (response.Success) {
            context.showSuccessMessage(response.Message, center);
        } else {
            switch (response.ResponseStatus) {
                //Warning
                case 2:
                    context.showWarningMessage(response.Message, center);
                    break;
                    //Unauthorize
                case 4:
                    context.showUnauthorizeMessage(response.Message);
                    break;
                    //Error
                default:
                    if (response.DetailMessage != null && response.DetailMessage != '') {
                        context.showDetailErrorMessage(response, center);
                    } else {
                        context.showErrorMessage(response.Message, center);
                    }
                    break;
            }
        }
    };

    //Show unauthorize message
    context.showUnauthorizeMessage = function (message) {
        $.gritter.add({
            sticky: true,
            title: 'Unauthorize',
            text: message,
            class_name: 'gritter-center gritter-error gritter-light'
        });
    };

    //Show success message
    context.showSuccessMessage = function (message, center) {
        var centerClass = "gritter-center ";
        if (center == null || !center)
            centerClass = "";

        $.gritter.add({
            time: 4000,
            title: 'Message',
            text: message,
            class_name: centerClass + 'gritter-info'
        });
    };

    //Build dropdownlist
    context.buildDropdownList = function (selector, data, defaultValue) {
        $(selector).empty();

        if (defaultValue != null) {
            (selector).append('<option value="">' + defaultValue + '</option>');
        }
        $.each(data, function (index, item) {
            $(selector).append('<option value="' + item.Value + '"' + (item.Selected ? "selected" : "") + '>' + item.Text + '</option>');
        });
    };

    //Show warning message
    context.showWarningMessage = function (message, center) {
        // Remove all available messages
        $.gritter.removeAll();

        var centerClass = "gritter-center ";
        if (center == null || !center)
            centerClass = "";

        $.gritter.add({
            time: 4000,
            title: 'Warning',
            text: message,
            class_name: centerClass + 'gritter-warning'
        });
    };

    //Show error message
    context.showErrorMessage = function (message, center) {
        // Remove all available messages
        removeAllErrorMessages();

        if (message === "") {
            message = "Internal Error";
        }

        var centerClass = "gritter-center ";
        if (center == null || !center)
            centerClass = "";

        $.gritter.add({
            time: 4000,
            title: 'Error',
            sticky: true,
            text: message,
            class_name: centerClass + 'gritter-error'
        });
    };

    //Show error message with details
    context.showDetailErrorMessage = function (response, center) {
        // Remove all available messages
        removeAllErrorMessages();

        var centerClass = "gritter-center ";
        if (center == null || center)
            centerClass = "";
        var html = "<div class='error-message'>" + response.Message + "</div>" +
            "<div class='error-detail-box'>" +
            "<div class='more-details'>" +
            "<a class='toggle-error-btn' href='javascript:void(0)'>" +
            "<i class='fa fa-chevron-down'></i> More Details</a>" +
            "</div>" +
            "<div class='detail-message' style='display: none'>" +
            response.DetailMessage +
            "</div>" +
            "</div>";
        $.gritter.add({
            title: 'Error',
            sticky: true,
            text: html,
            class_name: centerClass + 'gritter-error'
        });
    };

    // Disable click event in review mode
    context.disableClickEvent = function (selector) {
        $(document).on('click', selector, function (e) {
            e.preventDefault();
        });
    };

    //Get element visible width
    context.getVisibleWidth = function ($selector) {
        var elementWidth = $selector.outerWidth(),
            width = $(window).width() - 10,
            rect = $selector[0].getBoundingClientRect(), left = rect.left, right = rect.right;
        var a = Math.max(0, left >= 0 ? Math.min(elementWidth, width - left) : (right < width ? right : width));
        return a;
    };

    //Get element visible height
    context.getVisibleHeight = function ($selector) {
        var elementHeight = $selector.outerHeight(),
            height = $(window).height() - 10,
            rect = $selector[0].getBoundingClientRect(), top = rect.top, bottom = rect.bottom;
        return Math.max(0, top > 0 ? Math.min(elementHeight, height - top) : (bottom < height ? bottom : height));
    };
    
    // Get local storage value
    context.getLocalStorage = function (key) {
        if (!supportStorage()) {
            return null;
        }

        var value = localStorage.getItem(key);
        if (context.isJsonObject(value)) {
            value = JSON.parse(value);
        }

        return value;
    };
    
    // Set local storage value
    context.setLocalStorage = function (key, value) {
        if (supportStorage()) {
            localStorage.setItem(key, JSON.stringify(value));
        }
    };

    // Check if string is json or not
    context.isJsonObject = function (jsonString) {
        try {
            var object = JSON.parse(jsonString);
            if (object && typeof object === "object" && object !== null) {
                return true;
            }
        } catch(e) {  } 

        return false;
    };

})(siteHelper);

// Check browser support storage or not
function supportStorage() {
    return (typeof(Storage) !== "undefined");
}

// Remove all gritter error messages
function removeAllErrorMessages() {
    $.each($("#gritter-notice-wrapper div"), function (index, selector) {
        if ($(selector).hasClass("gritter-error")) {
            $(selector).fadeOut(500, function () { $(this).remove(); });
        }
    });
}

if (!('contains' in String.prototype)) {
    String.prototype.contains = function (str, startIndex) {
        return -1 !== String.prototype.indexOf.call(this, str, startIndex);
    };
}

String.prototype.format = function () {
    var str = this;
    for (var i = 0; i < arguments.length; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        str = str.replace(reg, arguments[i]);
    }
    return str;
}