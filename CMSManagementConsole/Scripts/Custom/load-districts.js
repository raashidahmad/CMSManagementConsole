var districts = $('#DistrictId');

$(function () {
    //Load Districts
    $.getJSON(districtsUrl, {}, function (response) {
        districts.empty();
        $.each(response, function (index, item) {
            districts.append($('<option>', {
                value: item.Id,
                text: item.Name
            }));
        });
    });
});