
$(document).ready(function () {


});

function CopyTemplate(element) {

    var template = $(element).closest('property').children('value').children('template');

    var thtml = $(template).html();

    var newTemplate = $(thtml);

    newTemplate = $(thtml);

	$(template).closest('[data-propertyname]').children('value').children('.valuesList').append(newTemplate);

	newTemplate.show();

	Meta.AttachSearchHandler();

}