define([
       "jquery", "underscore", "backbone"
      , "views/temp-snippet"
      , "helper/pubsub"
      , "text!templates/app/renderform.html"
], function (
  $, _, Backbone
  , TempSnippetView
  , PubSub
  , _renderForm
) {
    return Backbone.View.extend({
        tagName: "fieldset",
        initialize: function () {
            this.collection.on("add", this.render, this);
            this.collection.on("remove", this.render, this);
            this.collection.on("change", this.render, this);
            PubSub.on("mySnippetDrag", this.handleSnippetDrag, this);
            PubSub.on("tempMove", this.handleTempMove, this);
            PubSub.on("tempDrop", this.handleTempDrop, this);
            
            this.$build = $("#build");
            
            this.$buildingForm = $("#building-form");
            
            this.topWrapper = this.$build.offset().top - $(window).scrollTop();
            this.leftWrapper = this.$build.offset().left - $(window).scrollLeft();
            
            var that = this;
            $(window).resize(function () { //when window is scrolled
                that.topWrapper = that.$build.offset().top - $(window).scrollTop();
                that.leftWrapper = that.$build.offset().left - $(window).scrollLeft();
            });

            this.renderForm = _.template(_renderForm);
            this.render();
        },
        render: function () {
            //Render Snippet Views
            this.$el.empty();
            var that = this;
            _.each(this.collection.renderAll(), function (snippet) {
                that.$el.append(snippet);
            });
            var formComponents = [];
            $.each(this.collection.models, function(index, value) {
                formComponents.push({                    
                    title: value.get("title"),
                    template: value.get("template"),
                    fields: value.attributes.fields
                });
            });
            $("#jsonContent").val(JSON.stringify(formComponents));
            $("#content").val(that.renderForm({
                text: _.map(this.collection.renderAllClean(), function (e) {
                    return e.html();
                }).join("\n")
            }));
            this.$el.appendTo("#build form");

            $("[multiple=multiple]").select2();
            $("select").select2();
            
            this.delegateEvents();
        },
        getBottomAbove: function (eventY) {
            var that = this;
            var myFormBits = $(that.$el.find(".component"));
            var topelement = _.find(myFormBits, function (renderedSnippet) {
                var top = that.topWrapper + $(renderedSnippet).position().top + $(renderedSnippet).height();
                if (top > eventY) {
                    return true;
                } else {
                    return false;
                }
            });
            if (topelement) {
                return topelement;
            } else {
                return myFormBits[0];
            }
        },
        handleSnippetDrag: function (mouseEvent, snippetModel) {
            $("body").append(new TempSnippetView({ model: snippetModel }).render());
            this.collection.remove(snippetModel);
            PubSub.trigger("newTempPostRender", mouseEvent);
        },
        handleTempMove: function (mouseEvent) {
            var leftFrom = this.leftWrapper;
            var leftTo = this.$build.width() + leftFrom;
            var topFrom = this.topWrapper;
            var topTo = this.$build.height() + topFrom;

            $(".target").removeClass("target");
            if (mouseEvent.pageX >= leftFrom && mouseEvent.pageX < leftTo && mouseEvent.pageY >= topFrom && mouseEvent.pageY < topTo) {
                $(this.getBottomAbove(mouseEvent.pageY)).addClass("target");
            } else {
                $(".target").removeClass("target");
            }
        },
        handleTempDrop: function (mouseEvent, model, i) {
            var leftFrom = this.leftWrapper;
            var leftTo = this.$build.width() + leftFrom;
            var topFrom = this.topWrapper;
            var topTo = this.$build.height() + topFrom;
            

            if (mouseEvent.pageX >= leftFrom && mouseEvent.pageX < leftTo && mouseEvent.pageY >= topFrom && mouseEvent.pageY < topTo) {
                var index = $(".target").index();
                $(".target").removeClass("target");
                this.collection.add(model, { at: index + 1 });
            } else {
                $(".target").removeClass("target");
            }
        }
    });
});
