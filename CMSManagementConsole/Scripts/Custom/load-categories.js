$(function () {
    $.ajaxSetup({ cache: false });
    
    var categories = $('#CategoryId');
    categories.empty();

    $.ajax({
        url: categoriesUrl,
        type: 'GET',
        success: function (response) {
            if (response.length > 0) {
                $.each(response, function (index, item) {
                    categories.append($('<option>', {
                        value: item.Id,
                        text: item.Name
                    }));
                });
            }
        },
        error: function (error) {
        }
    });
});