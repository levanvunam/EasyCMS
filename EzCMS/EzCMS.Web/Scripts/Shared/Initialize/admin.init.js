// Init client culture
var currentCulture = currentCulture || "";

var language = language || "";
var dateFormat = dateFormat || "";
var timeFormat = timeFormat || "";
var dateTimeFormat = dateTimeFormat || "";

// Textbox format
var defaultAddress = defaultAddress || "";
var phoneFormat = phoneFormat || "";

//Keep alive code
var keepAliveInterval = keepAliveInterval || 60;

//Timezone cookie
var enableCookie = false;
var currentTimezoneCookieKey = currentTimezoneCookieKey || "";
var currentTimezoneCookie = currentTimezoneCookie || "";

//Media browser
var maxSizeUploaded = maxSizeUploaded || 10485760;

//Check if cookie is enabled
function checkCookieEnabled() {
    var testCookie = 'test cookie';
    $.cookie(testCookie, true);
    
    // browser supports cookie
    if ($.cookie(testCookie)) {

        // delete the test cookie
        $.cookie(testCookie, null);

        return true;
    }

    return false;
}

$.cookie.raw = true;
function setTimezoneCookie() {

    //Set expired date to next year
    var expiredDate = new Date();
    expiredDate.setTime(expiredDate.getTime() + (365 * 24 * 60 * 60 * 1000));

    // if the timezone cookie not exists create one.
    if (!$.cookie(currentTimezoneCookieKey)) {
        // browser supports cookie
        if (enableCookie) {

            // create a new cookie
            $.cookie(currentTimezoneCookieKey, currentTimezoneCookie, {
                expires: expiredDate,
                path: '/'
            });

            // re-load the page to make cookie effected
            location.reload();
        }
    }
    else {
        //user may have changed the timezone
        if ($.cookie(currentTimezoneCookieKey) !== currentTimezoneCookie) {

            $.cookie(currentTimezoneCookieKey, currentTimezoneCookie, {
                expires: expiredDate,
                path: '/'
            });

            location.reload();
        }
    }
}

$(function () {

    // Register some admin plugins
    registerPlugins();

    // Register / modify some ace plugins
    registerBasePlugins();

    // Register admin handlers
    registerAdminHandlers();

    //Register simple upload
    registerSimpleUpload();
    
    //Show validation error
    $(".tab-content").find("div.tab-pane:hidden:has(span.field-validation-error)").each(function (index, tab) {
        var id = $(tab).attr("id");
        $('a[href="#' + id + '"]').tab('show');

        //Break the loop
        return false;
    });
});

/* 
 * Register and override some admin theme plugins
 */
