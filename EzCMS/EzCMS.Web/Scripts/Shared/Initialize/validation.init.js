//Input format
var dateFormat = Globalize.culture().calendar.patterns.d;
var timeFormat = Globalize.culture().calendar.patterns.t;
var dateTimeFormat = Globalize.culture().calendar.patterns.d + " " + Globalize.culture().calendar.patterns.t;

//Print format
var printDateFormat = Globalize.culture().calendar.patterns.w == undefined ? Globalize.culture().calendar.patterns.d : Globalize.culture().calendar.patterns.w;
var printLongDateFormat = Globalize.culture().calendar.patterns.W == undefined ? Globalize.culture().calendar.patterns.d : Globalize.culture().calendar.patterns.W;
var printTimeFormat = Globalize.culture().calendar.patterns.t;
var printDateTimeFormat = printDateFormat + " " + printTimeFormat;
var printLongDateTimeFormat = printLongDateFormat + " " + printTimeFormat;

var language = Globalize.culture().language;

// Clone original methods we want to call into
var originalMethods = {
    min: $.validator.methods.min,
    max: $.validator.methods.max,
    range: $.validator.methods.range
};

// Tell the validator that we want dates parsed using Globalize
$.validator.methods.date = function (value, element) {
    var tryDate = Globalize.parseDate(value, dateFormat);
    var tryTime = Globalize.parseDate(value, timeFormat);
    var tryDatetime = Globalize.parseDate(value, dateTimeFormat);
    return this.optional(element) || tryDate || tryTime || tryDatetime;
};

// Tell the validator that we want numbers parsed using Globalize
$.validator.methods.number = function (value, element) {
    var val = Globalize.parseFloat(value);
    return this.optional(element) || ($.isNumeric(val));
};

$.validator.addMethod('requiredif',
   function (value, element, parameters) {
       var id = '#' + parameters['dependentproperty'];

       // get the target value (as a string,
       // as that's what actual value will be)
       var targetvalue = parameters['targetvalue'];
       targetvalue = (targetvalue == null ? '' : targetvalue).toString();

       // get the actual value of the target control
       // note - this probably needs to cater for more
       // control types, e.g. radios
       var control = $(id);
       var controltype = control.attr('type');
       var actualvalue =
           controltype === 'checkbox' ?
               (control.is(':checked') ? 'true' : 'false') :
               control.val();

       // if the condition is true, reuse the existing
       // required field validator functionality
       if ($.trim(targetvalue) === $.trim(actualvalue) || ($.trim(targetvalue) === '*' && $.trim(actualvalue) !== ''))
           return $.validator.methods.required.call(
               this, value, element, parameters);

       return true;
   });

$.validator.unobtrusive.adapters.add(
    'requiredif',
    ['dependentproperty', 'targetvalue'],
    function (options) {
        options.rules['requiredif'] = {
            dependentproperty: options.params['dependentproperty'],
            targetvalue: options.params['targetvalue']
        };
        options.messages['requiredif'] = options.message;
    });

$.validator.addMethod('requiredinteger',
   function (value, element, parameters) {
       return value > 0;
   });

$.validator.unobtrusive.adapters.add(
    'requiredinteger',
    [],
    function (options) {
        options.rules['requiredinteger'] = { };
        options.messages['requiredinteger'] = options.message;
    });

$.validator.setDefaults({
    ignore: [],
    showErrors: function (errorMap, errorList) {
        $(".tab-content").find("div.tab-pane:hidden:has(span.field-validation-error)").each(function (index, tab) {
            var id = $(tab).attr("id");
            $('a[href="#' + id + '"]').tab('show');
            
            //Break the loop
            return false;
        });
        this.defaultShowErrors();     // to display the default error placement
    }
});