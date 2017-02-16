$(function () {
    $.ajaxSetup({ cache: false });
    $('.form-horizontal').on('change', '#DistrictId', function () {
        loadSDCs();
    });
});

function loadSDCs() {
    var sdcs = $('#SDCId');
    var districtId = $('#DistrictId').val();
    sdcs.empty();

    $.ajax({
        url: sdcsUrl + '/' + districtId,
        type: 'GET',
        success: function (response) {
            if (response.length > 0) {
                $.each(response, function (index, item) {
                    sdcs.append($('<option>', {
                        value: item.Id,
                        text: item.Title
                    }));
                });
            }
        },
        error: function (error) {
        }
    });

   /* $.getJSON(sdcsUrl + '/' + districtId, {}, function (response) {
        
    });*/
}