function registerBasePlugins() {

    // Extend jQuery UI dialog to have html title
    $.widget("ui.dialog", $.extend({}, $.ui.dialog.prototype, {
        _title: function (title) {
            if (!this.options.title) {
                title.html("&#160;");
            } else {
                title.html(this.options.title);
            }
        }
    }));


    /*
     * Override ACE menu
     */

    var $minimized = $('#sidebar').hasClass('menu-min');
    $('#sidebar-collapse').on(ace.click_event, function () {
        $minimized = $('#sidebar').hasClass('menu-min');
        ace.settings.sidebar_collapsed(!$minimized); //@ ace-extra.js
    });

    //opening submenu
    $('.nav-list').on("click", ".arrow", function (e) {
        //check to see if we have clicked on an element which is inside a .dropdown-toggle element?!
        //if so, it means we should toggle a submenu
        var linkElement = $(e.target).closest('a');
        if (!linkElement || linkElement.length == 0) return true; //if not clicked inside a link element

        $minimized = $('#sidebar').hasClass('menu-min');

        if (!linkElement.hasClass('dropdown-toggle')) { //it doesn't have a submenu return
            //just one thing before we return
            //if sidebar is collapsed(minimized) and we click on a first level menu item
            //and the click is on the icon, not on the menu text then let's cancel event and cancel navigation
            //Good for touch devices, that when the icon is tapped to see the menu text, navigation is cancelled
            //navigation is only done when menu text is tapped
            if ($minimized && ace.click_event == "tap" &&
                linkElement.get(0).parentNode.parentNode == this /*.nav-list*/)//i.e. only level-1 links
            {
                var text = linkElement.find('.menu-text').get(0);
                if (e.target != text && !$.contains(text, e.target))//not clicking on the text or its children
                    return false;
            }

            return;
        }
        //
        var sub = linkElement.next().get(0);

        //if we are opening this submenu, close all other submenus except the ".active" one
        if (!$(sub).is(':visible')) { //if not open and visible, let's open it and make it visible
            var parentUl = $(sub.parentNode).closest('ul');
            if ($minimized && parentUl.hasClass('nav-list')) return;

            parentUl.find('> .open > .submenu').each(function () {
                //close all other open submenus except for the active one
                if (this != sub && !$(this.parentNode).hasClass('active')) {
                    $(this).slideUp(200).parent().removeClass('open');

                    //uncomment the following line to close all submenus on deeper levels when closing a submenu
                    //$(this).find('.open > .submenu').slideUp(0).parent().removeClass('open');
                }
            });
        } else {
            //uncomment the following line to close all submenus on deeper levels when closing a submenu
            //$(sub).find('.open > .submenu').slideUp(0).parent().removeClass('open');
        }

        if ($minimized && $(sub.parentNode.parentNode).hasClass('nav-list')) return false;

        $(sub).slideToggle(200).parent().toggleClass('open');
        return false;
    });

    // Not doing anything when click on add favourite icon
    $('.nav-list').on("click", ".add-to-favourite", function (e) {
        return false;
    });

    $('.nav-list').on("click", ".remove-from-favourite", function (e) {
        return false;
    });

    $('.nav-list').on("click", ".dropdown-toggle", function (e) {
        var linkElement = $(e.target).closest('a');

        //if not clicked inside a link element
        if (!linkElement || linkElement.length == 0) {
            return true;
        } else {
            var href = linkElement.attr("href");
            if (href != "#")
                location.href = linkElement.attr("href");
        }
    });
}

//Media browser
var $TargetControl;
$(".browsefile").click(function () {
    $TargetControl = $(this).parent().find("input").first();
    var rootFolder = $TargetControl.data("root");
    if (rootFolder == undefined) {
        rootFolder = "";
    }
    var mode = $TargetControl.data("mode");
    if (mode == undefined) {
        mode = "";
    }
    var imageUrl = $TargetControl.val();
    siteHelper.showPopup({
        href: '/Admin/Media/MediaBrowser?rootFolder=' + rootFolder + "&imageUrl=" + imageUrl + "&mode=" + mode,
        type: 'iframe',
        width: "100%",
        height: "auto"
    });
});

