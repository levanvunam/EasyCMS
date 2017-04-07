function loadScript(url, callback, checkLoadedCallback) {
    if (checkLoadedCallback != null && checkLoadedCallback()) {
        if (callback != null) {
            callback();
        }
    } else {
        var script = document.createElement("script");
        script.type = "text/javascript";

        if (script.readyState) { //IE
            script.onreadystatechange = function () {
                if (script.readyState == "loaded" || script.readyState == "complete") {
                    script.onreadystatechange = null;
                    if (callback != null) {
                        callback();
                    }
                }
            };
        } else { //Others
            script.onload = function () {
                if (callback != null) {
                    callback();
                }
            };
        }
        script.src = url;
        document.getElementsByTagName("head")[0].appendChild(script);
    }
}

function loadStyles(filename, checkLoadedCallback) {
    if (checkLoadedCallback != null && checkLoadedCallback()) {
        return;
    }

    var fileref = document.createElement("link");
    fileref.setAttribute("rel", "stylesheet");
    fileref.setAttribute("type", "text/css");
    fileref.setAttribute("href", filename);
    document.getElementsByTagName("head")[0].appendChild(fileref);
}

function getInternetExplorerVersion() {
    var rv = -1; // Return value assumes failure.
    if (navigator.appName == 'Microsoft Internet Explorer') {
        var ua = navigator.userAgent;
        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
    }
    return rv;
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

function removeParameter(key, url) {
    var rtn = url.split("?")[0],
        param,
        paramsArr = [],
        queryString = (url.indexOf("?") !== -1) ? url.split("?")[1] : "";
    if (queryString !== "") {
        paramsArr = queryString.split("&");
        for (var i = paramsArr.length - 1; i >= 0; i -= 1) {
            param = paramsArr[i].split("=")[0];
            if (param === key) {
                paramsArr.splice(i, 1);
            }
        }
        rtn = rtn + "?" + paramsArr.join("&");
    }
    return rtn;
}

/*
Check if jquery or plugin is loaded or not
*/
checkJqueryLoaded = function () { if (window.jQuery) { return true; } else { return false; } };
checkSelect2Loaded = function () { if (window.jQuery().select2) { return true; } else { return false; } };
checkValidationLoaded = function () { if (window.jQuery().validate) { return true; } else { return false; } };
checkBootstrapLoaded = function () { if (window.jQuery().modal) { return true; } else { return false; } };

if (getParameterByName("formSubmitted") == 'true') {
    document.write("@Raw(Model.ThankyouMessage)");

    var url = removeParameter("formSubmitted", window.location.href);
    window.history.pushState({}, "", url);
} else {
    var meta = document.createElement("meta");
    meta.name = 'viewport';
    meta.content = 'width=device-width, initial-scale=1.0';
    document.getElementsByTagName('head')[0].appendChild(meta);

    loadStyles("@(Model.CurrentSiteUrl)/Scripts/FormBuilder/css/bootstrap.min.css");
    loadStyles("@(Model.CurrentSiteUrl)/Scripts/FormBuilder/css/Select2/select2.min.css");
    loadStyles("@(Model.CurrentSiteUrl)/Scripts/FormBuilder/css/form.css");
    var additionalStyle = "@Model.Style";
    if (additionalStyle != '') {
        loadStyles(additionalStyle);
    }

    var jqueryPath = "@(Model.CurrentSiteUrl)/Scripts/FormBuilder/jquery.min.js";
    if (getInternetExplorerVersion() > -1) {
        jqueryPath = "@(Model.CurrentSiteUrl)/Scripts/FormBuilder/jquery.ie.js";
    }

    loadScript(jqueryPath, function () {
        var ver = getInternetExplorerVersion();
        if (ver > -1 && ver <= 10) {
            loadScript("@(Model.CurrentSiteUrl)/Scripts/FormBuilder/html5.js");
            loadScript("@(Model.CurrentSiteUrl)/Scripts/FormBuilder/respond.min.js");
        }

        loadScript("@(Model.CurrentSiteUrl)/Scripts/FormBuilder/bootstrap.min.js", null, checkBootstrapLoaded);
        loadScript("@(Model.CurrentSiteUrl)/Scripts/FormBuilder/select2.min.js", function () {
            $("[multiple=multiple]").select2();
            $("select").select2();
        }, checkSelect2Loaded);
        loadScript("@(Model.CurrentSiteUrl)/Scripts/FormBuilder/jquery.validate.min.js", function () {
            $(function () {
                var form = "#EzCMSForm";
                var formSpinner = "#EzCMSForm #FormSpinner";
                var formError = "#EzCMSForm #EzCMSFormError";
                var spinnerUrl = "@(Model.CurrentSiteUrl)/Scripts/FormBuilder/images/spinner.gif";


                $(form).validate({
                    errorPlacement: function (error, element) {
                        if (element.attr('type') === 'radio' || element.attr('type') === 'checkbox') {
                            error.insertAfter(element.parents('.controls').children(':last'));
                        }
                        else {
                            error.insertAfter(element);
                        }
                    }
                });
                $("#EzCMSForm [data-validate]").each(function (i, element) {
                    $.each($(this).attr("data-validate").split(","), function (j, rule) {
                        if (rule != '') {
                            $(element).rules("add", rule.trim());
                        }
                    });
                });

                $(form).submit(function (e) {
                    if ($(form).valid()) {
                        var button = $(form).find("button[type=submit]");

                        $(formError).hide();
                        $(formSpinner).remove();
                        $(button).hide();
                        $(button).after('<span id="FormSpinner"><img src="' + spinnerUrl + '" alt="Loading"/> Processing...</span>');

                        if ("@Model.AllowAjaxSubmit" == "True") {
                            e.preventDefault();
                            $.ajax({
                                type: "POST",
                                url: "@(Raw(Model.PostUrl))",
                                crossDomain: true,
                                datatype: "jsonp",
                                data: $(form).serialize(),
                                contentType: "application/x-www-form-urlencoded",
                                success: function (response) {
                                    $(button).show();
                                    $(formSpinner).remove();
                                    if (response.Success) {
                                        $(form).html(response.Message);
                                        document.write = function (str) {
                                            $(form).append(str);
                                        };
                                    } else {
                                        $(formError).show().html('<div class="alert alert-error"><a class="close" data-dismiss="alert">&times;</a>' + response.Message + '</div>');
                                    }
                                }
                            });
                        }
                    }
                    else {
                        e.preventDefault();
                    }
                });
            });
        }, checkValidationLoaded);
    }, checkJqueryLoaded);
    document.write('<form id="EzCMSForm" class="form-horizontal" action="@Raw(Model.PostUrl)" method="post" role="form"><div id="EzCMSFormError"></div><input type="hidden" name="ReferredUrl" value="' + window.location.href + '"/>@(Raw(Model.Content))</form>');
}