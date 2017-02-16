var roles = $('#RoleName');
$(function () {
    //Load Roles
    $.getJSON(rolesUrl, {}, function (response) {
        roles.empty();
        $.each(response, function (index, item) {
            roles.append($('<option>', {
                value: item.Id,
                text: item.Name
            }));
        });
    });
});