// Register simple upload
function registerSimpleUpload() {
    $.each($(".simple-upload"), function (index, value) {
        var $control = $(this).parent().find("input");
        var uploadFolder = $control.data("upload");
        var mode = $control.data("mode");
        if (uploadFolder == undefined) {
            uploadFolder = "";
        }

        var allowedExtensions = [];

        //Upload image mode
        if (mode == 6 || mode == 7) {
            allowedExtensions = ['jpg', 'jpeg', 'png', 'gif', 'tif'];
        }

        var uploader = new qq.FileUploader({
            element: $(this)[0],
            multiple: false,
            uploadButtonText: '<i class="btn-xs btn btn-yellow fa fa-upload bigger-140 icon-only"></i>',
            action: '/Media/FileUpload',
            debug: false,
            dragAndDropFile: false,
            sizeLimit: maxSizeUploaded,
            allowedExtensions: allowedExtensions,
            onSubmit: function () {
                uploader.setParams({
                    dir: uploadFolder
                });

                return true;
            },
            onProgress: function () {
                $(".simple-upload .qq-upload-button").hide();
            },
            onComplete: function (id, filename, response) {
                if (response.Success) {
                    $control.val(response.fileLocation);
                    
                    // Check if there are any select image function in parent window
                    // If any then excute
                    if (window["selectMedia"] != undefined && window["selectMedia"].length > 0) {
                        siteHelper.executeFunction("selectMedia", window, response.fileLocation);
                    }
                }
                siteHelper.showMessage(response);
                $(".qq-upload-fail").remove();
                $(".qq-upload-success").remove();
                $(".simple-upload .qq-upload-button").show();
            },
            onCancel: function (id, fileName) {
                $(".simple-upload .qq-upload-button").show();
            },
            onError: function (id, fileName, xhr) {
                $(".simple-upload .qq-upload-button").show();
            }
        });
    });

    //Prevent drag image to body
    window.addEventListener("dragover", function (e) {
        e = e || event;
        e.preventDefault();
    }, false);
    window.addEventListener("drop", function (e) {
        e = e || event;
        e.preventDefault();
    }, false);
}

/* 
 * Register admin plugins
 */
function registerPlugins() {

    // Add mask to phone textbox
    if (phoneFormat != "*") {
        $('.phone-input').mask(phoneFormat);
    }

    /* 
     * Load spinner
     */
    var spinnerOpts = {
        lines: 13, // The number of lines to draw
        length: 20, // The length of each line
        width: 10, // The line thickness
        radius: 30, // The radius of the inner circle
        corners: 1, // Corner roundness (0..1)  
    };

    var target = document.getElementById('spinner-preview');
    var spinner = new Spinner(spinnerOpts).spin(target);

    /* Date time */

    $('.date-picker').datetimepicker({
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
    });

    $('.time-picker').datetimepicker({
        format: siteHelper.convertMomentFormat(timeFormat),
        //format: "HH:mm:ss",
        locale: language,
        showClear: true,
        useCurrent: false,
        keepInvalid: true,
        icons: {
            time: 'fa fa-clock-o',
            date: 'fa fa-calendar',
            up: 'fa fa-chevron-up',
            down: 'fa fa-chevron-down'
        }
    });

    $('.datetime-picker').datetimepicker({
        format: siteHelper.convertMomentFormat(dateTimeFormat),
        locale: language,
        showClear: true,
        keepInvalid: true,
        useCurrent: false,
        icons: {
            time: 'fa fa-clock-o',
            date: 'fa fa-calendar',
            up: 'fa fa-chevron-up',
            down: 'fa fa-chevron-down'
        },
        useStrict: false,
        sideBySide: true
    });

    /*
     * Select2 initialize
     */

    $(".multi-select").select2({
        allowClear: true
    });

    $(".single-select").select2({
        allowClear: true
    });

    $(".single-select-no-search").select2({
        minimumResultsForSearch: Infinity,
        allowClear: true
    });

    /* Register number */
    $.each($(".input-number"), function (index, value) {
        var selector = $(this);

        $(selector).spinner({
            create: function (event, ui) {

                if ($(this).attr("disabled") == "disabled") {
                    $(this).spinner("option", "disabled", true);
                }

                //add custom classes and icons
                $(this).next().addClass('btn btn-success').html('<i class="fa fa-plus"></i>')
                    .next().addClass('btn btn-danger').html('<i class="fa fa-minus"></i>');

                //larger buttons on touch devices
                if (ace.click_event == "tap") $(this).closest('.ui-spinner').addClass('ui-spinner-touch');
            }
        });

        var min = $(selector).data("min");
        if (min != null) {
            $(selector).spinner("option", "min", min);
        } else {
            $(selector).spinner("option", "min", 0);
        }

    });

    $('.ui-spinner-button').click(function () {
        $(this).siblings('input').change();
    });

    /*
     * Tooltips
     */
    $('[data-rel=tooltip]').tooltip({
        container: 'body',
        html: true
    });

    $('[data-rel=popover]').popover({
        container: 'body',
        html: true
    });

    $(".popover-hover").popover({
        trigger: "manual",
        html: true,
        animation: false,
        content: function () { return $(this).data('content'); }
    }).on("mouseenter", function () {
        var _this = this;
        $(this).popover("show");
        $(".popover").on("mouseleave", function () {
            $(_this).popover('hide');
        });
    }).on("mouseleave", function () {
        var _this = this;
        setTimeout(function () {
            if (!$(".popover:hover").length) {
                $(_this).popover("hide");
            }
        }, 100);
    });
}

