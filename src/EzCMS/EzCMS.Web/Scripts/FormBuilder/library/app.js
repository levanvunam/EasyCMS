define([
       "jquery", "underscore", "backbone"
       , "collections/snippets", "collections/my-form-snippets"
       , "views/tab", "views/my-form"
], function (
  $, _, Backbone
  , SnippetsCollection, MyFormSnippetsCollection
  , TabView, MyFormView
) {
    return {
        initialize: function () {
            var id = $("#formId").val();
            siteHelper.httpPost({
                url: '/Admin/Forms/GetFormConfigurations',
                dataType: 'json',
                data: { id: id },
                success: function (response) {
                    $('#build-form-box').css('visibility', 'visible');
                    if (response.Success) {
                        var components = $.parseJSON(response.Data.TabComponents);
                        $.each(components, function (index, value) {
                            var item = new TabView({
                                title: value.title,
                                collection: new SnippetsCollection(value.data)
                            });
                        });

                        //Make the first tab active!
                        $("#components .tab-pane").first().addClass("active");
                        $("#formtabs li").first().addClass("active");


                        // Bootstrap "My Form" with 'Form Name' snippet.
                        var formComponents = $.parseJSON(response.Data.FormComponents);
                        var form = new MyFormView({
                            title: "Original",
                            collection: new MyFormSnippetsCollection(formComponents)
                        });
                    }
                }
            });
        }
    };
});
