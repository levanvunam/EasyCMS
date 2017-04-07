define([
  "jquery", "underscore", "backbone"
  , "text!templates/popover/popover-main.html"
  , "text!templates/popover/popover-input.html"
  , "text!templates/popover/popover-select.html"
  , "text!templates/popover/popover-textarea.html"
  , "text!templates/popover/popover-textarea-split.html"
  , "text!templates/popover/popover-hidden.html"
  , "text!templates/popover/popover-checkbox.html"
  , "bootstrap"
], function (
  $, _, Backbone
  , _PopoverMain
  , _PopoverInput
  , _PopoverSelect
  , _PopoverTextArea
  , _PopoverTextAreaSplit
  , _PopoverHidden
  , _PopoverCheckbox
) {
    return Backbone.View.extend({
        tagName: "div"
      , className: "component"
      , initialize: function () {
          this.popoverTemplates = {
              "input": _.template(_PopoverInput),
              "select": _.template(_PopoverSelect),
              "textarea": _.template(_PopoverTextArea),
              "textarea-split": _.template(_PopoverTextAreaSplit),
              "hidden": _.template(_PopoverHidden),
              "checkbox": _.template(_PopoverCheckbox)
          };
      }
      , render: function (withAttributes) {
          var that = this;

          var html = "";
          var templateId = "[data-template=" + that.model.getTemplateName() + "]";
          if ($(templateId).length > 0) {
              var raw = $(templateId).data("content");
              html = _.template(raw, that.model.getValues());
          }
          var content = _.template(_PopoverMain)({
              "title": that.model.get("title"),
              "items": that.model.get("fields"),
              "popoverTemplates": that.popoverTemplates
          });

          if (withAttributes) {
              return this.$el.html(html).attr({
                  "data-content": content,
                  "data-title": that.model.get("title"),
                  "data-trigger": "manual",
                  "data-html": true,
                  "data-placement": "left"
              });
          } else {
              return this.$el.html(html);
          }
      }
    });
});