/*
 * Register admin handlers
 */
function registerAdminHandlers() {

    //Toggle details error
    $("body").off("click", ".toggle-error-btn").on("click", ".toggle-error-btn", function () {
        var detailMessage = $(this).parent().parent().find(".detail-message");
        if (detailMessage.is(":visible")) {
            $(this).html("<i class='fa fa-chevron-down'></i> More Details");
        } else {
            $(this).html("<i class='fa fa-chevron-up'></i> Hide Details");
        }
        detailMessage.toggle();
    });

    /* 
     * Slide in helps
     */
    $("body").on("click", ".edit-slide-in-help", function () {
        var id = $(this).data("id");
        siteHelper.showPopup({
            href: '/Admin/SlideInHelps/PopupEdit/' + id,
            type: 'iframe',
            autoSize: false,
            width: 800,
            height: 820
        });
    });

    $("body").on("click", ".change-status-slide-in-help", function () {
        var icon = this;
        var id = $(icon).data("id");

        var i = $($(icon).children("i")[0]);
        var popover = $(".slide-in-help[data-id=" + id + "]");
        var currentTooltipText = popover.data('content');

        var status = $(icon).data("status");
        var data = { id: id, status: status };

        siteHelper.httpPost({
            url: '/Admin/SlideInHelps//ChangeStatus',
            data: data,
            success: function (response) {
                siteHelper.showMessage(response);
                if (response.Success) {
                    if (status == 'True') {
                        i.removeClass("fa-power-off");
                        i.addClass("fa-check");
                        popover.addClass("popover-disabled");
                        popover.attr('data-content', currentTooltipText.replace("True", "False").replace("fa-power-off", "fa-check")).data('popover');
                        popover.popover('hide').popover('show');
                    }
                    else {
                        $(icon).attr("data-status", "True");
                        i.addClass("fa-power-off");
                        i.removeClass("fa-check");
                        popover.removeClass("popover-disabled");
                        popover.attr('data-content', currentTooltipText.replace("False", "True").replace("fa-check", "fa-power-off")).data('popover');
                        popover.popover('hide').popover('show');
                    }
                }
            }
        });
    });

    //Check if cookie is enable or not
    enableCookie = checkCookieEnabled();
    if (!enableCookie) {
        $("#enableCookieMessage").show();
    }

    //Keep alive interval
    setInterval(function () {
        siteHelper.httpPost({
            url: "/Admin/Home/HeartBeat",
            showLoading: false,
            showError: false,
            data: {}
        });
    }, keepAliveInterval);

    //Set cookie for timezone
    setTimezoneCookie();
}

jQuery(function ($) {

    var _oldShow = $.fn.show;

    $.fn.show = function(speed, oldCallback) {
        return $(this).each(function() {
            var obj = $(this),
                newCallback = function() {
                    if ($.isFunction(oldCallback)) {
                        oldCallback.apply(obj);
                    }
                    obj.trigger('afterShow');
                };

            // you can trigger a before show if you want
            obj.trigger('beforeShow');

            // now use the old function to show the element passing the new callback
            _oldShow.apply(obj, [speed, newCallback]);
        });
    };
});